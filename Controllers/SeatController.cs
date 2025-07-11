using Microsoft.AspNetCore.Mvc;

namespace MovieTicketWebsite.Controllers
{
    public class SeatController : Controller
    {
        public IActionResult Matrix()
        {
            return View(); // => sẽ render View/Seat/Matrix.cshtml
        }
    }
}
