using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceProfilesAndOrderInvoiceFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveredAt",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceCompanyAddress",
                table: "Orders",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceCompanyName",
                table: "Orders",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceEmail",
                table: "Orders",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InvoiceIssuedAt",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InvoiceRequested",
                table: "Orders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceTaxCode",
                table: "Orders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InvoiceProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaxCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    CompanyAddress = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceProfiles_UserId",
                table: "InvoiceProfiles",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoiceProfiles");

            migrationBuilder.DropColumn(
                name: "DeliveredAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "InvoiceCompanyAddress",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "InvoiceCompanyName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "InvoiceEmail",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "InvoiceIssuedAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "InvoiceRequested",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "InvoiceTaxCode",
                table: "Orders");
        }
    }
}
