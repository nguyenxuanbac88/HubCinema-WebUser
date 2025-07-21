using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models;
using Newtonsoft.Json;

namespace MovieTicketWebsite.Controllers
{
    public class MovieController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseApiUrl;

        public MovieController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient();

            // Lấy thông tin phim
            var movieResponse = await client.GetStringAsync($"{_baseApiUrl}/Public/GetMovieById/{id}");
            var movie = JsonConvert.DeserializeObject<MovieDetailViewModel>(movieResponse);
            movie.TrailerURL = ConvertYoutubeUrlToEmbed(movie.TrailerURL);

            // Lấy regions + cinemas
            var filterDataResp = await client.GetStringAsync($"{_baseApiUrl}/Schedule/filter-data");
            dynamic filterData = JsonConvert.DeserializeObject(filterDataResp);

            var regions = ((IEnumerable<dynamic>)filterData?.data?.regions)
                ?.Select(r => r?.ToString()).OfType<string>().ToList() ?? new();

            var cinemas = ((IEnumerable<dynamic>)filterData?.data?.cinemas)
                ?.Select(c => new CinemaInfo
                {
                    MaRap = (int)c.maRap,
                    TenRap = (string)c.tenRap,
                    Region = (string)c.region
                }).ToList() ?? new();

            // Lấy danh sách ngày chiếu
            var dateList = new List<DateTime>();
            try
            {
                var datesResp = await client.GetStringAsync($"{_baseApiUrl}/Schedule/dates?maPhim={id}");
                dynamic datesData = JsonConvert.DeserializeObject(datesResp);
                var rawDates = datesData?.data as IEnumerable<dynamic>;
                // Pseudocode:
                // 1. Check if rawDates is not null.
                // 2. Convert rawDates to a list of string or dynamic.
                // 3. For each item, parse to DateTime safely (handle nulls and invalid formats).
                // 4. If parsing fails, skip that item.

                dateList = new List<DateTime>();
                if (rawDates != null)
                {
                    foreach (var d in rawDates)
                    {
                        if (d == null) continue;
                        // Fix for CS8197: Explicitly specify the type of the out variable 'dt' in DateTime.TryParse.
                        if (DateTime.TryParse(d.ToString(), out DateTime dt))
                        {
                            dateList.Add(dt);
                        }
                    }

                }
            }
            catch { }

            // Chuẩn bị allShowtimes
            var allShowtimes = new List<object>();
            foreach (var date in dateList)
            {
                var formattedDate = date.ToString("yyyy-MM-dd");
                var showtimeResp = await client.GetStringAsync($"{_baseApiUrl}/Schedule?maPhim={id}&date={formattedDate}");
                dynamic showtimeData = JsonConvert.DeserializeObject(showtimeResp);

                var rawShowtimes = showtimeData?.data as IEnumerable<dynamic>;
                if (rawShowtimes != null)
                {
                    foreach (var item in rawShowtimes)
                    {
                        var gioChieuList = ((IEnumerable<dynamic>)item.gioChieu)
                            .Select(g => new
                            {
                                gioChieu = (string)g.gioChieu,
                                suatChieu = (int)g.suatChieu
                            })
                            .ToList();

                        // Log giá trị của gioChieuList
                        Console.WriteLine("gioChieuList: " + JsonConvert.SerializeObject(gioChieuList));



                        var showtimeObj = new
                        {
                            maRap = (int)item.maRap,
                            tenRap = (string)item.tenRap,
                            regions = new List<string>
                            {
                                (string)item.region ?? cinemas.FirstOrDefault(c => c.MaRap == (int)item.maRap)?.Region,
                                "Toàn quốc"
                            },
                            date = formattedDate,
                            gioChieu = gioChieuList
                        };

                        allShowtimes.Add(showtimeObj);
                        // Log giá trị của showtimeObj
                        Console.WriteLine("showtimeObj: " + JsonConvert.SerializeObject(showtimeObj));
                    }
                }
            }

            // Gán vào ViewModel
            movie.Regions = regions;
            movie.Cinemas = cinemas;
            movie.AvailableDates = dateList;
            movie.SelectedDate = dateList.FirstOrDefault();
            movie.AllShowtimesRawJson = JsonConvert.SerializeObject(allShowtimes); // thêm property này

            // Lấy thông tin của một suất chiếu đầu tiên nếu có
            var firstShowtime = allShowtimes
                .SelectMany(s => ((IEnumerable<dynamic>)((dynamic)s).gioChieu)
                    .Select(g => new
                    {
                        SuatChieuId = (int)g.suatChieu,
                        GioChieu = g.gioChieu.ToString(),
                        NgayChieu = ((dynamic)s).date,
                        TenRap = (string)((dynamic)s).tenRap,
                        TenPhong = "Phòng 1" // Bạn cần thay bằng thông tin thật nếu có
                    }))
                .FirstOrDefault();

            if (firstShowtime != null)
            {
                var ngay = DateTime.Parse(firstShowtime.NgayChieu);
                var gio = TimeSpan.Parse(firstShowtime.GioChieu);

                var seatModel = new SeatSelectionViewModel
                {
                    MovieId = id,
                    MovieTitle = movie.MovieName,
                    PosterUrl = movie.CoverURL,
                    CinemaName = firstShowtime.TenRap,
                    RoomName = firstShowtime.TenPhong,
                    ShowTime = ngay.Date.Add(gio),
                    ShowId = firstShowtime.SuatChieuId,
                    AgeRestriction = movie.AgeRestriction
                };

                TempData["SeatInfo"] = JsonConvert.SerializeObject(seatModel);
                TempData.Keep("SeatInfo");
            }


            return View(movie);
        }




        public IActionResult ChonGhe(int id)
        {
            // Lấy dữ liệu đã lưu từ TempData
            if (TempData["SeatInfo"] is string json)
            {
                var model = JsonConvert.DeserializeObject<SeatSelectionViewModel>(json);
                if (model != null)
                {
                    model.ShowId = id; // Cập nhật lại ShowId nếu cần

                    TempData["SeatSelectionData"] = JsonConvert.SerializeObject(model);
                    TempData.Keep("SeatSelectionData");
                    return RedirectToAction("Matrix", "Seat", new { id });
                }
            }

            // Nếu không có dữ liệu thì chỉ tạo model đơn giản
            var fallbackModel = new SeatSelectionViewModel { ShowId = id };
            TempData["SeatSelectionData"] = JsonConvert.SerializeObject(fallbackModel);
            TempData.Keep("SeatSelectionData");

            return RedirectToAction("Matrix", "Seat", new { id });
        }






        private string ConvertYoutubeUrlToEmbed(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return "https://www.youtube.com/embed/EQ9CGrgIq9M"; // Trailer mặc định

            try
            {
                var uri = new Uri(url);

                if (url.Contains("watch?v="))
                {
                    var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
                    var videoId = query["v"].ToString();
                    if (!string.IsNullOrEmpty(videoId))
                        return $"https://www.youtube.com/embed/{videoId}";
                }

                if (url.Contains("youtu.be"))
                {
                    var id = uri.Segments.LastOrDefault()?.Trim('/');
                    if (!string.IsNullOrEmpty(id))
                        return $"https://www.youtube.com/embed/{id}";
                }
            }
            catch
            {
                // Nếu url không parse được hoặc không hợp lệ
            }

            return "https://www.youtube.com/embed/EQ9CGrgIq9M"; // fallback
        }


    }

}
