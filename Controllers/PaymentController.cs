using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models.Vnpay;
using MovieTicketWebsite.Services.VNPay;
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

            // Lấy InvoiceId từ session
            var invoiceId = HttpContext.Session.GetInt32("InvoiceId");
            if (invoiceId == null)
                return RedirectToAction("Index", "Home");

            // ✅ Huỷ đếm giờ
            HttpContext.Session.Remove("CountdownStart");
            TempData["ClearSeatCountdown"] = true;

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
