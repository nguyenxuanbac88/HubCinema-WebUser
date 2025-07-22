namespace MovieTicketWebsite.Models.Booking
{
    public class FoodDto
    {
        public int IdFood { get; set; }
        public string FoodName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
