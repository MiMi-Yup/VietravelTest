using Application.Common;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Moq;

namespace Tests.Services;

[TestFixture]
public class TourServiceTests
{
    private Mock<ITourRepository> _repoMock = null!;
    private TourService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _repoMock = new Mock<ITourRepository>();
        _service = new TourService(_repoMock.Object);
    }

    // ===== GetToursAsync =====

    [Test]
    public async Task GetToursAsync_ReturnsPagedList()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new() { Id = Guid.NewGuid(), Name = "Tour A", City = "Hà Nội", Price = 1000000 },
            new() { Id = Guid.NewGuid(), Name = "Tour B", City = "Đà Nẵng", Price = 2000000 }
        };
        var pagedList = new PagedList<Tour>(tours, 2, 1, 10);
        _repoMock.Setup(r => r.GetToursAsync(1, 10, default)).ReturnsAsync(pagedList);

        // Act
        var result = await _service.GetToursAsync(1, 10);

        // Assert
        Assert.That(result.Items, Has.Count.EqualTo(2));
        Assert.That(result.TotalCount, Is.EqualTo(2));
    }

    // ===== GetByIdAsync =====

    [Test]
    public async Task GetByIdAsync_ExistingId_ReturnsTour()
    {
        var id = Guid.NewGuid();
        var tour = new Tour { Id = id, Name = "Test Tour", City = "HCM", Price = 500000 };
        _repoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(tour);

        var result = await _service.GetByIdAsync(id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("Test Tour"));
    }

    [Test]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Tour?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid());

        Assert.That(result, Is.Null);
    }

    // ===== CreateAsync =====

    [Test]
    public async Task CreateAsync_UniqueName_CreatesTour()
    {
        _repoMock.Setup(r => r.ExistsByNameAsync("New Tour", null, default)).ReturnsAsync(false);
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Tour>(), default))
            .ReturnsAsync((Tour t, CancellationToken _) => t);

        var tour = new Tour { Name = "New Tour", City = "Đà Lạt", Price = 3000000 };
        var result = await _service.CreateAsync(tour);

        Assert.That(result.Name, Is.EqualTo("New Tour"));
        Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(result.IsActive, Is.True);
        Assert.That(result.IsDeleted, Is.False);
        _repoMock.Verify(r => r.CreateAsync(It.IsAny<Tour>(), default), Times.Once);
    }

    [Test]
    public void CreateAsync_DuplicateName_ThrowsInvalidOperation()
    {
        _repoMock.Setup(r => r.ExistsByNameAsync("Existing Tour", null, default)).ReturnsAsync(true);

        var tour = new Tour { Name = "Existing Tour", City = "HCM", Price = 1000000 };

        Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(tour));
    }

    // ===== UpdateAsync =====

    [Test]
    public async Task UpdateAsync_ValidTour_UpdatesSuccessfully()
    {
        var id = Guid.NewGuid();
        var existing = new Tour { Id = id, Name = "Old Name", City = "HN", Price = 1000000 };

        _repoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(existing);
        _repoMock.Setup(r => r.ExistsByNameAsync("New Name", id, default)).ReturnsAsync(false);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Tour>(), default))
            .ReturnsAsync((Tour t, CancellationToken _) => t);

        var tour = new Tour { Id = id, Name = "New Name", City = "ĐN", Price = 2000000 };
        var result = await _service.UpdateAsync(tour);

        Assert.That(result.Name, Is.EqualTo("New Name"));
        Assert.That(result.City, Is.EqualTo("ĐN"));
    }

    [Test]
    public void UpdateAsync_NonExistingTour_ThrowsKeyNotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Tour?)null);

        var tour = new Tour { Id = Guid.NewGuid(), Name = "X" };
        Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(tour));
    }

    [Test]
    public void UpdateAsync_DuplicateName_ThrowsInvalidOperation()
    {
        var id = Guid.NewGuid();
        var existing = new Tour { Id = id, Name = "Old", City = "HN" };
        _repoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(existing);
        _repoMock.Setup(r => r.ExistsByNameAsync("Duplicate", id, default)).ReturnsAsync(true);

        var tour = new Tour { Id = id, Name = "Duplicate" };
        Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(tour));
    }

    // ===== DeleteAsync =====

    [Test]
    public void DeleteAsync_NonExistingTour_ThrowsKeyNotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((Tour?)null);

        Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(Guid.NewGuid()));
    }

    [Test]
    public async Task DeleteAsync_ExistingTour_CallsRepository()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(new Tour { Id = id, Name = "T" });

        await _service.DeleteAsync(id);

        _repoMock.Verify(r => r.DeleteAsync(id, default), Times.Once);
    }
}
