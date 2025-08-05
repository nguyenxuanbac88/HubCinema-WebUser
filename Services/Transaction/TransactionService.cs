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

    public async Task<TransactionHistoryItem?> GetInvoiceByOrderIdAsync(int orderId)
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("AccessToken");
        if (string.IsNullOrEmpty(token))
            return null;

        var all = await GetTransactionHistoryAsync(token);
        return all.FirstOrDefault(x => x.OrderId == orderId);
    }

}
