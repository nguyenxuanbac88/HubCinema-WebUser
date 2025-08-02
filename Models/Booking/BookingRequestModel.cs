namespace MovieTicketWebsite.Models.Booking
{
    public class BookingRequestModel
    {
        public int IdShowtime { get; set; }

        public List<SeatDto>? Seats { get; set; }

        public List<FoodDto> Foods { get; set; } = new(); // Optional

        public int IdVoucher { get; set; }

        public int UsedPoint { get; set; }
        public decimal SeatTotal { get; set; } // Tổng tiền ghế


    }
}
