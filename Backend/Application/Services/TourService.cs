using Application.Common;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class TourService : ITourService
{
    private readonly ITourRepository _repository;

    public TourService(ITourRepository repository) => _repository = repository;

    public async Task<PagedList<Tour>> GetToursAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _repository.GetToursAsync(pageNumber, pageSize, cancellationToken);
    }

    public async Task<Tour?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsByNameAsync(name, excludeId, cancellationToken);
    }

    public async Task<Tour> CreateAsync(Tour tour, CancellationToken cancellationToken = default)
    {
        // Check unique name
        var exists = await _repository.ExistsByNameAsync(tour.Name, null, cancellationToken);
        if (exists)
            throw new InvalidOperationException($"Tour với tên '{tour.Name}' đã tồn tại trong hệ thống.");

        tour.Id = Guid.NewGuid();
        tour.CreatedAt = DateTimeOffset.UtcNow;
        tour.UpdatedAt = DateTimeOffset.UtcNow;
        tour.IsActive = true;
        tour.IsDeleted = false;

        return await _repository.CreateAsync(tour, cancellationToken);
    }

    public async Task<Tour> UpdateAsync(Tour tour, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(tour.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Không tìm thấy Tour với Id '{tour.Id}'.");

        // Check unique name (exclude current tour)
        var exists = await _repository.ExistsByNameAsync(tour.Name, tour.Id, cancellationToken);
        if (exists)
            throw new InvalidOperationException($"Tour với tên '{tour.Name}' đã tồn tại trong hệ thống.");

        existing.Name = tour.Name;
        existing.Description = tour.Description;
        existing.Price = tour.Price;
        existing.City = tour.City;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        return await _repository.UpdateAsync(existing, cancellationToken);
    }

    public async Task UpdateStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Không tìm thấy Tour với Id '{id}'.");

        existing.IsActive = isActive;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        await _repository.UpdateAsync(existing, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Không tìm thấy Tour với Id '{id}'.");

        await _repository.DeleteAsync(id, cancellationToken);
    }
}