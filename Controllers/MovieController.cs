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

        // ======================
        // 🎬 DETAILS - Chi tiết phim
        // ======================
        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient();

            // 🔹 1. Lấy thông tin phim
            var movieResponse = await client.GetStringAsync($"{_baseApiUrl}/Public/GetMovieById/{id}");
            var movie = JsonConvert.DeserializeObject<MovieDetailViewModel>(movieResponse);
            movie.TrailerURL = ConvertYoutubeUrlToEmbed(movie.TrailerURL);

            // 🔹 2. Lấy dữ liệu filter (Regions + Cinemas)
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

            // 🔹 3. Lấy danh sách ngày chiếu
            var dateList = new List<DateTime>();
            try
            {
                var datesResp = await client.GetStringAsync($"{_baseApiUrl}/Schedule/dates?maPhim={id}");
                dynamic datesData = JsonConvert.DeserializeObject(datesResp);
                var rawDates = datesData?.data as IEnumerable<dynamic>;
                if (rawDates != null)
                {
                    foreach (var d in rawDates)
                    {
                        if (d == null) continue;
                        if (DateTime.TryParse(d.ToString(), out DateTime dt))
                            dateList.Add(dt);
                    }
                }
            }
            catch { }
            // ✅ Fix: chỉ giữ phần ngày, loại trùng, sắp xếp
            dateList = dateList
                .Select(d => d.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();
            // 🔹 4. Lấy danh sách suất chiếu (lọc giờ, gộp theo rạp)
            var allShowtimes = new List<object>();
            var now = DateTime.Now.AddMinutes(-20); // ✅ trừ hao 20 phút
            var today = now.Date;

            foreach (var date in dateList)
            {
                var formattedDate = date.ToString("yyyy-MM-dd");
                var showtimeResp = await client.GetStringAsync($"{_baseApiUrl}/Schedule?maPhim={id}&date={formattedDate}");
                dynamic showtimeData = JsonConvert.DeserializeObject(showtimeResp);
                var rawShowtimes = showtimeData?.data as IEnumerable<dynamic>;
                if (rawShowtimes == null) continue;

                // ✅ Gom suất chiếu theo rạp
                foreach (var group in rawShowtimes.GroupBy(x => (int)x.maRap))
                {
                    var first = group.First();
                    var maRap = (int)first.maRap;
                    var tenRap = (string)first.tenRap;
                    var region = (string)first.region ?? cinemas.FirstOrDefault(c => c.MaRap == maRap)?.Region;

                    // ✅ Gộp và lọc giờ chiếu
                    var gioChieuList = group
    // Gom tất cả giờ chiếu của các phòng trong rạp
    .SelectMany(x => ((IEnumerable<dynamic>)x.gioChieu)
        .Select(g => new
        {
            gioChieu = (string)g.gioChieu,
            suatChieu = (int)g.suatChieu
        }))
    // ✅ Loại trùng ngay tại đây (trước khi lọc)
    .GroupBy(g => g.gioChieu)
    .Select(g => g.First())
    // ✅ Parse và lọc giờ hợp lệ
    .Where(g =>
    {
        if (DateTime.TryParse(g.gioChieu, out DateTime timePart))
        {
            var fullDateTime = date.Date
                .AddHours(timePart.Hour)
                .AddMinutes(timePart.Minute)
                .AddSeconds(timePart.Second);

            if (fullDateTime.Date == today)
                return fullDateTime >= now;

            return fullDateTime.Date > today;
        }
        return false;
    })
    .OrderBy(g => g.gioChieu)
    .ToList();


                    if (!gioChieuList.Any()) continue;

                    var showtimeObj = new
                    {
                        maRap = maRap,
                        tenRap = tenRap,
                        regions = new List<string> { region, "Toàn quốc" },
                        date = formattedDate,
                        gioChieu = gioChieuList
                    };

                    allShowtimes.Add(showtimeObj);
                }
            }

            // 🔹 5. Gán lại ViewModel
            movie.Regions = regions;
            movie.Cinemas = cinemas;
            movie.AvailableDates = dateList;
            movie.SelectedDate = dateList.FirstOrDefault();
            movie.AllShowtimesRawJson = JsonConvert.SerializeObject(allShowtimes);

            // 🔹 6. Lưu session (đặt ghế)
            HttpContext.Session.SetString("AllShowtimes", movie.AllShowtimesRawJson);
            var seatModel = new SeatSelectionViewModel
            {
                MovieId = id,
                MovieTitle = movie.MovieName,
                PosterUrl = movie.CoverURL,
                Poster = movie.CoverURL,
                AgeRestriction = movie.AgeRestriction
            };
            HttpContext.Session.SetString("SeatInfo", JsonConvert.SerializeObject(seatModel));

            // 🔹 7. Trả về view
            return View(movie);
        }


        // ======================
        // 🎟️ CHON GHE - Khi click suất chiếu
        // ======================
        public IActionResult ChonGhe(int id)
        {
            try
            {
                // 🔹 Lấy SeatInfo cũ (để giữ poster, tên phim, v.v.)
                var oldSeatJson = HttpContext.Session.GetString("SeatInfo");
                if (string.IsNullOrEmpty(oldSeatJson))
                    return RedirectToAction("Index", "Home");
                var seatModel = JsonConvert.DeserializeObject<SeatSelectionViewModel>(oldSeatJson);

                // 🔹 Lấy danh sách suất chiếu
                var allShowtimesJson = HttpContext.Session.GetString("AllShowtimes");
                if (string.IsNullOrEmpty(allShowtimesJson))
                    return RedirectToAction("Index", "Home");
                var allShowtimes = JsonConvert.DeserializeObject<List<dynamic>>(allShowtimesJson);

                // 🔹 Tìm suất người dùng chọn
                dynamic found = null;
                foreach (var s in allShowtimes)
                {
                    foreach (var g in (IEnumerable<dynamic>)s.gioChieu)
                    {
                        if ((int)g.suatChieu == id)
                        {
                            found = new
                            {
                                tenRap = (string)s.tenRap,
                                gio = (string)g.gioChieu,
                                ngay = (string)s.date
                            };
                            break;
                        }
                    }
                    if (found != null) break;
                }

                if (found == null)
                    return RedirectToAction("Index", "Home");

                // 🔹 Cập nhật thông tin suất mới
                DateTime.TryParse((string)found.ngay, out DateTime ngay);
                TimeSpan.TryParse((string)found.gio, out TimeSpan gio);

                seatModel.ShowId = id;
                seatModel.CinemaName = found.tenRap;
                seatModel.ShowTime = ngay.Add(gio);

                // 🔹 Ghi đè lại session
                HttpContext.Session.SetString("SeatInfo", JsonConvert.SerializeObject(seatModel));

                Console.WriteLine("✅ SeatInfo updated: " + JsonConvert.SerializeObject(seatModel));

                return RedirectToAction("Matrix", "Seat", new { id });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi ChonGhe: " + ex.Message);
                return RedirectToAction("Index", "Home");
            }
        }

        // ======================
        // 🎥 Xử lý URL trailer
        // ======================
        private string ConvertYoutubeUrlToEmbed(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return "https://www.youtube.com/embed/EQ9CGrgIq9M";

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
            catch { }

            return "https://www.youtube.com/embed/EQ9CGrgIq9M";
        }
    }
}
