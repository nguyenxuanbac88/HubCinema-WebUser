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
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/Public/GetCinemaById/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound("Cinema not found");

            var json = await response.Content.ReadAsStringAsync();
            var cinema = JsonSerializer.Deserialize<Cinema>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

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
