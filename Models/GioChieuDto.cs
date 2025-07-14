using System.Text.Json.Serialization;

namespace MovieTicketWebsite.Models
{
    public class GioChieuDto
    {
        [JsonPropertyName("gioChieu")]
        public string StartTime { get; set; } = string.Empty;

        [JsonPropertyName("suatChieu")]
        public int MaSuat { get; set; }
    }
}
