using Microsoft.EntityFrameworkCore.Migrations;

namespace LocalShowsOnly.Migrations
{
    public partial class AddingEventPhotoReqment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "photoURL",
                table: "Event",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "photoURL",
                table: "Event",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
