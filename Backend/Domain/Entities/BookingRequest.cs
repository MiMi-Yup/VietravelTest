using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities;

public class BookingRequest
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    [Required]
    public string TourName { get; set; } = string.Empty;

    [Required]
    public DateTimeOffset DepartureDate { get; set; }

    [Required]
    public string PersonInCharge { get; set; } = string.Empty;

    [Required]
    public TourType TourType { get; set; }

    [Required]
    public int GuestCount { get; set; }

    public RequestStatus Status { get; set; }

    [Column(TypeName = "numeric(18,2)")]
    public decimal TotalAmount { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    [DefaultValue(true)]
    public bool IsActive { get; set; } = true;

    [DefaultValue(false)]
    public bool IsDeleted { get; set; } = false;

    // Navigation property
    public ICollection<RequestDetail> Details { get; set; } = new List<RequestDetail>();
}
