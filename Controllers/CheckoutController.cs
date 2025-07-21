using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models;
using MovieTicketWebsite.Models.Booking;
using Newtonsoft.Json;

namespace MovieTicketWebsite.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            var bookingJson = HttpContext.Session.GetString("BookingData");
            if (string.IsNullOrEmpty(bookingJson))
                return RedirectToAction("Index", "Home");

            var booking = JsonConvert.DeserializeObject<BookingRequestModel>(bookingJson);

            var seatInfoJson = HttpContext.Session.GetString("SeatInfo");
            var seatInfo = JsonConvert.DeserializeObject<SeatSelectionViewModel>(seatInfoJson ?? "{}");

            // 🔥 Đọc combo đã chọn
            var comboJson = HttpContext.Session.GetString("ComboData");
            var combo = string.IsNullOrEmpty(comboJson)
                ? new ComboSelectionModel()
                : JsonConvert.DeserializeObject<ComboSelectionModel>(comboJson);

            var vm = new CheckoutViewModel
            {
                MovieTitle = seatInfo.MovieTitle,
                PosterUrl = seatInfo.PosterUrl ?? "/images/default-poster.jpg",
                CinemaName = seatInfo.CinemaName,
                RoomName = seatInfo.RoomName,
                ShowTime = seatInfo.ShowTime,
                Seats = booking.Seats,
                SeatTotal = booking.SeatTotal,
                Foods = combo.Foods?.Select(f => new FoodDto
                {
                    IdFood = f.IdFood,
                    Price = (decimal)f.Price,
                    Quantity = f.Quantity
                }).ToList() ?? new List<FoodDto>(),

                MovieId = seatInfo.MovieId,
                ShowId = seatInfo.ShowId,
                AgeRestriction = seatInfo.AgeRestriction
            };

            return View(vm);
        }




        [HttpPost]
        public IActionResult ConfirmPayment(string VoucherCode, int UsedPoint, string PaymentMethod)
        {
            // Ghi vào session để dùng cho VNPay
            HttpContext.Session.SetString("UsedPoint", UsedPoint.ToString());
            HttpContext.Session.SetString("VoucherCode", VoucherCode ?? "");

            return RedirectToAction("CreatePaymentUrlVnpay", "Payment");
        }
    }
}
