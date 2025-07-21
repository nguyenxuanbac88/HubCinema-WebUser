using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models;
using MovieTicketWebsite.Models.Booking;
using Newtonsoft.Json;

namespace MovieTicketWebsite.Controllers
{
    public class ComboController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseApiUrl;

        public ComboController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }
        public async Task<IActionResult> Index(bool inBookingFlow = false)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetStringAsync($"{_baseApiUrl}/Public/GetFoods");
            var foods = JsonConvert.DeserializeObject<List<FoodViewModel>>(response);

            ViewBag.InBookingFlow = inBookingFlow;

            // Lấy tổng tiền ghế
            var seatTotal = HttpContext.Session.GetString("SeatTotal");
            ViewBag.SeatTotal = string.IsNullOrEmpty(seatTotal) ? 0 : decimal.Parse(seatTotal);

            // Lấy thông tin phim, rạp, phòng, suất chiếu từ Session
            var seatInfoJson = HttpContext.Session.GetString("SeatInfo");
            if (!string.IsNullOrEmpty(seatInfoJson))
            {
                var seatInfo = JsonConvert.DeserializeObject<SeatSelectionViewModel>(seatInfoJson);
                ViewBag.MovieTitle = seatInfo.MovieTitle ?? "";
                ViewBag.CinemaName = seatInfo.CinemaName ?? "";
                ViewBag.RoomName = seatInfo.RoomName ?? "";
                ViewBag.ShowTime = seatInfo.ShowTime;
                ViewBag.PosterUrl = seatInfo.PosterUrl ?? "/images/default-poster.jpg";
                ViewBag.MovieId = seatInfo.MovieId;
                ViewBag.ShowId = seatInfo.ShowId;
            }
            else
            {
                // fallback nếu không có thông tin
                ViewBag.MovieTitle = "";
                ViewBag.CinemaName = "";
                ViewBag.RoomName = "";
                ViewBag.ShowTime = "";
                ViewBag.PosterUrl = "/images/default-poster.jpg";
            }

            // Đọc danh sách ghế đã chọn
            var bookingDataJson = HttpContext.Session.GetString("BookingData");
            if (!string.IsNullOrEmpty(bookingDataJson))
            {
                var booking = JsonConvert.DeserializeObject<BookingRequestModel>(bookingDataJson);
                ViewBag.SelectedSeats = booking.Seats?.Select(s => s.MaGhe).ToList() ?? new List<string>();
            }
            else
            {
                ViewBag.SelectedSeats = new List<string>();
            }


            // 🔁 Đọc lại combo nếu quay lại từ thanh toán
            var comboJson = HttpContext.Session.GetString("ComboData");
            if (!string.IsNullOrEmpty(comboJson))
            {
                var combo = JsonConvert.DeserializeObject<ComboSelectionModel>(comboJson);
                ViewBag.SelectedFoods = combo?.Foods ?? new List<FoodItem>();
            }
            else
            {
                ViewBag.SelectedFoods = new List<FoodItem>();
            }


            return View(foods);
        }



        [HttpPost]
        public IActionResult SaveComboSelection([FromBody] ComboSelectionModel selection)
        {
            HttpContext.Session.SetString("ComboData", JsonConvert.SerializeObject(selection));
            return Ok();
        }
    }
}
