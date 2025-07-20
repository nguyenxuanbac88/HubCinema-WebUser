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
            newsList = JsonConvert.DeserializeObject<List<News>>(json);
        }

        return PartialView("_IndexNews", newsList);
    }

    public async Task<IActionResult> Index()
    {
        var movieUrl = $"{_baseApiUrl}/Public/GetMovies";
        var foodUrl = $"{_baseApiUrl}/Public/GetFoods";
        var newsUrl = $"{_baseApiUrl}/News";

        List<Movie> movies = new();
        List<ComboUuDai> combos = new();
        List<News> newsList = new();

        // Gọi API phim
        var movieResponse = await _httpClient.GetAsync(movieUrl);
        if (movieResponse.IsSuccessStatusCode)
        {
            var json = await movieResponse.Content.ReadAsStringAsync();
            movies = JsonConvert.DeserializeObject<List<Movie>>(json);
        }

        // Gọi API combo
        var foodResponse = await _httpClient.GetAsync(foodUrl);
        if (foodResponse.IsSuccessStatusCode)
        {
            var json = await foodResponse.Content.ReadAsStringAsync();
            combos = JsonConvert.DeserializeObject<List<ComboUuDai>>(json);
        }

        // Gọi API news
        var newsResponse = await _httpClient.GetAsync(newsUrl);
        if (newsResponse.IsSuccessStatusCode)
        {
            var json = await newsResponse.Content.ReadAsStringAsync();
            newsList = JsonConvert.DeserializeObject<List<News>>(json);
        }

        var model = new HomeIndexViewModel
        {
            Movies = movies,
            Combos = combos,
            News = newsList
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
            newsList = JsonConvert.DeserializeObject<List<News>>(json);
        }

        return View(newsList); 
    }


}
