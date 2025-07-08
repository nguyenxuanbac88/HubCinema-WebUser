using Microsoft.AspNetCore.Mvc;
using MovieTicketWebsite.Models;
using Newtonsoft.Json;
using System.Text;

namespace MovieTicketWebsite.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseApiUrl;

        public LoginController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"];
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
            //chuyển đối tượng loginModel (chứa email và password) thành một chuỗi JSON.
            var jsonContent = JsonConvert.SerializeObject(loginModel);
            // tạo một đối tượng StringContent chứa dữ liệu JSON (biến jsonContent) với mã hóa UTF-8 và đặt header
            // Content-Type là "application/json". Đối tượng content này sẽ được dùng làm nội dung (body) của HTTP POST
            // request khi gửi thông tin đăng nhập đến API.
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync($"{_baseApiUrl}/auth/login", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    //sử dụng thư viện Newtonsoft.Json để chuyển đổi (deserialize) chuỗi JSON (responseBody) nhận được từ API thành một đối tượng động (dynamic).
                    dynamic result = JsonConvert.DeserializeObject(responseBody);
                    //Nếu result khác null, nó sẽ lấy thuộc tính token từ đối tượng result. Nếu result là null, kết quả sẽ là null mà không gây lỗi.
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
                var response = await client.PostAsync($"{_baseApiUrl}/auth/forgot-password", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    dynamic result = JsonConvert.DeserializeObject(responseBody);

                    TempData["ForgotMessage"] = result?.message?.ToString() ?? "Đã gửi yêu cầu khôi phục.";
                    HttpContext.Session.SetString("ResetEmail", email); // Trong ForgotPassword

                    string otpToken = result?.otpToken?.ToString(); // Ép kiểu tường minh
                    HttpContext.Session.SetString("OtpToken", otpToken);

                    TempData["CloseForgotModal"] = true;
                    TempData["OpenOtpModal"] = true;
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
        public async Task<IActionResult> CheckOTP(string otp)
        {
            if (string.IsNullOrEmpty(otp))
            {
                TempData["OTPResult"] = "Vui lòng nhập mã OTP.";
                TempData["OpenOtpModal"] = true;
                return RedirectToAction("Index", "Home");
            }

            var email = HttpContext.Session.GetString("ResetEmail"); // Trong CheckOTP
            var otpToken = HttpContext.Session.GetString("OtpToken");

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(otpToken))
            {
                TempData["OTPResult"] = "Thiếu thông tin email hoặc OTP token.";
                TempData["OpenOtpModal"] = true;
                return RedirectToAction("Index", "Home");
            }

            var client = _httpClientFactory.CreateClient();

            var requestBody = new
            {
                username = email,
                otp = otp,
                otpToken = otpToken
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync($"{_baseApiUrl}/auth/check-otp", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    dynamic result = JsonConvert.DeserializeObject(responseBody);
                    string message = result?.message?.ToString();

                    HttpContext.Session.SetString("VerifiedEmail", email);
                    HttpContext.Session.SetString("VerifiedOTP", otp);

                    TempData["OTPResult"] = message ?? "Mã OTP hợp lệ.";
                    TempData["OpenConfirmModal"] = true; // ✅ trigger mở ConfirmPasswordModal

                }
                else
                {
                    TempData["OTPResult"] = "Mã OTP không hợp lệ.";
                    TempData["OpenOtpModal"] = true;
                }
            }
            catch (Exception ex)
            {
                TempData["OTPResult"] = "Lỗi kết nối: " + ex.Message;
                TempData["OpenOtpModal"] = true;
            }

            return RedirectToAction("Index", "Home");
        }




        [HttpPost]
        public async Task<IActionResult> ConfirmPassword(string email, string newPassword, string otp)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(otp))
            {
                TempData["ConfirmMessage"] = "Vui lòng nhập đầy đủ Email, mật khẩu mới và mã OTP.";
                return RedirectToAction("Index", "Home");
            }

            var otpToken = HttpContext.Session.GetString("OtpToken");

            if (string.IsNullOrEmpty(otpToken))
            {
                TempData["ConfirmMessage"] = "Mã OTP token không tồn tại hoặc đã hết hạn.";
                return RedirectToAction("Index", "Home");
            }

            var requestBody = new
            {
                username = email,
                newPW = newPassword,
                otp = otp,
                otpToken = otpToken // ✅ bổ sung đúng theo API
            };

            var client = _httpClientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync($"{_baseApiUrl}/auth/confirm-password", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody);

                    TempData["ConfirmMessage"] = result != null && result.ContainsKey("message")
                        ? result["message"]
                        : "Đổi mật khẩu thành công.";

                    // ✅ Dọn session sau khi thành công
                    HttpContext.Session.Remove("OtpToken");
                    HttpContext.Session.Remove("VerifiedEmail");
                    HttpContext.Session.Remove("VerifiedOTP");
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
