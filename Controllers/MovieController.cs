using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models;
using Newtonsoft.Json;

namespace MovieTicketWebsite.Controllers
{
    public class MovieController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MovieController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient();

            // 1. Lấy thông tin phim
            var movieResponse = await client.GetStringAsync($"http://api.dvxuanbac.com:2030/api/Public/GetMovieById/{id}");
            var movie = JsonConvert.DeserializeObject<MovieDetailViewModel>(movieResponse);
            movie.TrailerURL = ConvertYoutubeUrlToEmbed(movie.TrailerURL);

            // 2. Lấy regions + cinemas
            var filterDataResp = await client.GetStringAsync("http://api.dvxuanbac.com:2030/api/Schedule/filter-data");
            dynamic filterData = JsonConvert.DeserializeObject(filterDataResp);

            var regions = new List<string>();
            var cinemas = new List<CinemaInfo>();

            try
            {
                var rawRegions = filterData?.data?.regions as IEnumerable<dynamic>;
                if (rawRegions != null)
                    regions = rawRegions.Select(r => r?.ToString()).OfType<string>().ToList();

                var rawCinemas = filterData?.data?.cinemas as IEnumerable<dynamic>;
                if (rawCinemas != null)
                {
                    cinemas = rawCinemas.Select(c => new CinemaInfo
                    {
                        MaRap = (int)c.maRap,
                        TenRap = (string)c.tenRap,
                        Region = (string)c.region
                    }).ToList();
                }
            }
            catch
            {
                regions = new();
                cinemas = new();
            }

            // 3. Lấy danh sách ngày có lịch chiếu
            var dateList = new List<DateTime>();
            try
            {
                var datesResp = await client.GetStringAsync($"http://api.dvxuanbac.com:2030/api/Schedule/dates?maPhim={id}");
                dynamic datesData = JsonConvert.DeserializeObject(datesResp);
                var rawDates = datesData?.data as IEnumerable<dynamic>;
                if (rawDates != null)
                    dateList = rawDates.Select(d => DateTime.Parse((string)d)).ToList();
            }
            catch
            {
                dateList = new();
            }

            // 4. Lấy tất cả lịch chiếu cho mỗi ngày
            var showtimeMap = new Dictionary<string, List<TheaterShowtimes>>();
            if (dateList.Any())
            {
                foreach (var date in dateList)
                {
                    try
                    {
                        string formattedDate = date.ToString("yyyy-MM-dd");
                        var showtimeResp = await client.GetStringAsync($"http://api.dvxuanbac.com:2030/api/Schedule?maPhim={id}&date={formattedDate}");
                        dynamic showtimeData = JsonConvert.DeserializeObject(showtimeResp);

                        var rawShowtimes = showtimeData?.data as IEnumerable<dynamic>;
                        var dailyShowtimes = new List<TheaterShowtimes>();

                        if (rawShowtimes != null)
                        {
                            foreach (var t in rawShowtimes)
                            {
                                var theater = dailyShowtimes.FirstOrDefault(s => s.TheaterName == (string)t.tenRap);
                                var times = ((IEnumerable<dynamic>)t.gioChieu).Select(g => new ShowtimeItem
                                {
                                    Id = 0,
                                    StartTime = date.Date + TimeSpan.Parse((string)g),
                                    NgayChieu = date.Date
                                }).ToList();

                                if (theater != null)
                                    theater.Showtimes.AddRange(times);
                                else
                                    dailyShowtimes.Add(new TheaterShowtimes
                                    {
                                        TheaterName = (string)t.tenRap,
                                        Region = (string)t.region,      // <== Lấy từ API
                                        CinemaId = (int)t.maRap,        // <== Lấy từ API
                                        Showtimes = times
                                    });

                            }
                        }

                        showtimeMap[formattedDate] = dailyShowtimes;
                    }
                    catch { continue; }
                }
            }

            // 5. Chọn ngày mặc định (ngày đầu tiên có lịch)
            DateTime selected = dateList.FirstOrDefault();

            // 6. Gán dữ liệu vào ViewModel
            movie.Regions = regions;
            movie.Cinemas = cinemas;
            movie.AvailableDates = dateList;
            movie.SelectedDate = selected;
            movie.AllShowtimes = showtimeMap;

            // Tùy chọn: nếu muốn giữ lại hiển thị hôm nay trong phần đầu tiên
            movie.ShowtimesByTheater = showtimeMap.TryGetValue(selected.ToString("yyyy-MM-dd"), out var todayList)
                ? todayList
                : new();

            return View(movie);
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
