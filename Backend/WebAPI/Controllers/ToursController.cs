using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs.Requests;
using WebAPI.DTOs.Responses;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ToursController : ControllerBase
{
    private readonly ITourService _tourService;

    public ToursController(ITourService tourService)
    {
        _tourService = tourService;
    }

    /// <summary>GET /api/tours?pageNumber=1&pageSize=10</summary>
    [HttpGet]
    public async Task<IActionResult> GetTours(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 50) pageSize = 50;

        var pagedTours = await _tourService.GetToursAsync(pageNumber, pageSize, cancellationToken);

        var response = new PagedResponse<TourResponse>(
            Items: pagedTours.Items.Select(t => MapToResponse(t)).ToList(),
            PageNumber: pagedTours.PageNumber,
            PageSize: pagedTours.PageSize,
            TotalCount: pagedTours.TotalCount,
            TotalPages: pagedTours.TotalPages,
            HasPrevious: pagedTours.HasPrevious,
            HasNext: pagedTours.HasNext
        );

        return Ok(response);
    }

    /// <summary>GET /api/tours/{id}</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTourById(Guid id, CancellationToken cancellationToken = default)
    {
        var tour = await _tourService.GetByIdAsync(id, cancellationToken);
        if (tour == null)
            return NotFound(new { Message = $"Không tìm thấy Tour với Id '{id}'." });

        return Ok(MapToResponse(tour));
    }

    /// <summary>POST /api/tours</summary>
    [HttpPost]
    public async Task<IActionResult> CreateTour(
        [FromBody] CreateTourRequest request,
        CancellationToken cancellationToken = default)
    {
        var tour = new Tour
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            City = request.City
        };

        var created = await _tourService.CreateAsync(tour, cancellationToken);
        return CreatedAtAction(nameof(GetTourById), new { id = created.Id }, MapToResponse(created));
    }

    /// <summary>PUT /api/tours/{id}</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTour(
        Guid id,
        [FromBody] UpdateTourRequest request,
        CancellationToken cancellationToken = default)
    {
        var tour = new Tour
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            City = request.City
        };

        var updated = await _tourService.UpdateAsync(tour, cancellationToken);
        return Ok(MapToResponse(updated));
    }

    /// <summary>PATCH /api/tours/{id}/status</summary>
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateTourStatus(
        Guid id,
        [FromBody] UpdateTourStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        await _tourService.UpdateStatusAsync(id, request.IsActive, cancellationToken);
        return NoContent();
    }

    /// <summary>DELETE /api/tours/{id}</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTour(Guid id, CancellationToken cancellationToken = default)
    {
        await _tourService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    private static TourResponse MapToResponse(Tour tour) => new(
        Id: tour.Id,
        Name: tour.Name,
        Description: tour.Description,
        Price: tour.Price,
        City: tour.City,
        IsActive: tour.IsActive,
        CreatedAt: tour.CreatedAt,
        UpdatedAt: tour.UpdatedAt
    );
}