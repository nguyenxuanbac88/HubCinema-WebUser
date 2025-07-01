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
                dateList = rawDates?.Select(d => DateTime.Parse((string)d)).ToList() ?? new();
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
                            .Select(g => g.ToString())
                            .ToList();

                        allShowtimes.Add(new
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
                        });
                    }
                }
            }

            // Gán vào ViewModel
            movie.Regions = regions;
            movie.Cinemas = cinemas;
            movie.AvailableDates = dateList;
            movie.SelectedDate = dateList.FirstOrDefault();
            movie.AllShowtimesRawJson = JsonConvert.SerializeObject(allShowtimes); // thêm property này

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
