// Models/TicketViewModel.cs
using MovieTicketWebsite.Models.Booking;

namespace MovieTicketWebsite.Models
{
    public class TicketViewModel
    {
        public string MovieTitle { get; set; }
        public string CinemaName { get; set; }
        public string RoomName { get; set; }
        public DateTime ShowTime { get; set; }
        public string Seats { get; set; }
        public decimal Price { get; set; }
        public string OrderId { get; set; }
        public string? PosterUrl { get; set; }

        // 🔥 Thêm 2 dòng dưới đây
        public List<FoodDto> Foods { get; set; } = new();
        public decimal ComboTotal { get; set; }
    }
}
