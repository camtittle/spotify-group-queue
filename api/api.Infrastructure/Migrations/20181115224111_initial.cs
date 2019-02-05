using Microsoft.EntityFrameworkCore.Migrations;

namespace Api.Infrastructure.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Parties",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    OwnedPartyId = table.Column<string>(nullable: true),
                    IsOwner = table.Column<bool>(nullable: false, computedColumnSql: "CASE WHEN OwnedPartyId IS NULL THEN 0 ELSE 1 END"),
                    CurrentPartyId = table.Column<string>(nullable: true),
                    PendingPartyId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Parties_CurrentPartyId",
                        column: x => x.CurrentPartyId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Parties_OwnedPartyId",
                        column: x => x.OwnedPartyId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Parties_PendingPartyId",
                        column: x => x.PendingPartyId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_CurrentPartyId",
                table: "Users",
                column: "CurrentPartyId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_OwnedPartyId",
                table: "Users",
                column: "OwnedPartyId",
                unique: true,
                filter: "[OwnedPartyId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PendingPartyId",
                table: "Users",
                column: "PendingPartyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Parties");
        }
    }
}
