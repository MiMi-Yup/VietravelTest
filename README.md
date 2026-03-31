# Vietravel Tour Management & Booking Request System

## 🌟 Tổng quan
Dự án là hệ thống quản lý danh sách Tour và lập Phiếu đề nghị đặt dịch vụ đi tour theo đoàn, được phát triển theo mô hình Fullstack hiện đại, đáp ứng các tiêu chuẩn Clean Architecture, bảo mật, và khả năng mở rộng.

## 🛠️ Công nghệ sử dụng
- **Backend:** .NET 10 (C#), ASP.NET Core Web API, Entity Framework Core (PostgreSQL)
- **Frontend:** Next.js 16 (React 19), Tailwind CSS 4, TypeScript
- **Database:** PostgreSQL 15
- **Infrastructure:** Docker & Docker Compose
- **Testing:** NUnit (Backend) & Jest + React Testing Library (Frontend)

## ✨ Tính năng chính
- **Quản lý Tour:** 
  - Đầy đủ thao tác CRUD với phân trang (Pagination).
  - Áp dụng Soft-delete (`IsActive`, `IsDeleted`).
  - Kiểm tra tính duy nhất (Unique) của tên Tour (bỏ qua các Tour đã bị xóa mềm).
  - Có đánh index để tối ưu truy vấn tìm kiếm.
- **Quản lý Phiếu Đề Nghị Đặt Dịch Vụ:**
  - Hỗ trợ các phân loại Tour: Khách lẻ (FIT), Khách đoàn (GIT), Hội nghị (MICE).
  - Tự động tính toán giá trị dịch vụ: `LineTotal = Quanity * UnitPrice` và `TotalAmount = Sum(LineTotal)`.
  - Workflow phân loại trạng thái logic:
    - Tổng tiền ≤ 100,000,000 VNĐ: Status = **Đã tiếp nhận**.
    - Tổng tiền > 100,000,000 VNĐ: Status = **Chờ duyệt**.
  - Cảnh báo UI linh hoạt: Hiển thị cảnh báo nếu loại Tour là MICE nhưng số lượng khách < 10.
  - Sau khi phiếu ở trạng thái **Đã duyệt**, áp dụng rule Read-Only (không cho phép sửa/xóa, khóa UI components).
- **Bảo mật & Cấu hình:**
  - Xác thực JWT thông qua Basic Auth Form.
  - Lọc theo IP (IP Whitelist) linh hoạt thông qua Config pattern (`IOptions`).
  - Quản trị cấu hình thông qua `.env` và `appsettings.json` chia môi trường (Develop/Production).

## 🚀 Hướng dẫn chạy dự án

Dự án đã được cấu hình sẵn Docker Compose. Để khởi chạy toàn bộ hệ thống:

1. Sao chép tệp biến môi trường (nếu chưa có):
   ```bash
   cp .env.example .env
   ```
   *(Thêm các thông số cần thiết vào file `.env`)*

2. Khởi chạy thông qua Docker Compose:
   ```bash
   docker compose up --build
   ```

3. Truy cập ứng dụng:
   - **Frontend:** http://localhost:3000
   - **Backend API:** http://localhost:8080

## 🧪 Chạy Test
- **Backend (NUnit):**
  ```bash
  cd Backend
  dotnet test Tests/Tests.csproj --verbosity normal
  ```
- **Frontend (Jest):**
  ```bash
  cd Frontend
  npm run test
  ```
