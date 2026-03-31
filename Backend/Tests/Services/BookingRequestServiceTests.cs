using Application.Common;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace Tests.Services;

[TestFixture]
public class BookingRequestServiceTests
{
    private Mock<IBookingRequestRepository> _repoMock = null!;
    private BookingRequestService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _repoMock = new Mock<IBookingRequestRepository>();
        _service = new BookingRequestService(_repoMock.Object);
    }

    // ===== CreateAsync =====

    [Test]
    public async Task CreateAsync_Under100M_SetsStatusReceived()
    {
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<BookingRequest>(), default))
            .ReturnsAsync((BookingRequest req, CancellationToken _) => req);

        var request = new BookingRequest
        {
            TourName = "Tour A",
            DepartureDate = DateTimeOffset.UtcNow.AddDays(30),
            PersonInCharge = "Nguyen Van A",
            TourType = TourType.FIT,
            GuestCount = 5,
            Details = new List<RequestDetail>
            {
                new() { ServiceType = "Hotel", ServiceName = "5 Star", Supplier = "ABC", Quantity = 5, UnitPrice = 10_000_000 }
            }
        };

        var result = await _service.CreateAsync(request);

        Assert.That(result.Status, Is.EqualTo(RequestStatus.Received));
        Assert.That(result.TotalAmount, Is.EqualTo(50_000_000));
        Assert.That(result.Details.First().LineTotal, Is.EqualTo(50_000_000));
    }

    [Test]
    public async Task CreateAsync_Over100M_SetsStatusPendingApproval()
    {
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<BookingRequest>(), default))
            .ReturnsAsync((BookingRequest req, CancellationToken _) => req);

        var request = new BookingRequest
        {
            TourName = "Tour B",
            DepartureDate = DateTimeOffset.UtcNow.AddDays(30),
            PersonInCharge = "Tran Van B",
            TourType = TourType.MICE,
            GuestCount = 50,
            Details = new List<RequestDetail>
            {
                new() { ServiceType = "Hotel", ServiceName = "Resort", Supplier = "XYZ", Quantity = 50, UnitPrice = 3_000_000 }
            }
        };

        var result = await _service.CreateAsync(request);

        Assert.That(result.Status, Is.EqualTo(RequestStatus.PendingApproval));
        Assert.That(result.TotalAmount, Is.EqualTo(150_000_000));
    }

    [Test]
    public async Task CreateAsync_CalculatesLineTotalsCorrectly()
    {
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<BookingRequest>(), default))
            .ReturnsAsync((BookingRequest req, CancellationToken _) => req);

        var request = new BookingRequest
        {
            TourName = "Tour C",
            DepartureDate = DateTimeOffset.UtcNow.AddDays(10),
            PersonInCharge = "Le Van C",
            TourType = TourType.GIT,
            GuestCount = 20,
            Details = new List<RequestDetail>
            {
                new() { ServiceType = "Transport", ServiceName = "Bus", Supplier = "A", Quantity = 2, UnitPrice = 5_000_000 },
                new() { ServiceType = "Hotel", ServiceName = "3 Star", Supplier = "B", Quantity = 20, UnitPrice = 1_000_000 },
                new() { ServiceType = "Guide", ServiceName = "Local guide", Supplier = "C", Quantity = 3, UnitPrice = 2_000_000 }
            }
        };

        var result = await _service.CreateAsync(request);

        var details = result.Details.ToList();
        Assert.That(details[0].LineTotal, Is.EqualTo(10_000_000));
        Assert.That(details[1].LineTotal, Is.EqualTo(20_000_000));
        Assert.That(details[2].LineTotal, Is.EqualTo(6_000_000));
        Assert.That(result.TotalAmount, Is.EqualTo(36_000_000));
    }

    // ===== UpdateAsync =====

    [Test]
    public void UpdateAsync_ApprovedRequest_ThrowsInvalidOperation()
    {
        var id = Guid.NewGuid();
        var existing = new BookingRequest
        {
            Id = id,
            Status = RequestStatus.Approved,
            Details = new List<RequestDetail>()
        };
        _repoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(existing);

        var update = new BookingRequest { Id = id, Details = new List<RequestDetail>() };
        Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(update));
    }

    [Test]
    public void UpdateAsync_NonExisting_ThrowsKeyNotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((BookingRequest?)null);

        var req = new BookingRequest { Id = Guid.NewGuid(), Details = new List<RequestDetail>() };
        Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(req));
    }

    // ===== ApproveAsync =====

    [Test]
    public async Task ApproveAsync_ValidRequest_SetsStatusApproved()
    {
        var id = Guid.NewGuid();
        var existing = new BookingRequest { Id = id, Status = RequestStatus.PendingApproval, Details = new List<RequestDetail>() };
        _repoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(existing);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<BookingRequest>(), default))
            .ReturnsAsync((BookingRequest req, CancellationToken _) => req);

        var result = await _service.ApproveAsync(id);

        Assert.That(result.Status, Is.EqualTo(RequestStatus.Approved));
    }

    [Test]
    public void ApproveAsync_AlreadyApproved_ThrowsInvalidOperation()
    {
        var id = Guid.NewGuid();
        var existing = new BookingRequest { Id = id, Status = RequestStatus.Approved, Details = new List<RequestDetail>() };
        _repoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(existing);

        Assert.ThrowsAsync<InvalidOperationException>(() => _service.ApproveAsync(id));
    }

    // ===== DeleteAsync =====

    [Test]
    public void DeleteAsync_ApprovedRequest_ThrowsInvalidOperation()
    {
        var id = Guid.NewGuid();
        var existing = new BookingRequest { Id = id, Status = RequestStatus.Approved, Details = new List<RequestDetail>() };
        _repoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(existing);

        Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeleteAsync(id));
    }

    [Test]
    public async Task DeleteAsync_NotApproved_CallsRepository()
    {
        var id = Guid.NewGuid();
        var existing = new BookingRequest { Id = id, Status = RequestStatus.Received, Details = new List<RequestDetail>() };
        _repoMock.Setup(r => r.GetByIdAsync(id, default)).ReturnsAsync(existing);

        await _service.DeleteAsync(id);

        _repoMock.Verify(r => r.DeleteAsync(id, default), Times.Once);
    }

    [Test]
    public void DeleteAsync_NonExisting_ThrowsKeyNotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((BookingRequest?)null);

        Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(Guid.NewGuid()));
    }
}
