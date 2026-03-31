using Application.Common;
using Domain.Entities;

namespace Application.Interfaces;

public interface IBookingRequestService
{
    Task<PagedList<BookingRequest>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<BookingRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BookingRequest> CreateAsync(BookingRequest request, CancellationToken cancellationToken = default);
    Task<BookingRequest> UpdateAsync(BookingRequest request, CancellationToken cancellationToken = default);
    Task<BookingRequest> ApproveAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
