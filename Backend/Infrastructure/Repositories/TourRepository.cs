using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TourRepository : ITourRepository
{
    private readonly ApplicationDbContext _context;

    public TourRepository(ApplicationDbContext context) => _context = context;

    public async Task<PagedList<Tour>> GetToursAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Tours
            .AsNoTracking()
            .Where(t => !t.IsDeleted)
            .OrderByDescending(t => t.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<Tour>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<Tour?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tours
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Tours
            .Where(t => !t.IsDeleted && t.Name == name);

        if (excludeId.HasValue)
            query = query.Where(t => t.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<Tour> CreateAsync(Tour tour, CancellationToken cancellationToken = default)
    {
        await _context.Tours.AddAsync(tour, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return tour;
    }

    public async Task<Tour> UpdateAsync(Tour tour, CancellationToken cancellationToken = default)
    {
        _context.Tours.Update(tour);
        await _context.SaveChangesAsync(cancellationToken);
        return tour;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tour = await _context.Tours.FindAsync(new object[] { id }, cancellationToken);
        if (tour != null)
        {
            tour.IsDeleted = true;
            tour.UpdatedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}