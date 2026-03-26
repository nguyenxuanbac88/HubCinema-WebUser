using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MovieTicketWebsite.Services.PayPal
{
    public class PayPalService : IPayPalService
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public string BaseUrl => _config["PayPal:Mode"] == "Live"
            ? "https://api-m.paypal.com"
            : "https://api-m.sandbox.paypal.com";

        public PayPalService(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        private async Task<string> AuthenticateAsync()
        {
            var clientId = _config["PayPal:ClientId"];
            var secret = _config["PayPal:Secret"];
            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{secret}"));

            var content = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            };

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseUrl}/v1/oauth2/token"),
                Method = HttpMethod.Post,
                Headers = { { "Authorization", $"Basic {auth}" } },
                Content = new FormUrlEncodedContent(content)
            };

            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(request);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            var authResponse = JsonSerializer.Deserialize<AuthResponse>(jsonResponse);
            return authResponse?.access_token;
        }

        public async Task<CreateOrderResponse> CreateOrderAsync(string value, string currency, string reference, string returnUrl, string cancelUrl)
        {
            var token = await AuthenticateAsync();

            var requestObj = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new {
                        reference_id = reference,
                        amount = new { currency_code = currency, value = value }
                    }
                },
                application_context = new
                {
                    return_url = returnUrl,
                    cancel_url = cancelUrl,
                    user_action = "PAY_NOW"
                }
            };

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token}");

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestObj), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{BaseUrl}/v2/checkout/orders", jsonContent);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<CreateOrderResponse>(jsonResponse);
        }

        public async Task<CaptureOrderResponse> CaptureOrderAsync(string orderId)
        {
            var token = await AuthenticateAsync();
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token}");

            var content = new StringContent("", Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{BaseUrl}/v2/checkout/orders/{orderId}/capture", content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<CaptureOrderResponse>(jsonResponse);
        }
    }

    // Models
    public class AuthResponse { public string access_token { get; set; } }
    public class CreateOrderResponse { public string id { get; set; } public string status { get; set; } public List<Link> links { get; set; } }
    public class CaptureOrderResponse { public string id { get; set; } public string status { get; set; } }
    public class Link { public string href { get; set; } public string rel { get; set; } public string method { get; set; } }
}
