using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models;
using Newtonsoft.Json;

namespace MovieTicketWebsite.Controllers
{
    public class FoodController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseApiUrl;

        public FoodController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<IActionResult> ListFood()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_baseApiUrl}/Public/GetFoods");
            var foods = JsonConvert.DeserializeObject<List<FoodViewModel>>(response);

            return View(foods);
        }

        public async Task<IActionResult> FoodDetails(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_baseApiUrl}/Public/GetFoodById/{id}");
            var food = JsonConvert.DeserializeObject<FoodViewModel>(response);

            return View(food);
        }
    }
}
