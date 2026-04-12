# HubCinema WebUser (Customer)

Frontend dành cho khách hàng của hệ thống bán vé xem phim HubCinema, phát triển bằng **ASP.NET Core MVC (.NET 8)**.  
Ứng dụng tập trung vào trải nghiệm người dùng cuối: khám phá phim, chọn suất chiếu, đặt ghế, chọn combo và thanh toán trực tuyến.

## Tổng quan

HubCinema WebUser là phần **customer-facing** trong hệ sinh thái HubCinema.  
Toàn bộ dữ liệu nghiệp vụ (phim, rạp, suất chiếu, combo, đơn hàng, tài khoản, ...) được lấy thông qua API backend do team xây dựng.

## Tính năng nổi bật

- Đăng ký, đăng nhập, quên mật khẩu
- Trang chủ hiển thị nội dung phim và thông tin nổi bật
- Trang chi tiết phim
- Luồng đặt vé đầy đủ:
  - Lấy sơ đồ ghế theo suất chiếu
  - Chọn ghế
  - Chọn combo
  - Thanh toán
- Tích hợp 2 cổng thanh toán:
  - **PayPal**
  - **VNPay**
- Song ngữ **Tiếng Việt / English**
- Trang thành viên và lịch sử giao dịch
- Hiển thị vé sau thanh toán và cho phép xem lại vé đã đặt

## Kiến trúc tích hợp

- WebUser đóng vai trò frontend MVC cho người dùng cuối.
- Backend API chịu trách nhiệm xử lý nghiệp vụ và cung cấp dữ liệu.
- WebUser gọi API để:
  - Lấy danh sách phim, rạp, suất chiếu, combo
  - Xử lý thông tin người dùng
  - Đồng bộ dữ liệu đơn hàng/thanh toán
- Sau khi thanh toán thành công, hệ thống trả kết quả để hiển thị vé và lưu lịch sử giao dịch.

## Công nghệ sử dụng

- **.NET 8 / ASP.NET Core MVC**
- **Razor Views**
- **Newtonsoft.Json**
- **QRCoder**
- Session & Middleware trong ASP.NET Core
- Localization (vi/en)

## Cấu trúc thư mục chính

- `Controllers/`: xử lý luồng request/response cho các màn hình
- `Models/`: model dữ liệu và DTO sử dụng trong WebUser
- `Services/`: tích hợp API, thanh toán PayPal/VNPay, transaction
- `Views/`: giao diện Razor theo từng module
- `wwwroot/`: tài nguyên tĩnh (CSS, JS, images)
- `middlewares/`: middleware dùng trong pipeline

## Thành viên & phạm vi đóng góp

Dự án được phát triển theo hướng phối hợp nhóm:

- API backend và phần admin được xây dựng bởi team backend/admin.
- Phần customer WebUser được thực hiện bởi các thành viên trong nhóm với các module chính như:
  - xác thực người dùng
  - trang chủ, chi tiết phim
  - quy trình đặt vé và thanh toán
  - song ngữ
  - trang thành viên và lịch sử giao dịch

Mục tiêu là đảm bảo trải nghiệm đặt vé liền mạch từ lúc chọn phim đến khi nhận vé sau thanh toán.

## Yêu cầu môi trường

- .NET SDK 8.0+

## Chạy dự án local

```bash
dotnet restore
dotnet build
dotnet run --project MovieTicketWebsite.csproj
```

Sau khi chạy, truy cập địa chỉ được hiển thị trên terminal (mặc định thường là `https://localhost:<port>`).

## Cấu hình

- Cập nhật các giá trị cấu hình trong:
  - `appsettings.json`
  - `appsettings.Development.json`
- Thiết lập endpoint API backend và thông tin cấu hình thanh toán (PayPal/VNPay) theo môi trường chạy.

## Ghi chú

- Đây là repository cho **WebUser (Customer)**.
- Backend API và Admin được triển khai ở repository/service khác trong hệ thống HubCinema.
