using Microsoft.EntityFrameworkCore.Migrations;

namespace booking_facilities.Migrations
{
    public partial class isBlockAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBlock",
                table: "Booking",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBlock",
                table: "Booking");
        }
    }
}
