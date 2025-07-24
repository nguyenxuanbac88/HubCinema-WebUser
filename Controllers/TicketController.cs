using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models;
using MovieTicketWebsite.Models.Booking;
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


            // Lấy các dữ liệu bổ sung từ session
            var bookingJson = HttpContext.Session.GetString("BookingData");
            var seatInfoJson = HttpContext.Session.GetString("SeatInfo");
            var comboJson = HttpContext.Session.GetString("ComboData");

            var booking = JsonSerializer.Deserialize<BookingRequestModel>(bookingJson ?? "{}");
            var seatInfo = JsonSerializer.Deserialize<SeatSelectionViewModel>(seatInfoJson ?? "{}");
            var combo = JsonSerializer.Deserialize<ComboSelectionModel>(comboJson ?? "{}");

            var comboTotal = combo?.Foods?.Sum(f => f.Price * f.Quantity) ?? 0;
            var totalAmount = booking.SeatTotal + (decimal)comboTotal;

            var ticket = new TicketViewModel
            {
                MovieTitle = seatInfo.MovieTitle,
                CinemaName = seatInfo.CinemaName,
                RoomName = seatInfo.RoomName,
                ShowTime = seatInfo.ShowTime,
                Seats = string.Join(", ", booking.Seats.Select(s => s.MaGhe)),
                Price = totalAmount,
                OrderId = response.OrderId,
                PosterUrl = seatInfo.Poster, // ➕ thêm dòng này

                // ✅ Thêm 2 dòng này
                Foods = combo?.Foods.Select(f => new FoodDto
                {
                    IdFood = f.IdFood,
                    FoodName = f.FoodName,
                    Price = (int)f.Price,
                    Quantity = f.Quantity
                }).ToList() ?? new List<FoodDto>(),

                ComboTotal = (decimal)comboTotal
            };

            // ⛔ Đây là bước bạn đang thiếu: xoá cookie booking_flow
            Response.Cookies.Delete("booking_flow"); // ✅ Xoá ngay khi hiện vé
            ViewBag.inBookingFlow = false; // ✳️ Truyền cứng
            return View(ticket);
        }


    }
}
