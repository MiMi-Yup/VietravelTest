namespace WebAPI.DTOs.Requests;

public record UpdateTourRequest(
    string Name,
    string? Description,
    decimal Price,
    string City
);
