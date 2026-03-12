using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models;
using System.Text.Json;

namespace MovieTicketWebsite.Controllers
{
    public class CinemaController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public CinemaController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<IActionResult> GetCinema(int id)
        {
            // Lấy thông tin rạp
            var resCinema = await _httpClient.GetAsync($"{_baseApiUrl}/Public/GetCinemaById/{id}");
            if (!resCinema.IsSuccessStatusCode) return NotFound("Cinema not found");

            var jsonCinema = await resCinema.Content.ReadAsStringAsync();
            var cinema = JsonSerializer.Deserialize<Cinema>(jsonCinema, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Lấy danh sách mã phim theo rạp
            var resMovies = await _httpClient.GetAsync($"{_baseApiUrl}/Schedule/GetMovieIdsByCinema?maRap={id}");
            if (!resMovies.IsSuccessStatusCode) return View(cinema);

            var jsonMovies = await resMovies.Content.ReadAsStringAsync();
            var movieResult = JsonSerializer.Deserialize<ApiResult<List<int>>>(jsonMovies, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var movieIds = movieResult?.Data ?? new();

            var movieShowList = new List<MovieWithShowtimes>();

            foreach (var movieId in movieIds)
            {
                // Lấy thông tin phim
                var resMovie = await _httpClient.GetAsync($"{_baseApiUrl}/Public/GetMovieById/{movieId}");
                if (!resMovie.IsSuccessStatusCode) continue;

                var jsonMovie = await resMovie.Content.ReadAsStringAsync();
                var movie = JsonSerializer.Deserialize<Movie>(jsonMovie, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (movie == null) continue;

                var movieEntry = new MovieWithShowtimes
                {
                    MovieId = movieId,
                    MovieTitle = movie.MovieName,
                    PosterUrl = movie.CoverURL ?? "/images/doraemon.jpg"
                };

                // ✅ Lấy danh sách toàn bộ suất chiếu (có cả ngày + giờ)
                var resShowtimes = await _httpClient.GetAsync($"{_baseApiUrl}/Schedule/dates?maPhim={movieId}");
                if (!resShowtimes.IsSuccessStatusCode) continue;

                var jsonShowtimes = await resShowtimes.Content.ReadAsStringAsync();
                var showtimeResult = JsonSerializer.Deserialize<ApiResult<List<string>>>(jsonShowtimes, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var showData = showtimeResult?.Data ?? new();

                // ✅ Gom suất chiếu theo ngày
                foreach (var timeStr in showData)
                {
                    if (DateTime.TryParse(timeStr, out DateTime parsed))
                    {
                        string key = parsed.ToString("yyyy-MM-dd");

                        if (!movieEntry.ShowtimesByDate.ContainsKey(key))
                            movieEntry.ShowtimesByDate[key] = new();

                        // Mỗi ngày có thể nhiều giờ chiếu -> thêm vào TheaterShowtimes
                        var theater = movieEntry.ShowtimesByDate[key]
                            .FirstOrDefault(t => t.TheaterName == cinema.CinemaName);

                        if (theater == null)
                        {
                            theater = new TheaterShowtimes
                            {
                                TheaterName = cinema.CinemaName,
                                Showtimes = new List<GioChieuDto>()
                            };
                            movieEntry.ShowtimesByDate[key].Add(theater);
                        }

                        theater.Showtimes.Add(new GioChieuDto
                        {
                            StartTime = parsed.ToString("yyyy-MM-ddTHH:mm:ss"),
                            MaSuat = 0 // nếu API không trả mã suất, ta để 0
                        });
                    }
                }

                movieShowList.Add(movieEntry);
            }

            // ✅ ViewBag để View hiện tab ngày và suất chiếu
            ViewBag.MovieShowList = movieShowList;
            ViewBag.AvailableDates = movieShowList
                .SelectMany(m => m.ShowtimesByDate.Keys)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            ViewBag.SelectedDate = ViewBag.AvailableDates.Count > 0 ? ViewBag.AvailableDates[0] : null;

            return View(cinema);
        }



        public async Task<IActionResult> GetAllCinemas()
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/Public/GetCinemas");

            if (!response.IsSuccessStatusCode)
                return View("Error"); // hoặc trả lỗi phù hợp

            var json = await response.Content.ReadAsStringAsync();
            var cinemas = JsonSerializer.Deserialize<List<Cinema>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var grouped = cinemas
                .GroupBy(c => c.City)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.ToList());

            return View(grouped);
        }

    }
}
