using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models.Vnpay;
using System.Text.Json;

namespace MovieTicketWebsite.Controllers
{
    public class TicketController : Controller
    {
        public IActionResult XemVe()
        {
            if (!TempData.ContainsKey("VnpayResult"))
                return RedirectToAction("Index", "Home");

            var json = TempData["VnpayResult"].ToString();
            var response = JsonSerializer.Deserialize<PaymentResponseModel>(json);

            // Optionally: load thêm thông tin vé từ DB nếu bạn đã lưu
            return View(response);
        }

    }
}
