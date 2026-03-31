# Phân tích Yêu cầu Hệ thống (System Requirements)

## 1. Tính năng chính
- **Quản lý Tour (Module 1):** Hiển thị danh sách tour đơn giản
- **Quản lý Phiếu đề nghị (Module 2):** Xây dựng hệ thống quản lý Phiếu đề nghị đặt dịch vụ tour
    - Tạo phiếu mới với thông tin chung và danh sách dịch vụ đi kèm
    - Hiển thị danh sách các phiếu đã tạo
    - Xem chi tiết từng phiếu

## 2. Các trường dữ liệu (Data Fields)
- **Tours:** Id, Name, Price, City, CreatedAt
- **Phiếu đề nghị (Booking Request):**
    - Thông tin chung: Tên tour, Ngày khởi hành, Người phụ trách, Loại tour (FIT/GIT/MICE), Số lượng khách
    - Danh sách dịch vụ: Loại dịch vụ, Tên dịch vụ, Nhà cung cấp, Số lượng, Đơn giá, Ghi chú

## 3. Module Quản lý Tour
Triển khai CRUD đầy đủ với các nghiệp vụ:
- **GET /api/tours**: Lấy danh sách tour có phân trang (Pagination).
- **GET /api/tours/{id}**: Lấy chi tiết một tour.
- **POST /api/tours**: Tạo mới tour (Kiểm tra trùng tên - Unique Name).
- **PUT /api/tours/{id}**: Cập nhật thông tin tour.
- **PATCH /api/tours/{id}/status**: Cập nhật trạng thái tour.
- **DELETE /api/tours/{id}**: Xóa tour (Sử dụng Logic Delete).

## 4. Module Phiếu đề nghị đặt dịch vụ
- **GET /api/requests**: Lấy danh sách phiếu có phân trang.
- **POST /api/requests**: Tạo phiếu mới (Validation nghiêm ngặt).
- **PUT /api/requests/{id}**: Cập nhật nội dung (Chỉ khi chưa phê duyệt).
- **POST /api/requests/{id}/approve**: Phê duyệt phiếu.
- **DELETE /api/requests/{id}**: Xóa phiếu (Chỉ khi chưa phê duyệt).