using System.Text.Json.Serialization;

namespace MovieTicketWebsite.Models
{
    public class TheaterShowtimes
    {
        [JsonPropertyName("maRap")]
        public int CinemaId { get; set; }

        [JsonPropertyName("tenRap")]
        public string TheaterName { get; set; } = string.Empty;

        [JsonPropertyName("gioChieu")]
        public List<GioChieuDto> Showtimes { get; set; } = new(); 
    }
}
