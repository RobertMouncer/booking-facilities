using Microsoft.EntityFrameworkCore.Migrations;

namespace booking_facilities.Migrations
{
    public partial class venueNameInt2String : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "VenueName",
                table: "Venue",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "VenueName",
                table: "Venue",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
