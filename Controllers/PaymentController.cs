using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models.Vnpay;
using MovieTicketWebsite.Services.VNPay;
using System.Text;
using System.Text.Json;

namespace MovieTicketWebsite.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly IHttpClientFactory _httpClientFactory;

        public PaymentController(IVnPayService vnPayService, IHttpClientFactory httpClientFactory)
        {
            _vnPayService = vnPayService;
            _httpClientFactory = httpClientFactory;
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

            // Lấy InvoiceId từ session
            var invoiceId = HttpContext.Session.GetInt32("InvoiceId");
            if (invoiceId == null)
                return RedirectToAction("Index", "Home");

            // ✅ Huỷ đếm giờ
            HttpContext.Session.Remove("CountdownStart");
            TempData["ClearSeatCountdown"] = true;

            // ✅ Gọi API cập nhật trạng thái ghế có kèm JWT
            try
            {
                var token = HttpContext.Session.GetString("AccessToken");
                if (!string.IsNullOrEmpty(token))
                {
                    var client = _httpClientFactory.CreateClient();
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var url = $"http://api.dvxuanbac.com:2030/api/Booking/update-seat-status/{invoiceId}";

                    var body = new
                    {
                        message = "Cập nhật trạng thái ghế thành công"
                    };

                    var jsonContent = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

                    var apiResponse = await client.PostAsync(url, jsonContent);

                    if (!apiResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"❌ Lỗi gọi update-seat-status: {(int)apiResponse.StatusCode} - {apiResponse.ReasonPhrase}");
                    }
                }
                else
                {
                    Console.WriteLine("❌ Không tìm thấy AccessToken trong session.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception khi gọi update-seat-status: {ex.Message}");
            }

            // 👉 Chỉ redirect thôi, không gọi API ở đây
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
