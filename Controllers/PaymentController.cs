using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models.Vnpay;
using MovieTicketWebsite.Services.VNPay;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MovieTicketWebsite.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;

        public PaymentController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        [HttpPost]
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            return Redirect(url);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            TempData["VnpayResult"] = JsonSerializer.Serialize(response);

            // ✅ Gọi API cập nhật trạng thái ghế
            var invoiceId = HttpContext.Session.GetInt32("InvoiceId");
            if (invoiceId != null)
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://api.dvxuanbac.com:2030");
                var token = HttpContext.Session.GetString("AccessToken");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                await Task.Delay(1500); // đợi 1.5 giây
                var apiRes = await client.PostAsync($"/api/Booking/update-seat-status/{invoiceId}", null);
                if (apiRes.IsSuccessStatusCode)
                {
                    Console.WriteLine("✅ Seat status updated successfully.");
                }
                else
                {
                    var err = await apiRes.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error updating seat status: {apiRes.StatusCode} - {err}");
                }
            }

            // ✅ Huỷ đếm giờ
            HttpContext.Session.Remove("CountdownStart");
            TempData["ClearSeatCountdown"] = true;


            return RedirectToAction("XemVe", "Ticket");
        }


        [HttpGet]
        public IActionResult RedirectToVNPay()
        {
            var json = HttpContext.Session.GetString("VnPayData");
            if (string.IsNullOrEmpty(json))
                return RedirectToAction("Index", "Home");

            var model = JsonSerializer.Deserialize<PaymentInformationModel>(json);
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }
    }
}
