using MovieTicketWebsite.Models;
using MovieTicketWebsite.Models.Transaction;
using MovieTicketWebsite.Services.Transaction;
using Newtonsoft.Json;
using System.Net.Http.Headers;

public class TransactionService : ITransactionService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TransactionService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;

    }

    public async Task<List<TransactionHistoryItem>> GetTransactionHistoryAsync(string token)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var apiUrl = _configuration["ApiSettings:BaseUrl"] + "/Invoice/by-user";

        try
        {
            var response = await client.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode) return new();

            var responseBody = await response.Content.ReadAsStringAsync();
            var items = JsonConvert.DeserializeObject<List<TransactionHistoryItem>>(responseBody);

            return items ?? new();
        }
        catch
        {
            return new(); // fallback khi lỗi kết nối
        }
    }

    public async Task<TicketViewModel?> GetInvoiceByOrderIdAsync(int orderId)
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
