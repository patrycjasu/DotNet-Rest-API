using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Projekt.Migrations
{
    /// <inheritdoc />
    public partial class SeedOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "ClientId", "Date", "IsDeleted", "ShipmentId", "Status", "TotalPrice", "TotalWeight" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 7, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 2, 0, 110m, 13m },
                    { 2, 2, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, 0, 200m, 6m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
