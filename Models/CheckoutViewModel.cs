// File: Models/CheckoutViewModel.cs
using MovieTicketWebsite.Models.Booking;

namespace MovieTicketWebsite.Models
{
    public class CheckoutViewModel
    {
        public string MovieTitle { get; set; }
        public string PosterUrl { get; set; }
        public string CinemaName { get; set; }
        public string RoomName { get; set; }
        public DateTime ShowTime { get; set; }

        public int MovieId { get; set; }
        public int ShowId { get; set; }
        public string AgeRestriction { get; set; }

        public List<SeatDto> Seats { get; set; } = new();
        public decimal SeatTotal { get; set; }

        public List<FoodDto> Foods { get; set; } = new();
        public decimal ComboTotal => Foods.Sum(f => f.Quantity * f.Price);
        public decimal TotalAmount => SeatTotal + ComboTotal;
    }
}
