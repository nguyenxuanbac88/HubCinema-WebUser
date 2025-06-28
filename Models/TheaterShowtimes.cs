namespace MovieTicketWebsite.Models
{
    public class TheaterShowtimes
    {
        public string TheaterName { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty; // ✅ Để lọc theo vùng
        public int CinemaId { get; set; } // ✅ Để lọc theo mã rạp nếu cần
        public List<ShowtimeItem> Showtimes { get; set; } = new();
    }
}
