# Tiêu chuẩn Kỹ thuật & Kiến trúc

## 1. Backend (.NET Core)
- **Kiến trúc:** Tuân thủ Clean Architecture
- **Thành phần bắt buộc:** Controller, Service, Repository
- **Cơ chế:** Áp dụng Dependency Injection, .NET Core 10, EF Core Database code first
- **Database:** Sử dụng SQL để tạo bảng và Index tối ưu truy vấn theo `City`
- **Bảo mật:** Triển khai White list IP và Authentication cho API
- **Pagination:** Sử dụng mẫu `PagedList<T>` với `PageNumber` và `PageSize`.
- **Validation:** Sử dụng FluentValidation để check trùng tên Tour và logic khóa dữ liệu sau phê duyệt.
- **Backend:** Đã triển khai một phần cấu trúc tại thư mục `/Backend`.
- **Frontend:** Đã triển khai một phần cấu trúc tại thư mục `/Frontend`.
- **Infrastucture:** Đã có file `docker-compose.yml` và `.env` tại root.
- **Yêu cầu:** Agent cần đọc mã nguồn hiện tại trước khi đề xuất code mới để đảm bảo tính nhất quán.

## 2. Frontend (Next.js)
- **Tính năng UI:**
    - Có trạng thái loading khi đang fetch dữ liệu
    - Có xử lý lỗi (Error handling) khi API thất bại
- **Kỹ thuật:** Trình bày rõ cấu trúc Page, cách fetch dữ liệu và quản lý state
- **State Management:** Xử lý logic khóa Button "Edit/Delete" dựa trên trạng thái `Status` của phiếu.