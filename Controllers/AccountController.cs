using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace MovieTicketWebsite.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
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
                var response = await client.GetAsync("http://api.dvxuanbac.com:2030/Api/User/GetInfo");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var userInfo = JsonConvert.DeserializeObject<UserInfo>(jsonString);
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

            var requestBody = new
            {
                oldPassword = model.OldPassword,
                newPassword = model.NewPassword
            };

            var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "http://api.dvxuanbac.com:2030/Api/User/ChangePw");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = content;

            try
            {
                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    TempData["ChangePasswordMessage"] = "Đổi mật khẩu thành công.";
                }
                else
                {
                    TempData["ChangePasswordMessage"] = $"Lỗi: {response.StatusCode}";
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
                // ❗ Dùng GET vì API chỉ cho phép GET
                var response = await client.GetAsync("http://api.dvxuanbac.com:2030/Api/User/Logout");
                var responseBody = await response.Content.ReadAsStringAsync();

                // ✅ Xóa session sau khi gửi logout API
                HttpContext.Session.Clear();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody);
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
                HttpContext.Session.Clear(); // Dù lỗi vẫn xóa session
                TempData["LogoutMessage"] = $"Lỗi kết nối API: {ex.Message}";
            }

            return RedirectToAction("Index", "Home");
        }


    }
}
