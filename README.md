# HubCinema-WebUser

## 1) Tóm tắt dự án

**HubCinema-WebUser** là phần **front-end web** cho hệ thống đặt vé phim HubCinema, xây dựng bằng **ASP.NET Core MVC (.NET 8)** với Razor Views.  
Mục tiêu của project là cung cấp trải nghiệm người dùng đầy đủ từ:

- xem phim, lịch chiếu, rạp
- chọn ghế, chọn combo
- thanh toán online (VNPay)
- xem vé, quản lý tài khoản và lịch sử giao dịch

Phần front-end này chủ yếu tiêu thụ API từ backend:

- **HubCinema-API:** https://github.com/nguyenxuanbac88/HubCinema-API

---

## CV Version (mẫu ghi ngắn gọn)

**Front-end Developer** | **HubCinema - Movie Ticket Booking Website** *(Github)*  
Xây dựng front-end web đặt vé phim cho người dùng cuối, tích hợp dữ liệu và nghiệp vụ từ HubCinema-API.

- Phát triển các luồng chính: xem phim/lịch chiếu, chọn ghế, chọn combo, checkout và thanh toán VNPay.
- Tích hợp API backend cho xác thực người dùng (JWT), lịch chiếu, đặt vé, hóa đơn/vé và tin tức/banner.
- Xây dựng giao diện responsive bằng Razor + Bootstrap, tối ưu trải nghiệm cho desktop/mobile.
- Triển khai đa ngôn ngữ Việt/Anh (Localization), quản lý trạng thái bằng Session và middleware kiểm tra token.
- **Công nghệ:** ASP.NET Core MVC (.NET 8), Razor Views, Bootstrap 5, JavaScript, jQuery, Slick Carousel, Newtonsoft.Json, VNPay, QRCoder.

---

## 2) Những gì bạn đã làm ở phần front-end (theo code hiện có)

Dựa trên cấu trúc và luồng trong repository này, phạm vi bạn đã triển khai gồm:

### Luồng người dùng chính

- Trang chủ: hiển thị phim, banner, tin tức, combo.
- Chi tiết phim + lịch chiếu theo ngày/rạp/khu vực.
- Chọn suất chiếu → vào trang ma trận ghế.
- Lưu dữ liệu đặt vé trong Session (ghế, combo, thông tin vé tạm).
- Checkout và tạo thanh toán VNPay.
- Nhận callback thanh toán, cập nhật trạng thái ghế, hiển thị vé.

### Tài khoản người dùng

- Đăng ký, đăng nhập, đăng xuất.
- Quên mật khẩu bằng OTP.
- Quản lý profile, đổi mật khẩu, đổi email.
- Xem lịch sử giao dịch.

### Nội dung và tiện ích

- Trang rạp (danh sách rạp, chi tiết rạp).
- Tin tức/khuyến mãi/review phim.
- Đa ngôn ngữ Việt/Anh bằng localization.
- Giao diện responsive cho desktop/mobile.

---

## 3) Công nghệ sử dụng cho front-end HubCinema-WebUser

### Nền tảng & kiến trúc

- **ASP.NET Core MVC (.NET 8)**
- **Razor Views** (server-rendered UI)
- **IHttpClientFactory** để gọi backend API
- **Session** để giữ trạng thái đặt vé và token
- **Middleware** kiểm tra token

### UI/UX

- **Bootstrap 5.3.2**
- **Bootstrap Icons**
- **Slick Carousel**
- CSS/JS custom theo từng module (movie, cinema, seat, account, ...)

### Dữ liệu & tích hợp

- **Newtonsoft.Json** để parse/serialize JSON
- **QRCoder** để hỗ trợ mã QR cho vé
- **VNPay integration** qua service riêng

### Đa ngôn ngữ

- Localization với resource `.resx` cho `vi` và `en`
- Chuyển ngôn ngữ qua cookie culture

---

## 4) Cách front-end này gọi HubCinema-API

Front-end đọc `ApiSettings:BaseUrl` trong `appsettings.json` và gọi các nhóm API chính:

- **Auth/User:** login, register, forgot-password, check-otp, confirm-password, getinfo, changepw, changeemail...
- **Public data:** phim, combo/food, rạp.
- **Schedule/Booking:** lịch chiếu, dữ liệu filter, cập nhật trạng thái ghế.
- **Invoice/Ticket:** lấy vé/hóa đơn, lịch sử giao dịch.
- **News/Banner:** tin tức, banner trang chủ.

Các controller như `HomeController`, `MovieController`, `BookingController`, `SeatController`, `CheckoutController`, `PaymentController`, `TicketController`, `AccountController` đều đang bám theo cách gọi API backend này.

---

## 5) Chạy project

```bash
dotnet restore
dotnet build MovieTicketWebsite.sln
dotnet run
```

Có thể deploy qua Docker/GitHub Actions:

- `Dockerfile`: build image chạy ứng dụng ASP.NET Core.
- `docker-compose.yml`: chạy nhanh theo mô hình container.
- `.github/workflows/deploy.yml`: pipeline CI/CD deploy tự động.
- `DOCKER_DEPLOYMENT.md`: hướng dẫn triển khai Docker chi tiết.
