using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public virtual DbSet<Tour> Tours { get; set; }
    public virtual DbSet<BookingRequest> BookingRequests { get; set; }
    public virtual DbSet<RequestDetail> RequestDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Tour configuration
        modelBuilder.Entity<Tour>(opts =>
        {
            opts.HasIndex(x => x.City);
            opts.HasIndex(x => new { x.City, x.IsActive, x.IsDeleted });
            opts.HasIndex(x => new { x.IsActive, x.IsDeleted });

            // Unique name index (only among non-deleted tours)
            opts.HasIndex(x => x.Name)
                .HasFilter("\"IsDeleted\" = false")
                .IsUnique();
        });

        // BookingRequest configuration
        modelBuilder.Entity<BookingRequest>(opts =>
        {
            opts.HasIndex(x => x.Status);
            opts.HasIndex(x => new { x.IsActive, x.IsDeleted });
            opts.HasIndex(x => new { x.Status, x.IsDeleted });

            // Store enum as string for readability
            opts.Property(x => x.TourType)
                .HasConversion<string>()
                .HasMaxLength(10);

            opts.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            // One-to-many: BookingRequest -> RequestDetails
            opts.HasMany(x => x.Details)
                .WithOne(x => x.BookingRequest)
                .HasForeignKey(x => x.BookingRequestId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RequestDetail configuration
        modelBuilder.Entity<RequestDetail>(opts =>
        {
            opts.HasIndex(x => x.BookingRequestId);
        });

        modelBuilder.SeedTours();
    }
}