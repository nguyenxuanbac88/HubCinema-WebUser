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
        public IActionResult PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response.Success)
            {
                // Lưu response vào TempData dưới dạng JSON (System.Text.Json)
                TempData["VnpayResult"] = JsonSerializer.Serialize(response);
                return RedirectToAction("XemVe", "Ticket");
            }
            else
            {
                return RedirectToAction("ThanhToanThatBai", "Ticket");
            }
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
