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
            var movieResponse = await client.GetStringAsync($"http://api.dvxuanbac.com:2030/api/Public/GetMovieById/{id}");
            var movie = JsonConvert.DeserializeObject<MovieDetailViewModel>(movieResponse);

            // ✅ Convert URL trailer về dạng embeddable
            movie.TrailerURL = ConvertYoutubeUrlToEmbed(movie.TrailerURL);
            // (Gợi ý) Gán giả dữ liệu lịch chiếu
            movie.ShowtimesByTheater = new List<TheaterShowtimes>
        {
            new TheaterShowtimes
            {
                TheaterName = "Galaxy Trung Chánh",
                Showtimes = new List<ShowtimeItem>
                {
                    new ShowtimeItem { Id = 1, StartTime = DateTime.Today.AddHours(14) },
                    new ShowtimeItem { Id = 2, StartTime = DateTime.Today.AddHours(17) },
                    new ShowtimeItem { Id = 3, StartTime = DateTime.Today.AddHours(20) }
                }
            }
        };

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
