using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models;
using Newtonsoft.Json;

namespace MovieTicketWebsite.Controllers
{
    public class SeatController : Controller
    {
        public IActionResult Matrix(int id)
        {
            if (TempData["SeatSelectionData"] is string json)
            {
                var model = JsonConvert.DeserializeObject<SeatSelectionViewModel>(json);
                return View(model);
            }

            return NotFound();
        }

    }
}
