namespace MovieTicketWebsite.Models
{
    public class SeatViewModel
    {
        public string Row { get; set; }          // A, B, C...
        public int Number { get; set; }          // 1, 2, 3...
        public bool IsSold { get; set; }
        public bool IsVip { get; set; }
        public bool IsCouple { get; set; }
        public bool IsTriple { get; set; }
    }
}
