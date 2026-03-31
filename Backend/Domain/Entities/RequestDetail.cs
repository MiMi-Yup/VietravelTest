using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class RequestDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    [Required]
    public Guid BookingRequestId { get; set; }

    [Required]
    public string ServiceType { get; set; } = string.Empty;

    [Required]
    public string ServiceName { get; set; } = string.Empty;

    [Required]
    public string Supplier { get; set; } = string.Empty;

    [Required]
    public int Quantity { get; set; }

    [Required]
    [Column(TypeName = "numeric(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "numeric(18,2)")]
    public decimal LineTotal { get; set; }

    public string? Note { get; set; }

    [DefaultValue(false)]
    public bool IsDeleted { get; set; } = false;

    // Navigation property
    [ForeignKey(nameof(BookingRequestId))]
    public BookingRequest BookingRequest { get; set; } = null!;
}
