using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace MovieTicketWebsite.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RegisterController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Submit(UserRegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["RegisterMessage"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction("Index", "Home");
            }

            var client = _httpClientFactory.CreateClient();
            var jsonContent = JsonConvert.SerializeObject(model, new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd"
            });
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");


            try
            {
                var response = await client.PostAsync("http://api.dvxuanbac.com:2030/api/auth/register", content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    dynamic result = JsonConvert.DeserializeObject(responseString);
                    TempData["RegisterMessage"] = (string)(result.message ?? "Đăng ký thành công.");

                }
                else
                {
                    TempData["RegisterMessage"] = $"Lỗi {response.StatusCode}: {responseString}";
                }
            }
            catch (Exception ex)
            {
                TempData["RegisterMessage"] = "Đã xảy ra lỗi khi kết nối API: " + ex.Message;
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
