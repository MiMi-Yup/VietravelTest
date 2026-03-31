# Task Tracker — Vietravel Full Implementation

## Phase 0: Sửa Bug
- [x] Fix `ApplicationExtension.cs` (UseAuthentication x2 → +UseAuthorization)

## Phase 1: Domain Layer
- [x] Tạo enums `TourType.cs`, `RequestStatus.cs`
- [x] Tạo entity `BookingRequest.cs`
- [x] Tạo entity `RequestDetail.cs`

## Phase 2: Application Layer
- [x] Tạo `PagedList.cs`
- [x] Mở rộng `ITourRepository.cs` + `ITourService.cs`
- [x] Tạo `IBookingRequestRepository.cs` + `IBookingRequestService.cs`
- [x] Mở rộng `TourService.cs`
- [x] Tạo `BookingRequestService.cs`

## Phase 3: Infrastructure Layer
- [x] Sửa `ApplicationDbContext.cs` (thêm DbSets + config)
- [x] Mở rộng `TourRepository.cs`
- [x] Tạo `BookingRequestRepository.cs`
- [ ] Tạo EF Migration (cần PostgreSQL running)

## Phase 4: WebAPI Layer
- [x] Tạo DTOs (Requests + Responses)
- [x] Tạo Validators (FluentValidation)
- [x] Mở rộng `ToursController.cs`
- [x] Tạo `RequestsController.cs`
- [x] Sửa `ServiceExtension.cs` (DI + FluentValidation)
- [x] Sửa `WebAPI.csproj` (thêm FluentValidation package)
- [x] Cập nhật `ExceptionHandlingMiddleware` (handle business exceptions)

## Phase 5: Frontend
- [x] Tạo `lib/api.ts` + `lib/types.ts`
- [x] Tạo components: Navbar, Pagination, LoadingSpinner, ErrorAlert
- [x] Sửa `layout.tsx` + `globals.css`
- [x] Trang Tours: list, detail, create
- [x] Trang Requests: list, detail, create
- [x] Sửa `page.tsx` (landing page)

## Verification
- [x] Backend build thành công (0 errors, 2 warnings)
- [x] Frontend build thành công (8/8 routes generated)
- [ ] EF Migration (cần PostgreSQL container)
- [ ] Docker Compose integration test
