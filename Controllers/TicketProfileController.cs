using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MovieTicketWebsite.Controllers
{
    public class TicketProfileController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseApiUrl = "http://api.dvxuanbac.com:2030";

        public TicketProfileController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> InvoiceTicket()
        {
            var token = HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrEmpty(token))
            {
                TempData["LoginMessage"] = "Chưa đăng nhập.";
                return RedirectToAction("Index", "Home");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            var response = await client.GetAsync($"{_baseApiUrl}/api/Booking/invoices");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Không thể lấy dữ liệu hóa đơn.";
                return View(new List<Invoice>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var invoices = JsonConvert.DeserializeObject<List<Invoice>>(json);

            return View(invoices);
        }
    }
}
