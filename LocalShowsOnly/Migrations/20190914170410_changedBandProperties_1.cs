using Microsoft.EntityFrameworkCore.Migrations;

namespace LocalShowsOnly.Migrations
{
    public partial class changedBandProperties_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "linkToMusic",
                table: "Band",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "linkToMusic",
                table: "Band");
        }
    }
}
