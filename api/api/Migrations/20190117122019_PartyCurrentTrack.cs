using Microsoft.EntityFrameworkCore.Migrations;

namespace api.Migrations
{
    public partial class PartyCurrentTrack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SpotifyDeviceName",
                table: "Users",
                newName: "CurrentDevice_Name");

            migrationBuilder.RenameColumn(
                name: "SpotifyDeviceId",
                table: "Users",
                newName: "CurrentDevice_DeviceId");

            migrationBuilder.AddColumn<string>(
                name: "CurrentTrack_Artist",
                table: "Parties",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentTrack_DurationMillis",
                table: "Parties",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "CurrentTrack_IsPlaying",
                table: "Parties",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CurrentTrack_Title",
                table: "Parties",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentTrack_Uri",
                table: "Parties",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentTrack_Artist",
                table: "Parties");

            migrationBuilder.DropColumn(
                name: "CurrentTrack_DurationMillis",
                table: "Parties");

            migrationBuilder.DropColumn(
                name: "CurrentTrack_IsPlaying",
                table: "Parties");

            migrationBuilder.DropColumn(
                name: "CurrentTrack_Title",
                table: "Parties");

            migrationBuilder.DropColumn(
                name: "CurrentTrack_Uri",
                table: "Parties");

            migrationBuilder.RenameColumn(
                name: "CurrentDevice_Name",
                table: "Users",
                newName: "SpotifyDeviceName");

            migrationBuilder.RenameColumn(
                name: "CurrentDevice_DeviceId",
                table: "Users",
                newName: "SpotifyDeviceId");
        }
    }
}
