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
            var json = HttpContext.Session.GetString("SeatInfo");
            if (string.IsNullOrEmpty(json))
            {
                // Nếu không có dữ liệu thì về trang chọn phim
                return RedirectToAction("Index", "Home");
            }

            var model = JsonConvert.DeserializeObject<SeatSelectionViewModel>(json);
            if (model == null || model.ShowId != id)
            {
                // Nếu sai Suất chiếu thì cũng về lại Index
                return RedirectToAction("Index", "Home");
            }

            return View(model);
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
