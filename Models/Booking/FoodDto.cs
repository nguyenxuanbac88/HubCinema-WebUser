using Newtonsoft.Json;

namespace MovieTicketWebsite.Models.Booking
{
    public class FoodDto
    {
        [JsonProperty("idFood")]
        public int IdFood { get; set; }

        [JsonIgnore]
        public string FoodName { get; set; } = string.Empty;

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; } // ⬅ PHẢI là int!
    }

}
