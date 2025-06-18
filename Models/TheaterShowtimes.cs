namespace MovieTicketWebsite.Models
{
    public class TheaterShowtimes
    {
        public string TheaterName { get; set; }             // Ví dụ: "Galaxy Trung Chánh"
        public List<ShowtimeItem> Showtimes { get; set; }   // Danh sách các giờ chiếu tại rạp đó
    }
}
