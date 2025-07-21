using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models;
using Newtonsoft.Json;

public class HomeController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly string _baseApiUrl;

    public HomeController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _baseApiUrl = configuration["ApiSettings:BaseUrl"];
    }

    public async Task<IActionResult> ShowNews()
    {
        var newsUrl = $"{_baseApiUrl}/News";
        List<News> newsList = new();

        var response = await _httpClient.GetAsync(newsUrl);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var allNews = JsonConvert.DeserializeObject<List<News>>(json);
            newsList = allNews.Where(n => n.Category == 2).ToList();
        }

        return PartialView("_IndexNews", newsList);
    }

    public async Task<IActionResult> Index()
    {
        var movieUrl = $"{_baseApiUrl}/Public/GetMovies";
        var foodUrl = $"{_baseApiUrl}/Public/GetFoods";
        var newsUrl = $"{_baseApiUrl}/News";
        var bannerUrl = $"{_baseApiUrl}/Banner/active"; // <-- API lấy banner

        List<Movie> movies = new();
        List<ComboUuDai> combos = new();
        List<News> newsList = new();
        List<Banner> banners = new();

        // Phim
        var movieResponse = await _httpClient.GetAsync(movieUrl);
        if (movieResponse.IsSuccessStatusCode)
        {
            var json = await movieResponse.Content.ReadAsStringAsync();
            movies = JsonConvert.DeserializeObject<List<Movie>>(json);
        }

        // Combo
        var foodResponse = await _httpClient.GetAsync(foodUrl);
        if (foodResponse.IsSuccessStatusCode)
        {
            var json = await foodResponse.Content.ReadAsStringAsync();
            combos = JsonConvert.DeserializeObject<List<ComboUuDai>>(json);
        }

        // Tin tức
        var newsResponse = await _httpClient.GetAsync(newsUrl);
        if (newsResponse.IsSuccessStatusCode)
        {
            var json = await newsResponse.Content.ReadAsStringAsync();
            var allNews = JsonConvert.DeserializeObject<List<News>>(json);

           
            newsList = allNews.Where(n => n.Category == 2).ToList();
        }

        // Banner
        var bannerResponse = await _httpClient.GetAsync(bannerUrl);
        if (bannerResponse.IsSuccessStatusCode)
        {
            var json = await bannerResponse.Content.ReadAsStringAsync();
            banners = JsonConvert.DeserializeObject<List<Banner>>(json);
        }

        var model = new HomeIndexViewModel
        {
            Movies = movies,
            Combos = combos,
            News = newsList,
            Banners = banners
        };

        return View(model);
    }


    public async Task<IActionResult> Promotion()
    {
        var newsUrl = $"{_baseApiUrl}/News";
        List<News> newsList = new();

        var response = await _httpClient.GetAsync(newsUrl);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var allNews = JsonConvert.DeserializeObject<List<News>>(json);

            
            newsList = allNews.Where(n => n.Category == 2).ToList();
        }

        return View(newsList);
    }
}
