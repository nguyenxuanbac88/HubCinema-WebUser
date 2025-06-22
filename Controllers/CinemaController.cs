using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models;
using System.Text.Json;

namespace MovieTicketWebsite.Controllers
{
    public class CinemaController : Controller
    {
        private readonly HttpClient _httpClient;

        public CinemaController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> GetCinema(int id)
        {
            var response = await _httpClient.GetAsync($"http://web.dvxuanbac.com:2030/api/Public/GetCinemaById/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound("Cinema not found");

            var json = await response.Content.ReadAsStringAsync();
            var cinema = JsonSerializer.Deserialize<Cinema>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(cinema);
        }
    }
}
