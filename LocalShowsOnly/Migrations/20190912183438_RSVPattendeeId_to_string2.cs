using Microsoft.EntityFrameworkCore.Migrations;

namespace LocalShowsOnly.Migrations
{
    public partial class RSVPattendeeId_to_string2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "reviewText",
                table: "RSVP",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "reviewText",
                table: "RSVP",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
