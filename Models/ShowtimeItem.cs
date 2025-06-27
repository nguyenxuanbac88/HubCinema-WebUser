namespace MovieTicketWebsite.Models
{
    public class ShowtimeItem
    {
        public int Id { get; set; }               // ID của suất chiếu (dùng để đặt vé)
        public DateTime StartTime { get; set; }   // Thời gian chiếu, ví dụ: 2025-06-18 20:00
        public DateTime NgayChieu { get; set; } // thêm dòng này
    }
}
