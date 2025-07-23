using Microsoft.AspNetCore.Mvc;

namespace MovieTicketWebsite.Controllers
{
    public class BookingController : Controller
    {
        [HttpGet]
        public IActionResult CancelFlow()
        {
            // ✅ Xoá toàn bộ session liên quan đến đặt vé
            HttpContext.Session.Remove("BookingFlow");
            HttpContext.Session.Remove("BookingData");
            HttpContext.Session.Remove("SeatTotal");
            HttpContext.Session.Remove("SeatInfo");
            HttpContext.Session.Remove("ComboData");
            HttpContext.Session.Remove("InvoiceId");
            HttpContext.Session.Remove("CountdownStart");

            return Ok(); // hoặc return RedirectToAction("Index", "Home");
        }
    }
}
