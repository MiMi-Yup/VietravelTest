namespace WebAPI.DTOs.Requests;

public record CreateTourRequest(
    string Name,
    string? Description,
    decimal Price,
    string City
);
