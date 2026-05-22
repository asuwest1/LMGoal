using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoftwareTracker.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SoftwareTitles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Vendor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoftwareTitles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LicensePurchases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoftwareTitleId = table.Column<int>(type: "int", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LicenseKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LicenseType = table.Column<int>(type: "int", nullable: false),
                    OrderNumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicensePurchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LicensePurchases_SoftwareTitles_SoftwareTitleId",
                        column: x => x.SoftwareTitleId,
                        principalTable: "SoftwareTitles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoftwareTitleId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CostPerPeriod = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BillingPeriod = table.Column<int>(type: "int", nullable: false),
                    AutoRenews = table.Column<bool>(type: "bit", nullable: false),
                    SeatCount = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_SoftwareTitles_SoftwareTitleId",
                        column: x => x.SoftwareTitleId,
                        principalTable: "SoftwareTitles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceContracts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LicensePurchaseId = table.Column<int>(type: "int", nullable: false),
                    ContractNumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AnnualCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    VendorContact = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceContracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceContracts_LicensePurchases_LicensePurchaseId",
                        column: x => x.LicensePurchaseId,
                        principalTable: "LicensePurchases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LicensePurchases_SoftwareTitleId",
                table: "LicensePurchases",
                column: "SoftwareTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceContracts_LicensePurchaseId",
                table: "MaintenanceContracts",
                column: "LicensePurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_SoftwareTitleId",
                table: "Subscriptions",
                column: "SoftwareTitleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "MaintenanceContracts");
            migrationBuilder.DropTable(name: "Subscriptions");
            migrationBuilder.DropTable(name: "LicensePurchases");
            migrationBuilder.DropTable(name: "SoftwareTitles");
        }
    }
}
