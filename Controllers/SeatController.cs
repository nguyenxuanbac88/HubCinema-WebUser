using Microsoft.AspNetCore.Mvc;

namespace MovieTicketWebsite.Controllers
{
    public class SeatController : Controller
    {
        public IActionResult Matrix([FromRoute(Name = "id")] int suatChieu)
        {
            Console.WriteLine("idSuatChieu = " + suatChieu);
            return View(suatChieu); // => sẽ render View/Seat/Matrix.cshtml
        }
    }
}
