using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookingRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TourName = table.Column<string>(type: "text", nullable: false),
                    DepartureDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PersonInCharge = table.Column<string>(type: "text", nullable: false),
                    TourType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    GuestCount = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tours",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tours", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceType = table.Column<string>(type: "text", nullable: false),
                    ServiceName = table.Column<string>(type: "text", nullable: false),
                    Supplier = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    LineTotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestDetails_BookingRequests_BookingRequestId",
                        column: x => x.BookingRequestId,
                        principalTable: "BookingRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Tours",
                columns: new[] { "Id", "City", "CreatedAt", "Description", "IsActive", "IsDeleted", "Name", "Price", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-2222-3333-4444-000000000000"), "Hà Nội", new DateTimeOffset(new DateTime(2026, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Trải nghiệm văn hóa, ẩm thực và các địa danh nổi tiếng tại Hà Nội. Phù hợp cho gia đình và khách lẻ.", true, false, "Tour du lịch khám phá Hà Nội 2 ngày 1 đêm", 1500000m, new DateTimeOffset(new DateTime(2026, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("11111111-2222-3333-4444-000000000001"), "Đà Nẵng", new DateTimeOffset(new DateTime(2026, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Trải nghiệm văn hóa, ẩm thực và các địa danh nổi tiếng tại Đà Nẵng. Phù hợp cho gia đình và khách lẻ.", true, false, "Tour du lịch khám phá Đà Nẵng 3 ngày 2 đêm", 1700000m, new DateTimeOffset(new DateTime(2026, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("11111111-2222-3333-4444-000000000002"), "Hồ Chí Minh", new DateTimeOffset(new DateTime(2026, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Trải nghiệm văn hóa, ẩm thực và các địa danh nổi tiếng tại Hồ Chí Minh. Phù hợp cho gia đình và khách lẻ.", true, false, "Tour du lịch khám phá Hồ Chí Minh 4 ngày 3 đêm", 1900000m, new DateTimeOffset(new DateTime(2026, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("11111111-2222-3333-4444-000000000003"), "Đà Lạt", new DateTimeOffset(new DateTime(2026, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Trải nghiệm văn hóa, ẩm thực và các địa danh nổi tiếng tại Đà Lạt. Phù hợp cho gia đình và khách lẻ.", true, false, "Tour du lịch khám phá Đà Lạt 5 ngày 4 đêm", 2100000m, new DateTimeOffset(new DateTime(2026, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("11111111-2222-3333-4444-000000000004"), "Nha Trang", new DateTimeOffset(new DateTime(2026, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Trải nghiệm văn hóa, ẩm thực và các địa danh nổi tiếng tại Nha Trang. Phù hợp cho gia đình và khách lẻ.", true, false, "Tour du lịch khám phá Nha Trang 6 ngày 5 đêm", 2300000m, new DateTimeOffset(new DateTime(2026, 3, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequests_IsActive_IsDeleted",
                table: "BookingRequests",
                columns: new[] { "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequests_Status",
                table: "BookingRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequests_Status_IsDeleted",
                table: "BookingRequests",
                columns: new[] { "Status", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_RequestDetails_BookingRequestId",
                table: "RequestDetails",
                column: "BookingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_City",
                table: "Tours",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_City_IsActive_IsDeleted",
                table: "Tours",
                columns: new[] { "City", "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Tours_IsActive_IsDeleted",
                table: "Tours",
                columns: new[] { "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Tours_Name",
                table: "Tours",
                column: "Name",
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestDetails");

            migrationBuilder.DropTable(
                name: "Tours");

            migrationBuilder.DropTable(
                name: "BookingRequests");
        }
    }
}
