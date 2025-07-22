namespace MovieTicketWebsite.Models
{
    public class SeatSelectionViewModel
    {
        public int MovieId { get; set; }
        public string MovieTitle { get; set; }
        public string PosterUrl { get; set; }
        //public string Format { get; set; }
        public string CinemaName { get; set; }
        public string RoomName { get; set; }
        public DateTime ShowTime { get; set; }
        public int ShowId { get; set; }

        public List<SeatViewModel> Seats { get; set; } = new();
        public string AgeRestriction { get; set; }
        public string? Poster { get; set; } // Thêm thuộc tính Poster để chứa ảnh poster của phim
    }
}
/*
 Vai trò:
Dùng để hiển thị thông tin suất chiếu và sơ đồ ghế tại trang Seat/Matrix.cshtml.

Dữ liệu chứa:

Tên phim (MovieTitle), ảnh poster (PosterUrl)

Thông tin rạp, phòng, suất chiếu (CinemaName, RoomName, ShowTime)

ShowId dùng để fetch sơ đồ ghế hoặc gửi khi đặt vé

Seats: danh sách các ghế hiện có trong suất chiếu (render layout)

📌 Model này không gửi lên backend, chỉ để render UI.
 */
