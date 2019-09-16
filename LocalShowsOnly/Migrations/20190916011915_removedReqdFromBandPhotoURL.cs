using Microsoft.EntityFrameworkCore.Migrations;

namespace LocalShowsOnly.Migrations
{
    public partial class removedReqdFromBandPhotoURL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "photoURL",
                table: "Band",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "photoURL",
                table: "Band",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
