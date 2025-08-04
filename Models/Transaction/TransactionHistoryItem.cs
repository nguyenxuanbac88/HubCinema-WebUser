using MovieTicketWebsite.Models.Booking;

namespace MovieTicketWebsite.Models.Transaction
{
    public class TransactionHistoryItem
    {
        public string? MovieTitle { get; set; }
        public string? CinemaName { get; set; }
        public string? RoomName { get; set; }
        public DateTime ShowTime { get; set; }
        public string? Seats { get; set; }
        public decimal Price { get; set; }
        public int OrderId { get; set; } // ✅ dùng int luôn, giống từ API

        public List<FoodDto> Foods { get; set; } = new();
        public decimal ComboTotal { get; set; }
        public string? PosterUrl { get; set; }
    }
}
