using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace api.Migrations
{
    public partial class SpotifyDevice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "SpotifyTokenExpiry",
                table: "Users",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<string>(
                name: "SpotifyDeviceId",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpotifyDeviceName",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpotifyDeviceId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SpotifyDeviceName",
                table: "Users");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SpotifyTokenExpiry",
                table: "Users",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
