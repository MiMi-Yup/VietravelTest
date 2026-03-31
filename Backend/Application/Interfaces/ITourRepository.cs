using Application.Common;
using Domain.Entities;

namespace Application.Interfaces;

public interface ITourRepository
{
    Task<PagedList<Tour>> GetToursAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<Tour?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<Tour> CreateAsync(Tour tour, CancellationToken cancellationToken = default);
    Task<Tour> UpdateAsync(Tour tour, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}