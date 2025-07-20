using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models;
using Newtonsoft.Json;

namespace MovieTicketWebsite.Controllers
{
    public class NewsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public NewsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<IActionResult> Detail(int id)
        {
            // Gọi bài viết
            var newsResponse = await _httpClient.GetAsync($"{_baseApiUrl}/News/{id}");
            if (!newsResponse.IsSuccessStatusCode)
                return NotFound();

            var newsJson = await newsResponse.Content.ReadAsStringAsync();
            var news = JsonConvert.DeserializeObject<News>(newsJson);

            // Gọi các bài viết liên quan (giả sử có sẵn API hoặc lấy all rồi lọc)
            var allNewsResponse = await _httpClient.GetAsync($"{_baseApiUrl}/News");
            var relatedNews = new List<News>();
            if (allNewsResponse.IsSuccessStatusCode)
            {
                var json = await allNewsResponse.Content.ReadAsStringAsync();
                var allNews = JsonConvert.DeserializeObject<List<News>>(json);
                relatedNews = allNews.Where(n => n.Id != id).Take(4).ToList();
            }

            // Gọi API phim đang chiếu
            var movies = new List<Movie>();
            var movieResponse = await _httpClient.GetAsync($"{_baseApiUrl}/Public/GetMovies");
            if (movieResponse.IsSuccessStatusCode)
            {
                var json = await movieResponse.Content.ReadAsStringAsync();
                movies = JsonConvert.DeserializeObject<List<Movie>>(json);
            }

            ViewBag.RelatedNews = relatedNews;
            ViewBag.ShowingMovies = movies.Take(4).ToList(); // Lấy 4 phim đang chiếu

            return View(news);
        }

    }
}
