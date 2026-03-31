using Domain.Enums;

namespace WebAPI.DTOs.Requests;

public record UpdateBookingRequestDto(
    string TourName,
    DateTimeOffset DepartureDate,
    string PersonInCharge,
    TourType TourType,
    int GuestCount,
    List<CreateRequestDetailDto> Details
);
