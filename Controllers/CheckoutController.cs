using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models;
using MovieTicketWebsite.Models.Booking;
using MovieTicketWebsite.Models.Vnpay;
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
            HttpContext.Session.SetString("UsedPoint", UsedPoint.ToString());
            HttpContext.Session.SetString("VoucherCode", VoucherCode ?? "");

            var bookingJson = HttpContext.Session.GetString("BookingData");
            var seatInfoJson = HttpContext.Session.GetString("SeatInfo");

            if (string.IsNullOrEmpty(bookingJson) || string.IsNullOrEmpty(seatInfoJson))
                return RedirectToAction("Index", "Home");

            var booking = JsonConvert.DeserializeObject<BookingRequestModel>(bookingJson);
            var seatInfo = JsonConvert.DeserializeObject<SeatSelectionViewModel>(seatInfoJson);

            // Tổng tiền = ghế + combo
            var comboJson = HttpContext.Session.GetString("ComboData");
            var combo = string.IsNullOrEmpty(comboJson)
                ? new ComboSelectionModel()
                : JsonConvert.DeserializeObject<ComboSelectionModel>(comboJson);

            var comboTotal = combo?.Foods?.Sum(f => f.Price * f.Quantity) ?? 0;
            var totalAmount = booking.SeatTotal + (decimal)comboTotal;

            // Tạo model gửi sang VNPay
            var model = new PaymentInformationModel
            {
                OrderType = "billpayment",
                Amount = (double)totalAmount,
                OrderDescription = $"Thanh toán vé xem phim {seatInfo.MovieTitle}",
                Name = "MovieTicket"
            };

            HttpContext.Session.SetString("VnPayData", JsonConvert.SerializeObject(model));
            return RedirectToAction("RedirectToVNPay", "Payment");


        }

    }
}
