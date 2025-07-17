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
                // Lưu vào DB, tạo vé hoặc chuyển hướng sang trang hiển thị vé
                TempData["VnpayResult"] = JsonSerializer.Serialize(response);
                return RedirectToAction("XemVe", "Ticket"); // ví dụ tên action là XemVe
            }
            else
            {
                return RedirectToAction("ThanhToanThatBai", "Ticket");
            }
        }



    }
}
