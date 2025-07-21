using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models;
using MovieTicketWebsite.Models.Booking;
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

                // ✅ Lưu vào session tại đây trước khi mất
                HttpContext.Session.SetString("SeatInfo", json);

                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult SaveBookingData([FromBody] BookingRequestModel model)
        {
            model.SeatTotal = model.Seats.Sum(s => s.Price); // ✅ Gán lại đảm bảo
            var json = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString("BookingData", json);

            var total = model.Seats.Sum(s => s.Price);
            HttpContext.Session.SetString("SeatTotal", total.ToString());

            // ❌ Không nên dùng TempData ở đây vì đã bị mất sau khi render Matrix
            return Ok();
            // ✅ Ghi log ra console bằng response JSON (để log đỏ ở JS)
            //return Json(new
            //{
            //    success = true,
            //    message = $"[✔] SaveBookingData CALLED - Total: {total} - Seats: {model.Seats.Count}"
            //});
        }

    }
}
