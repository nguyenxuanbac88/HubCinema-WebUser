namespace MovieTicketWebsite.Models
{
    public class MovieDetailViewModel
    {
        public int IdMovie { get; set; }
        public string MovieName { get; set; }
        public string Genre { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; }
        public string Director { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string CoverURL { get; set; }
        public string TrailerURL { get; set; }
        public string AgeRestriction { get; set; }
        public string Producer { get; set; }
        public string Actors { get; set; }

        public List<TheaterShowtimes> ShowtimesByTheater { get; set; } // gộp phần lịch chiếu

        // ✅ Thêm các thuộc tính mới
        public List<string> Regions { get; set; } = new();
        public List<CinemaInfo> Cinemas { get; set; } = new();
        public List<DateTime> AvailableDates { get; set; } = new();
    }
}
