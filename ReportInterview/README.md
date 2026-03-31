# Báo Cáo Phỏng Vấn (Interview Report)

---

## 5. Phân tích BA mini

### Bạn hiểu bài toán này như thế nào?
Bài toán tập trung xây dựng hệ thống đặt vé đi tour theo đoàn, phục vụ việc quản lý danh mục Tour và tiếp nhận các Phiếu đề nghị đặt dịch vụ. Hệ thống hỗ trợ đa dạng đối tượng, với 3 loại hình tổ chức chính:
- **FIT (Khách lẻ):** Khách du lịch tự do, đi lẻ tẻ.
- **GIT (Khách đoàn):** Khách đi theo đoàn.
- **MICE (Du lịch kết hợp hội nghị, sự kiện):** Tổ chức các tour có quy mô kèm hội thảo.

Việc phân quyền mức độ tác động cũng được phân nhỏ qua luồng xét duyệt (ngưỡng 100 triệu VNĐ) và các cảnh báo UX/UI cho nhân viên (vd: MICE < 10 khách).

### Các giả định nghiệp vụ đang dùng
- **Trạng thái Dữ liệu:** Các record quan trọng (Tour, Request) không thực sự bị xóa đi (Hard Delete) mà áp dụng cơ chế **Logic-Delete (Soft-Delete)** thông qua cờ `IsDeleted` và `IsActive`.
- **Hiệu suất & Truy xuất:** Đối với database, cần **đánh Index** cho các cột thường xuyên query như `TourName`, `IsDeleted` để tối ưu câu query. Phải áp dụng **Pagination (Phân trang)** cho toàn bộ danh sách để đảm bảo UI không bị quá tải nêú dữ liệu lớn.
- **Toàn vẹn Dữ liệu:** Khi một phiếu yêu cầu đổi sang trạng thái **Đã duyệt (Approved)**, nó sẽ trở thành **Read-only**. Không ai được phép thay đổi nội dung hay xóa phiếu đó nữa, UI các nút thao tác tương ứng sẽ bị disabled.

### Các câu hỏi cần hỏi thêm nếu triển khai thực tế
Để đưa dự án từ POC (Proof of Concept) / Test lên môi trường Production thực tế, có một số vấn đề cần đặt ra:
1. **Security & WAF:** Hiện tại IP Whitelist và Auth đang nằm ở tầng source code Backend. **Thực tế:** Việc block IP nên đẩy ra tầng WAF (Web Application Firewall) hoặc Security Group trên Cloud để giảm tải cho app. Phần Auth cũng nên tách ra sử dụng Identity Server (IDS) riêng hỗ trợ SSO hoặc tích hợp OAuth2 với các nền tảng nội bộ.
2. **QPS (Queries Per Second) & Rate Limit:** Hệ thống dự kiến chịu tải QPS là bao nhiêu? Việc này liên quan đến quyết định có cần cấu hình hệ thống Rate Limiting ở API Gateway không để chống bị SPAM/DDoS.
3. **Concurrency & Microservices:** 
   - Nếu chuyển đổi sang mô hình Microservices, có cần áp dụng **Distributed Lock** (vd dùng Redis/Redlock) để tránh race-conditions khi thao tác duyệt song song không? 
   - Khi QPS lớn, database dễ xảy ra conflict lúc update. Có nên bổ sung ngay từ đầu cột `ConcurrentTimestamp` hoặc `RowVersion`, đồng thời có thể cần viết **Raw SQL** thay vì dùng thuần EF Core cho một số transaction nóng không?
4. **Performance Optimization (CQRS & Caching):**
   - Hiện tại đang dùng EF Core thuần. Nếu nghiệp vụ mở rộng, các câu query report phức tạp hơn, có nên tách luồng Đọc - Ghi theo chuẩn **CQRS** và sử dụng thêm **Dapper** để tối ưu tốc độ View không?
   - Cần bổ sung các **External Services** nào không? (Ví dụ: Dùng **Redis** để cache các danh sách Tour hay được truy cập để cải thiện Read time. Dùng **Elasticsearch (ELK)** để tập trung tracking log, theo dõi bottleneck hệ thống).

---

## 6. AI Usage

### Bạn đã dùng AI cho phần nào?
- Tạo cấu trúc khung thư mục **Clean Architecture** và các file Boilerplate ban đầu cho .NET.
- Hỗ trợ xây dựng giao diện và các reusable components của Next.js (Tailwind CSS, bảng phân trang, layout).
- Xây dựng sơ bộ Dockerfile đa bước (multi-stage) để cache layer và tối ưu build time.
- Tạo nhanh các khối Unit Test cho logic của Tour và BookingRequest (sinh các case cover happy/sad path).

### Prompt chính bạn đã dùng
*(Các request đã đưa cho AI)*
- *"Đọc toàn bộ source code và mô tả các thông tin nghiệp vụ/spec trong các file `.claude/*.md`, sau đó lập kế hoạch triển khai đầy đủ cả Backend + Frontend."*
- *"Sửa lại phần IP Whitelist thay vì hard-code thì chuyển sang regex, cấu hình ở `appsettings.json`."*
- *"Tạo Unit Test (NUnit) cho Backend và Test Frontend cho Next.js, cấu hình Docker Multi-stage để yêu cầu vượt qua Test khi build image."*

### Bạn đã chỉnh lại gì từ output AI?
- **Workflow & DI:** Sửa lại các liên kết phụ thuộc `IOptions`, đưa Configuration class về layer `Application` thay vì `WebAPI` để tránh Circular Dependency. Móc nối các config vào DI container.
- **Docker Cache:** Sửa lại trật tự lệnh COPY trong Dockerfile của Frontend để Docker hiểu trình tự Build phụ thuộc vào Test stage. Đổi lại đường dẫn module resolver của thẻ config `jest.config.ts`.
- Tối ưu hóa lại các rules validation FluentValidation và xử lý Exceptions theo format hệ thống mong muốn.

---

## 7. Mục tiêu đánh giá

Theo thiết kế hệ thống và bộ source code, các tiêu chí đánh giá được đáp ứng như sau:

- **Khả năng phân tích yêu cầu:** Nắm được bức tranh tổng thể từ Entities (Tour, BookingRequest, Detail), các tham số liên quan (FIT, GIT, MICE), hiểu rõ ngữ cảnh sử dụng và hành vi kỳ vọng của hệ thống.
- **Khả năng bóc tách business rule:** Bóc tách tách bạch các rule như tính toán Validation logic, tổng giá tiền (`LineTotal`), logic cấm Xóa/Sửa khi qua trạng thái Đã Duyệt, ràng buộc lượng khách tham gia hội nghị MICE (< 10 khách cảnh báo).
- **Thiết kế solution hợp lý:** 
  - Đóng gói trong **Clean Architecture** tách biệt Core (Domain/Application) khỏi detail (WebAPI/Infrastructure).
  - Config Management tách bạch (IOptions), xử lý tốt các biến môi trường cho Docker Compose.
  - Testable design: Các Services độc lập, sử dụng DI để dễ dàng viết test với Moq.
- **Kỹ năng code thực tế:** Mã nguồn tuân thủ coding conventions C# / TypeScript, có cơ chế handle Error Global, tái sử dụng các components (Navbar, PagedList, Loading), tổ chức Project theo đúng chuẩn production-ready.
- **Khả năng sử dụng AI hiệu quả:** BIết cách giao tiếp yêu cầu ngữ cảnh lớn, sử dụng AI làm trợ lý tăng tốc xây dựng core base, sau đó tập trung sửa đổi/review các architecture mismatch, xử lý triệt để bug build pipelines và Docker staging.
