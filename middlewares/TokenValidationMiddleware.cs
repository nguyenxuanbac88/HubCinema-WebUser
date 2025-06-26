using System.Net;
using System.Net.Http.Headers;

namespace MovieTicketWebsite.middlewares
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpClientFactory _httpClientFactory;

        public TokenValidationMiddleware(RequestDelegate next, IHttpClientFactory httpClientFactory)
        {
            _next = next;
            _httpClientFactory = httpClientFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Session.GetString("AccessToken");

            // Chỉ kiểm tra nếu có token và đang truy cập trang cần đăng nhập
            var path = context.Request.Path.Value?.ToLower();
            var needCheck = !string.IsNullOrEmpty(token) &&
                            !path.StartsWith("/home") && !path.StartsWith("/account/login");

            if (needCheck)
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync("http://api.dvxuanbac.com:2030/Api/User/GetInfo");

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
