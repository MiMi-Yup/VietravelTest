namespace WebAPI.DTOs.Responses;

public record PagedResponse<T>(
    List<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPrevious,
    bool HasNext
);
