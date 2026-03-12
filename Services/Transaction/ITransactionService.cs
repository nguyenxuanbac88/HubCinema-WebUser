using MovieTicketWebsite.Models.Transaction;

namespace MovieTicketWebsite.Services.Transaction
{
    public interface ITransactionService
    {
        Task<List<TransactionHistoryItem>> GetTransactionHistoryAsync(string token);

        // ➕ Thêm phương thức này để gọi từ controller
        Task<TransactionHistoryItem?> GetInvoiceByOrderIdAsync(int orderId);
    }
}
