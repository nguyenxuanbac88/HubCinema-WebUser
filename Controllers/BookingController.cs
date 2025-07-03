using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models;

namespace MovieTicketWebsite.Controllers
{
    public class BookingController : Controller
    {
        [HttpGet]
        public IActionResult SelectSeat(int showId)
        {
            // 🧪 Giả lập data — bạn nên thay bằng API call hoặc DB query
            var model = new SeatSelectionViewModel
            {
                MovieTitle = "Elio Cậu Bé Đến Từ Trái Đất",
                PosterUrl = "/images/elio-movie.jpg",
                Format = "2D Lồng Tiếng",
                CinemaName = "Galaxy Nguyễn Du",
                RoomName = "RAP 5",
                ShowTime = new DateTime(2025, 7, 3, 18, 30, 0),
                ShowId = showId
            };

            // Sinh ghế từ A đến I, số từ 1 đến 9
            char[] rows = "ABCDEFGHI".ToCharArray();
            for (int i = 0; i < rows.Length; i++)
            {
                for (int j = 1; j <= 9; j++)
                {
                    model.Seats.Add(new SeatViewModel
                    {
                        Row = rows[i].ToString(),
                        Number = j,
                        IsSold = (rows[i] == 'G' && (j == 3 || j == 4 || j == 5)), // giả lập ghế đã bán
                        IsVip = (rows[i] == 'D'), // giả lập hàng D là VIP
                        IsCouple = (j == 8 || j == 9), // ghế 8-9 là ghế đôi
                        IsTriple = false
                    });
                }
            }

            return View(model);
        }
    }
}
