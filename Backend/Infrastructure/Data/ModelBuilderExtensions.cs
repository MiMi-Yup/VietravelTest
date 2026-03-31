using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public static class ModelBuilderExtensions
{
    public static void SeedTours(this ModelBuilder modelBuilder)
    {
        var tours = new List<Tour>();

        var fixedDate = new DateTime(2026, 03, 29, 0, 0, 0, DateTimeKind.Utc);
        var cities = new[] { "Hà Nội", "Đà Nẵng", "Hồ Chí Minh", "Đà Lạt", "Nha Trang", "Phú Quốc", "Sapa", "Hạ Long", "Hội An", "Cần Thơ" };

        for (int i = 0; i < 5; i++)
        {
            var city = cities[i % cities.Length];
            var days = (i % 5) + 2; // Random logic từ 2 đến 6 ngày

            tours.Add(new Tour
            {
                // Tạo Guid mang tính tất định (Deterministic) dựa trên vòng lặp
                Id = Guid.Parse($"11111111-2222-3333-4444-{i:D12}"),
                Name = $"Tour du lịch khám phá {city} {days} ngày {days - 1} đêm",
                Description = $"Trải nghiệm văn hóa, ẩm thực và các địa danh nổi tiếng tại {city}. Phù hợp cho gia đình và khách lẻ.",
                Price = 1500000m + (i * 200000m), // Giá tăng dần để dễ test Sort
                City = city,
                CreatedAt = fixedDate,
                UpdatedAt = fixedDate,
                IsActive = true,
                IsDeleted = false
            });
        }

        // Đẩy 30 record vào cấu hình của EF Core
        modelBuilder.Entity<Tour>().HasData(tours);
    }
}