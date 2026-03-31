# Quy tắc Nghiệp vụ (Business Rules)

## 1. Ràng buộc dữ liệu (Validation)
- Các trường bắt buộc: Tên tour, ngày khởi hành, loại tour, số lượng khách.
- Mỗi phiếu đề nghị phải có ít nhất 01 dịch vụ đi kèm.
- Số lượng và đơn giá của từng dịch vụ phải lớn hơn 0.
- **Duy nhất (Uniqueness):** Không cho phép tạo Tour trùng tên trong hệ thống.
- **Trạng thái:** Mặc định khi tạo mới Tour là `IsActive = true`.

## 2. Công thức tính toán
- **Tổng tiền dòng (Line Total):** Số lượng x Đơn giá.
- **Tổng chi phí phiếu:** Tổng cộng tất cả các dòng dịch vụ trong phiếu.

## 3. Logic Trạng thái & Cảnh báo
- **Cảnh báo MICE:** Nếu loại tour là "MICE" và số lượng khách < 10, hệ thống phải hiển thị cảnh báo trên giao diện.
- **Phân loại trạng thái phiếu:**
    - Nếu Tổng chi phí > 100,000,000 VNĐ: Trạng thái = "Chờ duyệt quản lý".
    - Nếu Tổng chi phí <= 100,000,000 VNĐ: Trạng thái = "Đã tiếp nhận".
- **Ràng buộc Phê duyệt:** - Khi phiếu ở trạng thái "Đã phê duyệt", hệ thống **KHÔNG** cho phép Cập nhật hoặc Xóa.
    - Sau khi phê duyệt, mọi thay đổi phải thông qua quy trình điều chỉnh (nếu có).

## 4. Cơ chế Xóa mềm (Soft Delete)
- Toàn bộ các bảng (Tours, Requests, RequestDetails) sử dụng các field:
    - `IsActive` (bool): Trạng thái hoạt động.
    - `IsDelete` (bool): Đánh dấu đã xóa.
- Khi thực hiện lệnh DELETE, hệ thống chỉ cập nhật `IsDelete = true`, không xóa vật lý khỏi database.