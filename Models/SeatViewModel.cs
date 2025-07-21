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

/*
 Vai trò:
Là phần tử trong Seats của SeatSelectionViewModel. Mỗi SeatViewModel thể hiện một ghế cụ thể.

Thông tin chứa:

Dòng (Row) và số ghế (Number)

Trạng thái: đã bán, ghế VIP, ghế đôi, ghế ba...

📌 Chỉ dùng để hiển thị, không dùng để đặt vé trực tiếp.
 */