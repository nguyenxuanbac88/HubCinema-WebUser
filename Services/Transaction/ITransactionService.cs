using MovieTicketWebsite.Models;
using MovieTicketWebsite.Models.Transaction;

namespace MovieTicketWebsite.Services.Transaction
{
    public interface ITransactionService
    {
        Task<List<TransactionHistoryItem>> GetTransactionHistoryAsync(string token);

        // ➕ Thêm phương thức này để gọi từ controller
        Task<TicketViewModel?> GetInvoiceByOrderIdAsync(string orderId);
    }
}
