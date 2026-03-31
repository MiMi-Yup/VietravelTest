namespace WebAPI.DTOs.Responses;

public record BookingRequestResponse(
    Guid Id,
    string TourName,
    DateTimeOffset DepartureDate,
    string PersonInCharge,
    string TourType,
    int GuestCount,
    string Status,
    decimal TotalAmount,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    List<RequestDetailResponse> Details
);

public record RequestDetailResponse(
    Guid Id,
    string ServiceType,
    string ServiceName,
    string Supplier,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal,
    string? Note
);
