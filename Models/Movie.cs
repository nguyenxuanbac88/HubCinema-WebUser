namespace MovieTicketWebsite.Models
{
    public class Movie
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

        // Các trường bị thiếu trong model gốc
        public string AgeRestriction { get; set; }
        public string Producer { get; set; }
        public string Actors { get; set; }
        public int Status { get; set; } // 1: đang chiếu, 0: sắp chiếu, 2: hết chiếu
    }

}
