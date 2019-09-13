using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LocalShowsOnly.Migrations
{
    public partial class RSVPattendeeId_to_string : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "attendeeId",
                table: "RSVP",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "Event",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<DateTime>(
                name: "showtime",
                table: "Event",
                type: "Date",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<string>(
                name: "externalLink",
                table: "Event",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "attendeeId",
                table: "RSVP",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "Event",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<DateTime>(
                name: "showtime",
                table: "Event",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "Date");

            migrationBuilder.AlterColumn<string>(
                name: "externalLink",
                table: "Event",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 80);
        }
    }
}
