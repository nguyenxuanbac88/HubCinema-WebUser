// Models/TicketViewModel.cs
using MovieTicketWebsite.Models.Booking;

namespace MovieTicketWebsite.Models
{
    public class TicketViewModel
    {
        public int OrderId { get; set; }

        public string MovieTitle { get; set; }
        public string PosterUrl { get; set; }
        public string CinemaName { get; set; }
        public string RoomName { get; set; }
        public DateTime ShowTime { get; set; }
        public string Seats { get; set; }
        public decimal Price { get; set; }
        public decimal ComboTotal { get; set; }

        public List<FoodDto> Foods { get; set; } = new();
    }

}
