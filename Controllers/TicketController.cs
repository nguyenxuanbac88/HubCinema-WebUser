using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models;
using MovieTicketWebsite.Services.Transaction;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MovieTicketWebsite.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITransactionService _transactionService;
        public TicketController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        public async Task<IActionResult> XemVe()
        {
            var invoiceId = HttpContext.Session.GetInt32("InvoiceId");
            if (invoiceId == null)
                return RedirectToAction("Index", "Home");

            var token = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Index", "Home");

            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://api.dvxuanbac.com:2030");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"/api/Invoice/{invoiceId}");
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Index", "Home");

            var json = await response.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<TicketViewModel>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> GetTicketPartial(int orderId)
        {
            var ticket = await _transactionService.GetInvoiceByOrderIdAsync(orderId);
            if (ticket == null)
                return NotFound();

            return PartialView("_TicketPartial", ticket);
        }
    }
}
