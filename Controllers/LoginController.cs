using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models;
using Newtonsoft.Json;
using System.Text;

namespace MovieTicketWebsite.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["LoginMessage"] = "Email và mật khẩu không được để trống.";
                return RedirectToAction("Index", "Home");
            }

            var loginModel = new UserLoginModel
            {
                Email = email,
                Password = password
            };

            var client = _httpClientFactory.CreateClient();
            var jsonContent = JsonConvert.SerializeObject(loginModel);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("http://api.dvxuanbac.com:2030/api/auth/login", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    dynamic result = JsonConvert.DeserializeObject(responseBody);
                    string token = result?.token;

                    // Lưu token vào session
                    HttpContext.Session.SetString("AccessToken", token);
                    TempData["LoginMessage"] = "Đăng nhập thành công.";
                }
                else
                {
                    TempData["LoginMessage"] = $"Đăng nhập thất bại: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                TempData["LoginMessage"] = "Lỗi kết nối API: " + ex.Message;
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["ForgotMessage"] = "Vui lòng nhập email.";
                return RedirectToAction("Index", "Home");
            }

            var client = _httpClientFactory.CreateClient();

            var requestBody = new
            {
                username = email
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("http://api.dvxuanbac.com:2030/api/auth/forgot-password", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    dynamic result = JsonConvert.DeserializeObject(responseBody);
                    TempData["ForgotMessage"] = result?.message?.ToString() ?? "Đã gửi yêu cầu khôi phục.";
                }
                else
                {
                    TempData["ForgotMessage"] = $"Lỗi: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                TempData["ForgotMessage"] = "Lỗi kết nối API: " + ex.Message;
            }

            return RedirectToAction("Index", "Home");
        }


    }
}
