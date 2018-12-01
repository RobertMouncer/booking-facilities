using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace booking_facilities.Migrations
{
    public partial class endBookingDateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndBookingDateTime",
                table: "Booking",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndBookingDateTime",
                table: "Booking");
        }
    }
}
