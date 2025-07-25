using MovieTicketWebsite.Models;
using MovieTicketWebsite.Models.Booking;
using MovieTicketWebsite.Models.Transaction;
using MovieTicketWebsite.Services.Transaction;

public class TransactionService : ITransactionService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TransactionService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<TransactionHistoryItem>> GetTransactionHistoryAsync(string token)
    {
        // TODO: Gọi API thật ở đây (dùng token nếu cần)
        // Tạm mock dữ liệu
        await Task.Delay(200); // giả lập delay

        return new List<TransactionHistoryItem>
        {
            new TransactionHistoryItem
            {
                MovieTitle = "Doraemon: Nobita và Thế Giới Truyện Tranh",
                CinemaName = "Galaxy Trung Chánh",
                RoomName = "Phòng 1",
                ShowTime = DateTime.Now.AddDays(-2).AddHours(19),
                Seats = "A1, A2",
                Price = 160000,
                OrderId = "INV123456",
                PosterUrl = "https://cdn.galaxycine.vn/media/2024/5/5/doraemon-poster.jpg"
            },
            new TransactionHistoryItem
            {
                MovieTitle = "Ba Mặt Lật Kèo",
                CinemaName = "Galaxy Parc Mall",
                RoomName = "Phòng 7",
                ShowTime = DateTime.Now.AddDays(-4).AddHours(21),
                Seats = "B3, B4",
                Price = 180000,
                OrderId = "INV654321",
                ComboTotal = 50000,
                Foods = new List<FoodDto> {
                    new FoodDto { FoodName = "Combo 2 Big Extra", Quantity = 1, Price = 50000 }
                }
            }
        };
    }

    public async Task<TicketViewModel?> GetInvoiceByOrderIdAsync(string orderId)
    {
        // Giả lập lấy token từ HttpContext để gọi API
        var token = _httpContextAccessor.HttpContext?.Session.GetString("AccessToken");
        if (string.IsNullOrEmpty(token))
            return null;

        var all = await GetTransactionHistoryAsync(token);
        var trans = all.FirstOrDefault(x => x.OrderId == orderId);
        if (trans == null) return null;

        return new TicketViewModel
        {
            MovieTitle = trans.MovieTitle,
            CinemaName = trans.CinemaName,
            RoomName = trans.RoomName,
            ShowTime = trans.ShowTime,
            Seats = trans.Seats,
            Price = trans.Price,
            OrderId = trans.OrderId,
            PosterUrl = trans.PosterUrl,
            ComboTotal = trans.ComboTotal,
            Foods = trans.Foods
        };
    }

}
