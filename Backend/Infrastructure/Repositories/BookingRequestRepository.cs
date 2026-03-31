using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BookingRequestRepository : IBookingRequestRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRequestRepository(ApplicationDbContext context) => _context = context;

    public async Task<PagedList<BookingRequest>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.BookingRequests
            .AsNoTracking()
            .Include(r => r.Details.Where(d => !d.IsDeleted))
            .Where(r => !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<BookingRequest>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<BookingRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.BookingRequests
            .Include(r => r.Details.Where(d => !d.IsDeleted))
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);
    }

    public async Task<BookingRequest> CreateAsync(BookingRequest request, CancellationToken cancellationToken = default)
    {
        await _context.BookingRequests.AddAsync(request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return request;
    }

    public async Task<BookingRequest> UpdateAsync(BookingRequest request, CancellationToken cancellationToken = default)
    {
        _context.BookingRequests.Update(request);
        await _context.SaveChangesAsync(cancellationToken);
        return request;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var request = await _context.BookingRequests
            .Include(r => r.Details)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (request != null)
        {
            request.IsDeleted = true;
            request.UpdatedAt = DateTimeOffset.UtcNow;

            // Soft delete all details too
            foreach (var detail in request.Details)
            {
                detail.IsDeleted = true;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
