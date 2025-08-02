using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models;
using Newtonsoft.Json;
using System.Globalization;

namespace MovieTicketWebsite.Controllers
{
    public class BookingController : Controller
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseApiUrl;

        public BookingController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();

            // 1. Lấy regions và danh sách rạp
            var filterDataResp = await client.GetStringAsync($"{_baseApiUrl}/Schedule/filter-data");
            dynamic filterData = JsonConvert.DeserializeObject(filterDataResp);

            var regions = ((IEnumerable<dynamic>)filterData?.data?.regions)
    ?.Select(r => r?.ToString())
    .OfType<string>()
    .Where(r => !string.Equals(r, "Toàn quốc", StringComparison.OrdinalIgnoreCase)) // ✨ Lọc bỏ
    .ToList() ?? new();


            var cinemas = ((IEnumerable<dynamic>)filterData?.data?.cinemas)
                ?.Select(c => new CinemaInfo
                {
                    MaRap = (int)c.maRap,
                    TenRap = (string)c.tenRap,
                    Region = (string)c.region
                }).ToList() ?? new();

            var movieMap = new Dictionary<int, Movie>();
            var allShowtimes = new List<object>();

            // 2. Duyệt qua từng rạp
            foreach (var cinema in cinemas)
            {
                try
                {
                    // 2.1 Lấy danh sách movieId của rạp
                    var url = $"{_baseApiUrl}/Schedule/GetMovieIdsByCinema?maRap={cinema.MaRap}";
                    var movieIdResp = await client.GetStringAsync(url);
                    var movieIdData = JsonConvert.DeserializeObject<IntArrayResponse>(movieIdResp);

                    var movieIds = movieIdData?.Data?.Distinct().ToList();
                    if (movieIds == null || !movieIds.Any()) continue;

                    // 2.2 Gọi GetMovieById cho mỗi phim
                    foreach (var movieId in movieIds)
                    {
                        if (movieMap.ContainsKey(movieId)) continue;

                        try
                        {
                            var movieResp = await client.GetStringAsync($"{_baseApiUrl}/Public/GetMovieById/{movieId}");
                            var movie = JsonConvert.DeserializeObject<Movie>(movieResp);

                            if (movie.Status != 1) continue; // chỉ lấy phim đang chiếu

                            movieMap[movieId] = movie;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[!] Lỗi lấy phim {movieId}: {ex.Message}");
                        }
                    }

                    // 2.3 Lấy suất chiếu theo phim và ngày
                    foreach (var movieId in movieIds)
                    {
                        if (!movieMap.ContainsKey(movieId)) continue;

                        try
                        {
                            // Gọi danh sách ngày chiếu có suất
                            var dateResp = await client.GetStringAsync($"{_baseApiUrl}/Schedule/dates?maPhim={movieId}");
                            dynamic dateData = JsonConvert.DeserializeObject(dateResp);

                            var dates = ((IEnumerable<dynamic>)dateData?.data)
    ?.Select(d => DateTime.ParseExact((string)d, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                            .ToString("yyyy-MM-dd"))
    .ToList();


                            if (dates == null || !dates.Any()) continue;

                            foreach (var date in dates)
                            {
                                var showtimeUrl = $"{_baseApiUrl}/Schedule?maPhim={movieId}&date={date}&region={Uri.EscapeDataString(cinema.Region)}&maRap={cinema.MaRap}";
                                var showtimeResp = await client.GetStringAsync(showtimeUrl);
                                dynamic showtimeData = JsonConvert.DeserializeObject(showtimeResp);

                                var rawShowtimes = showtimeData?.data as IEnumerable<dynamic>;
                                if (rawShowtimes == null) continue;

                                foreach (var item in rawShowtimes)
                                {
                                    var gioChieuList = ((IEnumerable<dynamic>)item.gioChieu)
                                        .Select(g => new
                                        {
                                            gioChieu = (string)g.gioChieu,
                                            suatChieu = (int)g.suatChieu
                                        }).ToList();

                                    allShowtimes.Add(new
                                    {
                                        maPhim = movieId,
                                        tenPhim = movieMap[movieId].MovieName,
                                        poster = movieMap[movieId].CoverURL,
                                        maRap = (int)item.maRap,
                                        tenRap = (string)item.tenRap,
                                        region = (string)item.region ?? cinema.Region,
                                        date = date,
                                        gioChieu = gioChieuList
                                    });
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[!] Lỗi lấy suất chiếu cho phim {movieId} tại rạp {cinema.MaRap}: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[!] Lỗi xử lý rạp {cinema.MaRap}: {ex.Message}");
                }
            }

            // 3. Truyền dữ liệu xuống view
            ViewBag.Regions = regions;
            ViewBag.Cinemas = cinemas;
            ViewBag.Movies = movieMap.Values.ToList();
            ViewBag.AllShowtimesJson = JsonConvert.SerializeObject(allShowtimes);

            return View();
        }


        [HttpGet]
        public IActionResult CancelFlow()
        {
            // ✅ Xoá toàn bộ session liên quan đến đặt vé
            HttpContext.Session.Remove("BookingFlow");
            HttpContext.Session.Remove("BookingData");
            HttpContext.Session.Remove("SeatTotal");
            HttpContext.Session.Remove("SeatInfo");
            HttpContext.Session.Remove("ComboData");
            HttpContext.Session.Remove("InvoiceId");
            HttpContext.Session.Remove("CountdownStart");

            return Ok(); // hoặc return RedirectToAction("Index", "Home");
        }
    }
}
