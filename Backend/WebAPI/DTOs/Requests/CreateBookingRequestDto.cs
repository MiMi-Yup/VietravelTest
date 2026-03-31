using Domain.Enums;

namespace WebAPI.DTOs.Requests;

public record CreateBookingRequestDto(
    string TourName,
    DateTimeOffset DepartureDate,
    string PersonInCharge,
    TourType TourType,
    int GuestCount,
    List<CreateRequestDetailDto> Details
);

public record CreateRequestDetailDto(
    string ServiceType,
    string ServiceName,
    string Supplier,
    int Quantity,
    decimal UnitPrice,
    string? Note
);
