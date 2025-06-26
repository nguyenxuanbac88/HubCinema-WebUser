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
                    // Lưu email vào session để sử dụng sau này
                    HttpContext.Session.SetString("UserEmail", email); // ❗ cần cho chức năng đổi mật khẩu
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

                    // 👇 THÊM DÒNG NÀY để modal xác nhận hiển thị sau khi gửi email
                    TempData["OpenConfirmModal"] = true;
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

        [HttpPost]
        public IActionResult CheckOTP(string otp)
        {
            // Giả sử bạn đã lưu email và otp tạm thời bằng TempData (hoặc Session nếu muốn bảo mật hơn)
            var expectedOtp = TempData["ResetOTP"]?.ToString();
            var email = TempData["ResetEmail"]?.ToString();

            if (string.IsNullOrEmpty(otp) || string.IsNullOrEmpty(expectedOtp) || otp != expectedOtp)
            {
                return Json(new { success = false, message = "Mã OTP không đúng hoặc đã hết hạn." });
            }

            // ✅ Ghi lại thông tin để chuyển sang bước xác nhận mật khẩu
            TempData["VerifiedEmail"] = email;
            TempData["VerifiedOTP"] = otp;

            return Json(new { success = true });
        }


        [HttpPost]
        public async Task<IActionResult> ConfirmPassword(string email, string newPassword, string otp)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(otp))
            {
                TempData["ConfirmMessage"] = "Vui lòng nhập đầy đủ Email, mật khẩu mới và mã OTP.";
                return RedirectToAction("Index", "Home");
            }

            var requestBody = new
            {
                username = email,
                newPW = newPassword,
                otp = otp
            };

            var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("http://api.dvxuanbac.com:2030/api/auth/confirm-password", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // ✅ Deserialize an toàn sang Dictionary
                    var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody);

                    TempData["ConfirmMessage"] = result != null && result.ContainsKey("message")
                        ? result["message"]
                        : "Đổi mật khẩu thành công.";

                }
                else
                {
                    TempData["ConfirmMessage"] = $"Lỗi: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                TempData["ConfirmMessage"] = "Lỗi kết nối API: " + ex.Message;
            }

            return RedirectToAction("Index", "Home");
        }



    }
}
