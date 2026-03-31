using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class BookingRequestService : IBookingRequestService
{
    private readonly IBookingRequestRepository _repository;

    public BookingRequestService(IBookingRequestRepository repository) => _repository = repository;

    public async Task<PagedList<BookingRequest>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllAsync(pageNumber, pageSize, cancellationToken);
    }

    public async Task<BookingRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<BookingRequest> CreateAsync(BookingRequest request, CancellationToken cancellationToken = default)
    {
        request.Id = Guid.NewGuid();
        request.CreatedAt = DateTimeOffset.UtcNow;
        request.UpdatedAt = DateTimeOffset.UtcNow;
        request.IsActive = true;
        request.IsDeleted = false;

        // Calculate line totals and total amount
        foreach (var detail in request.Details)
        {
            detail.Id = Guid.NewGuid();
            detail.BookingRequestId = request.Id;
            detail.LineTotal = detail.Quantity * detail.UnitPrice;
            detail.IsDeleted = false;
        }

        request.TotalAmount = request.Details.Sum(d => d.LineTotal);

        // Auto-assign status based on total amount (business rule: 100M VND threshold)
        request.Status = request.TotalAmount > 100_000_000m
            ? RequestStatus.PendingApproval
            : RequestStatus.Received;

        return await _repository.CreateAsync(request, cancellationToken);
    }

    public async Task<BookingRequest> UpdateAsync(BookingRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Không tìm thấy phiếu đề nghị với Id '{request.Id}'.");

        // Business rule: Cannot update approved requests
        if (existing.Status == RequestStatus.Approved)
            throw new InvalidOperationException("Không thể chỉnh sửa phiếu đã được phê duyệt.");

        existing.TourName = request.TourName;
        existing.DepartureDate = request.DepartureDate;
        existing.PersonInCharge = request.PersonInCharge;
        existing.TourType = request.TourType;
        existing.GuestCount = request.GuestCount;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        // Replace details: mark old as deleted, add new ones
        foreach (var oldDetail in existing.Details)
        {
            oldDetail.IsDeleted = true;
        }

        foreach (var newDetail in request.Details)
        {
            newDetail.Id = Guid.NewGuid();
            newDetail.BookingRequestId = existing.Id;
            newDetail.LineTotal = newDetail.Quantity * newDetail.UnitPrice;
            newDetail.IsDeleted = false;
            existing.Details.Add(newDetail);
        }

        // Recalculate total from non-deleted details
        existing.TotalAmount = existing.Details.Where(d => !d.IsDeleted).Sum(d => d.LineTotal);

        // Re-evaluate status (only if not already approved)
        existing.Status = existing.TotalAmount > 100_000_000m
            ? RequestStatus.PendingApproval
            : RequestStatus.Received;

        return await _repository.UpdateAsync(existing, cancellationToken);
    }

    public async Task<BookingRequest> ApproveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Không tìm thấy phiếu đề nghị với Id '{id}'.");

        if (existing.Status == RequestStatus.Approved)
            throw new InvalidOperationException("Phiếu này đã được phê duyệt trước đó.");

        existing.Status = RequestStatus.Approved;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        return await _repository.UpdateAsync(existing, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Không tìm thấy phiếu đề nghị với Id '{id}'.");

        // Business rule: Cannot delete approved requests
        if (existing.Status == RequestStatus.Approved)
            throw new InvalidOperationException("Không thể xóa phiếu đã được phê duyệt.");

        await _repository.DeleteAsync(id, cancellationToken);
    }
}
