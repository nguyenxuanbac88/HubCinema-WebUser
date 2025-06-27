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

        public async Task<IActionResult> Details(int id, string? selectedDate)
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
                // fallback nếu dữ liệu API lỗi format
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

            // Chọn ngày:
            DateTime selected = dateList.FirstOrDefault(); // fallback mặc định
            if (!string.IsNullOrEmpty(selectedDate) && DateTime.TryParse(selectedDate, out DateTime parsedDate))
            {
                if (dateList.Contains(parsedDate))
                {
                    selected = parsedDate;
                }
            }

            // 4. Lấy lịch chiếu cho tất cả các ngày có lịch chiếu
            var showtimes = new List<TheaterShowtimes>();
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
                        if (rawShowtimes != null)
                        {
                            foreach (var t in rawShowtimes)
                            {
                                var existingTheater = showtimes.FirstOrDefault(s => s.TheaterName == (string)t.tenRap);
                                var times = ((IEnumerable<dynamic>)t.gioChieu).Select(g => new ShowtimeItem
                                {
                                    Id = 0,
                                    StartTime = date.Date + TimeSpan.Parse((string)g),
                                    NgayChieu = date.Date // Gán ngày chiếu rõ ràng

                                }).ToList();

                                if (existingTheater != null)
                                {
                                    existingTheater.Showtimes.AddRange(times);
                                }
                                else
                                {
                                    showtimes.Add(new TheaterShowtimes
                                    {
                                        TheaterName = (string)t.tenRap,
                                        Showtimes = times
                                    });
                                }
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }


            // 5. Gán dữ liệu về ViewModel
            movie.Regions = regions ?? new();
            movie.Cinemas = cinemas ?? new();
            movie.AvailableDates = dateList ?? new();
            movie.ShowtimesByTheater = showtimes ?? new();
            movie.SelectedDate = selected; // gán ngày người dùng chọn

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
