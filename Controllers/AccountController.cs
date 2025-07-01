using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace MovieTicketWebsite.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseApiUrl;

        public AccountController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var token = HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrEmpty(token))
            {
                TempData["LoginMessage"] = "Chưa đăng nhập.";
                return RedirectToAction("Index", "Home");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.GetAsync($"{_baseApiUrl}/user/getinfo");

                if (response.IsSuccessStatusCode)
                {
                    var userInfo = await response.Content.ReadFromJsonAsync<UserInfo>();
                    return View(userInfo);
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    HttpContext.Session.Clear();
                    TempData["LoginMessage"] = "Phiên đăng nhập đã hết hạn.";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["LoginMessage"] = "Lỗi khi lấy thông tin người dùng.";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["LoginMessage"] = "Lỗi kết nối: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var token = HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrEmpty(token))
            {
                TempData["ChangePasswordMessage"] = "Không tìm thấy thông tin đăng nhập.";
                return RedirectToAction("Profile");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var requestBody = new
            {
                oldPassword = model.OldPassword,
                newPassword = model.NewPassword
            };

            try
            {
                var response = await client.PostAsJsonAsync($"{_baseApiUrl}/user/changepw", requestBody);

                var responseBody = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseBody);

                if (response.IsSuccessStatusCode && result.status == 1)
                {
                    TempData["ChangePasswordMessage"] = "Đổi mật khẩu thành công.";
                }
                else
                {
                    TempData["ChangePasswordMessage"] = $" {result.message}";
                }
            }
            catch (Exception ex)
            {
                TempData["ChangePasswordMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction("Profile");
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var token = HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrEmpty(token))
            {
                TempData["LogoutMessage"] = "Bạn chưa đăng nhập.";
                return RedirectToAction("Index", "Home");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.GetAsync($"{_baseApiUrl}/user/logout");

                var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>(); // ✅ dùng ReadFromJsonAsync
                HttpContext.Session.Clear();

                if (response.IsSuccessStatusCode)
                {
                    TempData["LogoutMessage"] = result != null && result.ContainsKey("message")
                        ? result["message"]
                        : "Đăng xuất thành công.";
                }
                else
                {
                    TempData["LogoutMessage"] = $"Lỗi đăng xuất: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                HttpContext.Session.Clear();
                TempData["LogoutMessage"] = $"Lỗi kết nối API: {ex.Message}";
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
