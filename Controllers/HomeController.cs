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

    public async Task<IActionResult> Index()
    {
        var movieUrl = $"{_baseApiUrl}/Public/GetMovies";
        var foodUrl = $"{_baseApiUrl}/Public/GetFoods";

        List<Movie> movies = new();
        List<ComboUuDai> combos = new();

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

        // Truyền xuống view model
        var model = new HomeIndexViewModel
        {
            Movies = movies,
            Combos = combos
        };

        return View(model);
    }
    public IActionResult promotion()
    {
        return View();
    }
}
