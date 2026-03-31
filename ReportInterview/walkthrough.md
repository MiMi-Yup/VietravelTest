# Walkthrough — Vietravel Tour Management Full Implementation

## Tổng quan

Triển khai hoàn chỉnh 2 module **Quản lý Tour** và **Phiếu đề nghị đặt dịch vụ** theo spec trong `.claude/`:

- **Backend:** .NET 10, Clean Architecture (Domain → Application → Infrastructure → WebAPI)
- **Frontend:** Next.js 16, React 19, TailwindCSS 4
- **Database:** PostgreSQL 15 (qua Docker)
- **Auth:** JWT Bearer + IP Whitelist

---

## Thay đổi đã thực hiện

### Bug Fixes
| File | Thay đổi |
|------|----------|
| [ApplicationExtension.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/WebAPI/Extensions/ApplicationExtension.cs) | Fix `UseAuthentication()` gọi 2 lần → `UseAuthentication()` + `UseAuthorization()` |
| [ExceptionHandlingMiddleware.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/WebAPI/Middlewares/ExceptionHandlingMiddleware.cs) | Xử lý riêng `KeyNotFoundException` (404) và `InvalidOperationException` (400), xóa debug console logging |

---

### Backend — Files mới (19 files)

| Layer | File | Mô tả |
|-------|------|-------|
| Domain | [TourType.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/Domain/Enums/TourType.cs) | Enum: FIT, GIT, MICE |
| Domain | [RequestStatus.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/Domain/Enums/RequestStatus.cs) | Enum: Received, PendingApproval, Approved |
| Domain | [BookingRequest.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/Domain/Entities/BookingRequest.cs) | Entity phiếu đề nghị |
| Domain | [RequestDetail.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/Domain/Entities/RequestDetail.cs) | Entity chi tiết dịch vụ |
| Application | [PagedList.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/Application/Common/PagedList.cs) | Generic pagination model |
| Application | [IBookingRequestRepository.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/Application/Interfaces/IBookingRequestRepository.cs) | Repository interface |
| Application | [IBookingRequestService.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/Application/Interfaces/IBookingRequestService.cs) | Service interface |
| Application | [BookingRequestService.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/Application/Services/BookingRequestService.cs) | Business logic |
| Infrastructure | [BookingRequestRepository.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/Infrastructure/Repositories/BookingRequestRepository.cs) | EF Core implementation |
| WebAPI | [CreateTourRequest.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/WebAPI/DTOs/Requests/CreateTourRequest.cs) | DTO |
| WebAPI | [UpdateTourRequest.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/WebAPI/DTOs/Requests/UpdateTourRequest.cs) | DTO |
| WebAPI | [UpdateTourStatusRequest.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/WebAPI/DTOs/Requests/UpdateTourStatusRequest.cs) | DTO |
| WebAPI | [CreateBookingRequestDto.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/WebAPI/DTOs/Requests/CreateBookingRequestDto.cs) | DTO |
| WebAPI | [UpdateBookingRequestDto.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/WebAPI/DTOs/Requests/UpdateBookingRequestDto.cs) | DTO |
| WebAPI | [TourResponse.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/WebAPI/DTOs/Responses/TourResponse.cs) | DTO |
| WebAPI | [BookingRequestResponse.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/WebAPI/DTOs/Responses/BookingRequestResponse.cs) | DTO |
| WebAPI | [PagedResponse.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/WebAPI/DTOs/Responses/PagedResponse.cs) | DTO |
| WebAPI | [TourValidators.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/WebAPI/Validators/TourValidators.cs) | FluentValidation |
| WebAPI | [BookingRequestValidators.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/WebAPI/Validators/BookingRequestValidators.cs) | FluentValidation |
| WebAPI | [RequestsController.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/WebAPI/Controllers/RequestsController.cs) | Full CRUD + Approve |

### Backend — Files đã sửa (7 files)

| File | Thay đổi |
|------|----------|
| [ITourRepository.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/Application/Interfaces/ITourRepository.cs) | Full CRUD + pagination |
| [ITourService.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/Application/Interfaces/ITourService.cs) | Full CRUD + UpdateStatus |
| [TourService.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/Application/Services/TourService.cs) | Full CRUD logic + unique name check |
| [ApplicationDbContext.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/Infrastructure/Data/ApplicationDbContext.cs) | Thêm DbSets, indexes, enum config |
| [TourRepository.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/Infrastructure/Repositories/TourRepository.cs) | Full CRUD + soft delete + pagination |
| [ToursController.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/WebAPI/Controllers/ToursController.cs) | 6 endpoints CRUD |
| [ServiceExtension.cs](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/WebAPI/Extensions/ServiceExtension.cs) | DI + FluentValidation |
| [WebAPI.csproj](file:///c:/Users/Thiet/Downloads/VietravelTest/Backend/WebAPI/WebAPI.csproj) | +FluentValidation package |

---

### Frontend — Files mới (11 files)

| File | Mô tả |
|------|-------|
| [types.ts](file:///c:/Users/Thiet/Downloads/VietravelTest/Frontend/app/lib/types.ts) | TypeScript interfaces |
| [api.ts](file:///c:/Users/Thiet/Downloads/VietravelTest/Frontend/app/lib/api.ts) | API client + auto-login |
| [Navbar.tsx](file:///c:/Users/Thiet/Downloads/VietravelTest/Frontend/app/components/Navbar.tsx) | Glassmorphism navigation |
| [Pagination.tsx](file:///c:/Users/Thiet/Downloads/VietravelTest/Frontend/app/components/Pagination.tsx) | Reusable pagination |
| [LoadingSpinner.tsx](file:///c:/Users/Thiet/Downloads/VietravelTest/Frontend/app/components/LoadingSpinner.tsx) | Dual-ring spinner |
| [ErrorAlert.tsx](file:///c:/Users/Thiet/Downloads/VietravelTest/Frontend/app/components/ErrorAlert.tsx) | Error display + retry |
| [/tours/page.tsx](file:///c:/Users/Thiet/Downloads/VietravelTest/Frontend/app/tours/page.tsx) | Danh sách tour + phân trang |
| [/tours/[id]/page.tsx](file:///c:/Users/Thiet/Downloads/VietravelTest/Frontend/app/tours/%5Bid%5D/page.tsx) | Chi tiết tour |
| [/tours/create/page.tsx](file:///c:/Users/Thiet/Downloads/VietravelTest/Frontend/app/tours/create/page.tsx) | Form tạo tour |
| [/requests/page.tsx](file:///c:/Users/Thiet/Downloads/VietravelTest/Frontend/app/requests/page.tsx) | Danh sách phiếu + lock buttons |
| [/requests/[id]/page.tsx](file:///c:/Users/Thiet/Downloads/VietravelTest/Frontend/app/requests/%5Bid%5D/page.tsx) | Chi tiết phiếu + dịch vụ table |
| [/requests/create/page.tsx](file:///c:/Users/Thiet/Downloads/VietravelTest/Frontend/app/requests/create/page.tsx) | Form tạo phiếu dynamic |

### Frontend — Files đã sửa (3 files)

| File | Thay đổi |
|------|----------|
| [layout.tsx](file:///c:/Users/Thiet/Downloads/VietravelTest/Frontend/app/layout.tsx) | Dark theme + Inter font + Navbar |
| [globals.css](file:///c:/Users/Thiet/Downloads/VietravelTest/Frontend/app/globals.css) | Custom theme + scrollbar |
| [page.tsx](file:///c:/Users/Thiet/Downloads/VietravelTest/Frontend/app/page.tsx) | Landing page hero |

---

## API Endpoints

### Tour Module
| Method | Route | Mô tả |
|--------|-------|-------|
| GET | `/api/tours?pageNumber=1&pageSize=10` | Danh sách phân trang |
| GET | `/api/tours/{id}` | Chi tiết tour |
| POST | `/api/tours` | Tạo mới (check trùng tên) |
| PUT | `/api/tours/{id}` | Cập nhật |
| PATCH | `/api/tours/{id}/status` | Toggle IsActive |
| DELETE | `/api/tours/{id}` | Soft delete |

### Phiếu Đề Nghị Module
| Method | Route | Mô tả |
|--------|-------|-------|
| GET | `/api/requests?pageNumber=1&pageSize=10` | Danh sách phân trang |
| GET | `/api/requests/{id}` | Chi tiết (include Details) |
| POST | `/api/requests` | Tạo + auto-calculate |
| PUT | `/api/requests/{id}` | Cập nhật (chỉ khi chưa duyệt) |
| POST | `/api/requests/{id}/approve` | Phê duyệt |
| DELETE | `/api/requests/{id}` | Soft delete (chỉ khi chưa duyệt) |

---

## Business Rules đã triển khai

1. ✅ **Tính toán tự động:** `LineTotal = Quantity × UnitPrice`, `TotalAmount = Σ LineTotals`
2. ✅ **Phân loại trạng thái:** > 100 triệu → "Chờ duyệt", ≤ 100 triệu → "Đã tiếp nhận"
3. ✅ **Cảnh báo MICE:** MICE + khách < 10 → warning banner (cả create lẫn detail page)
4. ✅ **Khóa phiếu đã duyệt:** Disable edit/delete button + block server-side
5. ✅ **Unique Tour Name:** Check ở service layer, trả 400 nếu trùng
6. ✅ **Soft Delete:** IsDeleted = true, không xóa vật lý

---

## Verification Results

| Test | Kết quả |
|------|---------|
| Backend `dotnet build` | ✅ 0 errors, 2 nullable warnings |
| Frontend `next build` | ✅ 8/8 routes succeeded |
| EF Migration | ⏳ Cần PostgreSQL container chạy |
| Docker Compose | ⏳ Cần chạy `docker compose up --build` |

---

## Cách chạy

```bash
# Chạy toàn bộ qua Docker
docker compose up --build

# Hoặc chạy từng service:
# 1. PostgreSQL
docker compose up postgres-db

# 2. Backend (terminal riêng)
cd Backend && dotnet run --project WebAPI

# 3. Frontend (terminal riêng)  
cd Frontend && npm run dev
```

Sau khi chạy:
- Frontend: http://localhost:3000
- Backend API: http://localhost:8080
- Auth login: GET http://localhost:8080/api/auth/login
