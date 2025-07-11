using System.Text.Json.Serialization;

namespace MovieTicketWebsite.Models
{
    public class ShowtimeItem
    {
        public int Id { get; set; }

        [JsonPropertyName("start_time")] // hoặc "startTime" nếu đúng camelCase
        public DateTime StartTime { get; set; }

        [JsonPropertyName("ngayChieu")]
        public DateTime NgayChieu { get; set; }
    }
}
