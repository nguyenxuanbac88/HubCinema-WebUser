using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models.Vnpay;
using MovieTicketWebsite.Services.PayPal;
using MovieTicketWebsite.Services.VNPay;
using System.Text;
using System.Text.Json;

namespace MovieTicketWebsite.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IPayPalService _payPalService;

        public PaymentController(IVnPayService vnPayService, IHttpClientFactory httpClientFactory, IConfiguration configuration, IPayPalService payPalService)
        {
            _vnPayService = vnPayService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _payPalService = payPalService;
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

        // 2. Thêm 3 Action Method dành cho PayPal
        [HttpGet]
        public async Task<IActionResult> RedirectToPayPal()
        {
            var json = HttpContext.Session.GetString("VnPayData"); // Có thể đổi tên session nếu muốn dùng chung
            if (string.IsNullOrEmpty(json))
                return RedirectToAction("Index", "Home");

            var model = JsonSerializer.Deserialize<PaymentInformationModel>(json);

            // Tính tiền USD (Tỷ giá ví dụ: 25,000 VND = 1 USD)
            double usdAmount = model.Amount / 25000.0;
            string amountStr = usdAmount.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);

            var returnUrl = _configuration["PayPal:ReturnUrl"];
            var cancelUrl = _configuration["PayPal:CancelUrl"];

            try
            {
                var response = await _payPalService.CreateOrderAsync(amountStr, "USD", model.OrderDescription, returnUrl, cancelUrl);

                // Tìm link Approve để redirect user sang PayPal
                var approveLink = response?.links?.FirstOrDefault(x => x.rel == "approve")?.href;
                if (!string.IsNullOrEmpty(approveLink))
                {
                    return Redirect(approveLink);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Khởi tạo PayPal thất bại: {ex.Message}");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallbackPayPal(string token, string PayerID)
        {
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Index", "Home");

            try
            {
                // Gọi API Capture để chốt tiền
                var response = await _payPalService.CaptureOrderAsync(token);

                if (response != null && response.status == "COMPLETED")
                {
                    // THANH TOÁN THÀNH CÔNG -> Copy y hệt logic của VNPay
                    var invoiceId = HttpContext.Session.GetInt32("InvoiceId");
                    if (invoiceId == null) return RedirectToAction("Index", "Home");

                    HttpContext.Session.Remove("CountdownStart");
                    TempData["ClearSeatCountdown"] = true;

                    var jwtToken = HttpContext.Session.GetString("AccessToken");
                    if (!string.IsNullOrEmpty(jwtToken))
                    {
                        var client = _httpClientFactory.CreateClient();
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);
                        var url = $"http://api.dvxuanbac.com:2030/api/Booking/update-seat-status/{invoiceId}";
                        var body = new { message = "Cập nhật trạng thái ghế thành công" };
                        var jsonContent = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
                        await client.PostAsync(url, jsonContent);
                    }

                    return RedirectToAction("XemVe", "Ticket");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Capture PayPal thất bại: {ex.Message}");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult PaymentCancelPayPal()
        {
            // Nếu user bấm Cancel trên giao diện PayPal
            return RedirectToAction("Index", "Home");
        }
    }
}
