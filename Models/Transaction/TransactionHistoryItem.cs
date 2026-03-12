namespace MovieTicketWebsite.Models.Transaction
{
    public class TransactionHistoryItem
    {
        public int OrderId { get; set; }
        public string MovieTitle { get; set; }
        public string PosterUrl { get; set; }
        public string CinemaName { get; set; }
        public string RoomName { get; set; }
        public DateTime ShowTime { get; set; }
        public string Seats { get; set; }
        public int Price { get; set; }
        public int ComboTotal { get; set; }
        public List<FoodDisplayModel> Foods { get; set; } = new();
    }

}
