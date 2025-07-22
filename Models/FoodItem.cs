namespace MovieTicketWebsite.Models
{
    public class FoodItem
    {
        public int IdFood { get; set; }
        public string FoodName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
