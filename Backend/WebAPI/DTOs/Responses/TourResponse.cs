namespace WebAPI.DTOs.Responses;

public record TourResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    string City,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
