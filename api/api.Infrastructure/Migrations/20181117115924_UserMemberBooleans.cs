using Microsoft.EntityFrameworkCore.Migrations;

namespace Api.Infrastructure.Migrations
{
    public partial class UserMemberBooleans : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsOwner",
                table: "Users",
                nullable: false,
                computedColumnSql: "CAST(CASE WHEN OwnedPartyId IS NULL THEN 0 ELSE 1 END AS BIT)",
                oldClrType: typeof(bool),
                oldComputedColumnSql: "CASE WHEN OwnedPartyId IS NULL THEN 0 ELSE 1 END");

            migrationBuilder.AddColumn<bool>(
                name: "IsMember",
                table: "Users",
                nullable: false,
                computedColumnSql: "CAST(CASE WHEN CurrentPartyId IS NULL THEN 0 ELSE 1 END AS BIT)");

            migrationBuilder.AddColumn<bool>(
                name: "IsPendingMember",
                table: "Users",
                nullable: false,
                computedColumnSql: "CAST(CASE WHEN PendingPartyId IS NULL THEN 0 ELSE 1 END AS BIT)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMember",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsPendingMember",
                table: "Users");

            migrationBuilder.AlterColumn<bool>(
                name: "IsOwner",
                table: "Users",
                nullable: false,
                computedColumnSql: "CASE WHEN OwnedPartyId IS NULL THEN 0 ELSE 1 END",
                oldClrType: typeof(bool),
                oldComputedColumnSql: "CAST(CASE WHEN OwnedPartyId IS NULL THEN 0 ELSE 1 END AS BIT)");
        }
    }
}
