namespace MovieTicketWebsite.Models
{
    public class SeatSelectionViewModel
    {
        public string MovieTitle { get; set; }
        public string PosterUrl { get; set; }
        //public string Format { get; set; }
        public string CinemaName { get; set; }
        public string RoomName { get; set; }
        public DateTime ShowTime { get; set; }
        public int ShowId { get; set; }

        public List<SeatViewModel> Seats { get; set; } = new();
    }
}
