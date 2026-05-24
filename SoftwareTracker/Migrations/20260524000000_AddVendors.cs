using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoftwareTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddVendors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vendors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Website = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VendorContacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorContacts_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.DropColumn(
                name: "Vendor",
                table: "SoftwareTitles");

            migrationBuilder.AddColumn<int>(
                name: "VendorId",
                table: "SoftwareTitles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VendorId",
                table: "LicensePurchases",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VendorId",
                table: "MaintenanceContracts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VendorId",
                table: "Subscriptions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendorContacts_VendorId",
                table: "VendorContacts",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_SoftwareTitles_VendorId",
                table: "SoftwareTitles",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_LicensePurchases_VendorId",
                table: "LicensePurchases",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceContracts_VendorId",
                table: "MaintenanceContracts",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_VendorId",
                table: "Subscriptions",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_SoftwareTitles_Vendors_VendorId",
                table: "SoftwareTitles",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_LicensePurchases_Vendors_VendorId",
                table: "LicensePurchases",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceContracts_Vendors_VendorId",
                table: "MaintenanceContracts",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Vendors_VendorId",
                table: "Subscriptions",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_SoftwareTitles_Vendors_VendorId", table: "SoftwareTitles");
            migrationBuilder.DropForeignKey(name: "FK_LicensePurchases_Vendors_VendorId", table: "LicensePurchases");
            migrationBuilder.DropForeignKey(name: "FK_MaintenanceContracts_Vendors_VendorId", table: "MaintenanceContracts");
            migrationBuilder.DropForeignKey(name: "FK_Subscriptions_Vendors_VendorId", table: "Subscriptions");

            migrationBuilder.DropIndex(name: "IX_SoftwareTitles_VendorId", table: "SoftwareTitles");
            migrationBuilder.DropIndex(name: "IX_LicensePurchases_VendorId", table: "LicensePurchases");
            migrationBuilder.DropIndex(name: "IX_MaintenanceContracts_VendorId", table: "MaintenanceContracts");
            migrationBuilder.DropIndex(name: "IX_Subscriptions_VendorId", table: "Subscriptions");
            migrationBuilder.DropIndex(name: "IX_VendorContacts_VendorId", table: "VendorContacts");

            migrationBuilder.DropColumn(name: "VendorId", table: "SoftwareTitles");
            migrationBuilder.DropColumn(name: "VendorId", table: "LicensePurchases");
            migrationBuilder.DropColumn(name: "VendorId", table: "MaintenanceContracts");
            migrationBuilder.DropColumn(name: "VendorId", table: "Subscriptions");

            migrationBuilder.AddColumn<string>(
                name: "Vendor",
                table: "SoftwareTitles",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.DropTable(name: "VendorContacts");
            migrationBuilder.DropTable(name: "Vendors");
        }
    }
}
