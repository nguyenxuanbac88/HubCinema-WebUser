using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models;
using MovieTicketWebsite.Models.Booking;
using MovieTicketWebsite.Models.Vnpay;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

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
                    FoodName = f.FoodName,
                    Price = (int)f.Price,
                    Quantity = f.Quantity
                }).ToList() ?? new List<FoodDto>(),

                MovieId = seatInfo.MovieId,
                ShowId = seatInfo.ShowId,
                AgeRestriction = seatInfo.AgeRestriction
            };

            return View(vm);
        }




        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(string VoucherCode, int UsedPoint, string PaymentMethod)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("🔔 [ConfirmPaymentAsync] ĐÃ ĐƯỢC GỌI");
            Console.ResetColor();

            // Lưu lại mã giảm giá và điểm sử dụng
            HttpContext.Session.SetString("UsedPoint", UsedPoint.ToString());
            HttpContext.Session.SetString("VoucherCode", VoucherCode ?? "");

            var bookingJson = HttpContext.Session.GetString("BookingData");
            var seatInfoJson = HttpContext.Session.GetString("SeatInfo");

            if (string.IsNullOrEmpty(bookingJson) || string.IsNullOrEmpty(seatInfoJson))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("⛔ Thiếu BookingData hoặc SeatInfo trong session.");
                Console.ResetColor();
                return RedirectToAction("Index", "Home");
            }

            var booking = JsonConvert.DeserializeObject<BookingRequestModel>(bookingJson);
            var seatInfo = JsonConvert.DeserializeObject<SeatSelectionViewModel>(seatInfoJson);

            // Tổng tiền = ghế + combo
            var comboJson = HttpContext.Session.GetString("ComboData");
            var combo = string.IsNullOrEmpty(comboJson)
                ? new ComboSelectionModel()
                : JsonConvert.DeserializeObject<ComboSelectionModel>(comboJson);

            var comboTotal = combo?.Foods?.Sum(f => f.Price * f.Quantity) ?? 0;
            var totalAmount = booking.SeatTotal + (decimal)comboTotal;

            // Chuẩn bị dữ liệu gửi API Booking
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://api.dvxuanbac.com:2030");
                var token = HttpContext.Session.GetString("AccessToken");
                if (!string.IsNullOrEmpty(token))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(token);
                    Console.ResetColor();
                }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                booking.IdVoucher = 0;
                booking.UsedPoint = UsedPoint;

                // ⚠️ Ghép combo và ép kiểu Price
                booking.Foods = combo.Foods?.Select(f => new FoodDto
                {
                    IdFood = f.IdFood,
                    Quantity = f.Quantity,
                    Price = (int)f.Price
                }).ToList() ?? new List<FoodDto>();

                // ⚠️ Ghép ghế và ép Price về int
                booking.Seats = booking.Seats
    .Where(s => !string.IsNullOrEmpty(s.MaGhe))
    .Select(s => new SeatDto
    {
        MaGhe = s.MaGhe.Trim().Trim(','), // 🧼 Trim space + dấu phẩy
        Price = (int)Math.Round((decimal)s.Price) // ✅ CHỐT QUAN TRỌNG
    }).ToList();





                // ✅ Bỏ bọc request (API KHÔNG yêu cầu wrapper)
                var json = JsonConvert.SerializeObject(booking);
                Console.WriteLine("✅ Booking JSON: " + json);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/api/Booking/book", content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[❌ API Booking Error]: {response.StatusCode} - {error}");
                    Console.ResetColor();

                    TempData["SeatSelectionData"] = seatInfoJson;
                    return RedirectToAction("Matrix", "Seat", new { id = booking.IdShowtime });
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<BookingResponseModel>(responseJson); // ✅ deserialize bằng model
                int invoiceId = result.InvoiceId; // ✅ lấy chuẩn theo property

                HttpContext.Session.SetInt32("InvoiceId", invoiceId);
                Console.WriteLine($"[✔] Booking thành công - InvoiceId: {invoiceId}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[❌ Exception]: {ex.Message}");
                Console.ResetColor();

                TempData["SeatSelectionData"] = seatInfoJson;
                return RedirectToAction("Matrix", "Seat", new { id = booking.IdShowtime });
            }

            // Chuẩn bị thông tin VNPay
            var model = new PaymentInformationModel
            {
                OrderType = "billpayment",
                Amount = (double)totalAmount,
                OrderDescription = $"Thanh toán vé xem phim {seatInfo.MovieTitle}",
                Name = "MovieTicket"
            };

            HttpContext.Session.SetString("VnPayData", JsonConvert.SerializeObject(model));
            Console.ForegroundColor = ConsoleColor.Green;


            return RedirectToAction("RedirectToVNPay", "Payment");
        }

    }
}
