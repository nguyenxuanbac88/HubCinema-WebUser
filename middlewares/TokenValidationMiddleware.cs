using System.Net;
using System.Net.Http.Headers;

namespace MovieTicketWebsite.middlewares
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseApiUrl;

        public TokenValidationMiddleware(RequestDelegate next, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _next = next;
            _httpClientFactory = httpClientFactory;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Session.GetString("AccessToken");

            // Chỉ kiểm tra nếu có token và đang truy cập trang cần đăng nhập
            var path = context.Request.Path.Value?.ToLower();
            var needCheck = !string.IsNullOrEmpty(token) &&
                            path != "/" &&
                            !path.StartsWith("/Home") &&
                            !path.StartsWith("/Cinema/GetAllCinemas") &&
                            !path.StartsWith("/Cinema/GetCinema") &&
                            !path.StartsWith("/Food");

            if (needCheck)
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync($"{_baseApiUrl}/User/GetInfo");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    context.Session.Clear();
                    context.Response.Redirect("/Home/Index?expired=1");
                    return;
                }
            }

            await _next(context);
        }
    }

}
