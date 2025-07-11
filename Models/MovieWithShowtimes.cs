namespace MovieTicketWebsite.Models
{
    public class MovieWithShowtimes
    {
        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = "/images/doraemon.jpg"; // ảnh mặc định
        public Dictionary<string, List<TheaterShowtimes>> ShowtimesByDate { get; set; } = new();
    }
}
