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
public class RequestsController : ControllerBase
{
    private readonly IBookingRequestService _requestService;

    public RequestsController(IBookingRequestService requestService)
    {
        _requestService = requestService;
    }

    /// <summary>GET /api/requests?pageNumber=1&pageSize=10</summary>
    [HttpGet]
    public async Task<IActionResult> GetRequests(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 50) pageSize = 50;

        var pagedRequests = await _requestService.GetAllAsync(pageNumber, pageSize, cancellationToken);

        var response = new PagedResponse<BookingRequestResponse>(
            Items: pagedRequests.Items.Select(r => MapToResponse(r)).ToList(),
            PageNumber: pagedRequests.PageNumber,
            PageSize: pagedRequests.PageSize,
            TotalCount: pagedRequests.TotalCount,
            TotalPages: pagedRequests.TotalPages,
            HasPrevious: pagedRequests.HasPrevious,
            HasNext: pagedRequests.HasNext
        );

        return Ok(response);
    }

    /// <summary>GET /api/requests/{id}</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRequestById(Guid id, CancellationToken cancellationToken = default)
    {
        var request = await _requestService.GetByIdAsync(id, cancellationToken);
        if (request == null)
            return NotFound(new { Message = $"Không tìm thấy phiếu đề nghị với Id '{id}'." });

        return Ok(MapToResponse(request));
    }

    /// <summary>POST /api/requests</summary>
    [HttpPost]
    public async Task<IActionResult> CreateRequest(
        [FromBody] CreateBookingRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        var bookingRequest = new BookingRequest
        {
            TourName = dto.TourName,
            DepartureDate = dto.DepartureDate,
            PersonInCharge = dto.PersonInCharge,
            TourType = dto.TourType,
            GuestCount = dto.GuestCount,
            Details = dto.Details.Select(d => new RequestDetail
            {
                ServiceType = d.ServiceType,
                ServiceName = d.ServiceName,
                Supplier = d.Supplier,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                Note = d.Note
            }).ToList()
        };

        var created = await _requestService.CreateAsync(bookingRequest, cancellationToken);
        return CreatedAtAction(nameof(GetRequestById), new { id = created.Id }, MapToResponse(created));
    }

    /// <summary>PUT /api/requests/{id}</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateRequest(
        Guid id,
        [FromBody] UpdateBookingRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        var bookingRequest = new BookingRequest
        {
            Id = id,
            TourName = dto.TourName,
            DepartureDate = dto.DepartureDate,
            PersonInCharge = dto.PersonInCharge,
            TourType = dto.TourType,
            GuestCount = dto.GuestCount,
            Details = dto.Details.Select(d => new RequestDetail
            {
                ServiceType = d.ServiceType,
                ServiceName = d.ServiceName,
                Supplier = d.Supplier,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                Note = d.Note
            }).ToList()
        };

        var updated = await _requestService.UpdateAsync(bookingRequest, cancellationToken);
        return Ok(MapToResponse(updated));
    }

    /// <summary>POST /api/requests/{id}/approve</summary>
    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> ApproveRequest(Guid id, CancellationToken cancellationToken = default)
    {
        var approved = await _requestService.ApproveAsync(id, cancellationToken);
        return Ok(MapToResponse(approved));
    }

    /// <summary>DELETE /api/requests/{id}</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteRequest(Guid id, CancellationToken cancellationToken = default)
    {
        await _requestService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    private static BookingRequestResponse MapToResponse(BookingRequest r) => new(
        Id: r.Id,
        TourName: r.TourName,
        DepartureDate: r.DepartureDate,
        PersonInCharge: r.PersonInCharge,
        TourType: r.TourType.ToString(),
        GuestCount: r.GuestCount,
        Status: r.Status.ToString(),
        TotalAmount: r.TotalAmount,
        CreatedAt: r.CreatedAt,
        UpdatedAt: r.UpdatedAt,
        Details: r.Details.Where(d => !d.IsDeleted).Select(d => new RequestDetailResponse(
            Id: d.Id,
            ServiceType: d.ServiceType,
            ServiceName: d.ServiceName,
            Supplier: d.Supplier,
            Quantity: d.Quantity,
            UnitPrice: d.UnitPrice,
            LineTotal: d.LineTotal,
            Note: d.Note
        )).ToList()
    );
}
