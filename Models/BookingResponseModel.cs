using Newtonsoft.Json;

namespace MovieTicketWebsite.Models
{
    public class BookingResponseModel
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("invoiceId")]
        public int InvoiceId { get; set; }
    }
}
