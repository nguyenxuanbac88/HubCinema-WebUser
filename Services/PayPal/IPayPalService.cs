namespace MovieTicketWebsite.Services.PayPal
{
    public interface IPayPalService
    {
        Task<CreateOrderResponse> CreateOrderAsync(string value, string currency, string reference, string returnUrl, string cancelUrl);
        Task<CaptureOrderResponse> CaptureOrderAsync(string orderId);
    }
}
