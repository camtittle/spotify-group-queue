using Microsoft.EntityFrameworkCore.Migrations;

namespace api.Migrations
{
    public partial class QueueItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QueueItems",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AddedByUserId = table.Column<string>(nullable: true),
                    ForPartyId = table.Column<string>(nullable: true),
                    SpotifyUri = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Artist = table.Column<string>(nullable: true),
                    DurationMillis = table.Column<long>(nullable: false),
                    PartyId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueueItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueueItems_Users_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QueueItems_Parties_ForPartyId",
                        column: x => x.ForPartyId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QueueItems_Parties_PartyId",
                        column: x => x.PartyId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QueueItems_AddedByUserId",
                table: "QueueItems",
                column: "AddedByUserId",
                unique: true,
                filter: "[AddedByUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_QueueItems_ForPartyId",
                table: "QueueItems",
                column: "ForPartyId",
                unique: true,
                filter: "[ForPartyId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_QueueItems_PartyId",
                table: "QueueItems",
                column: "PartyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QueueItems");
        }
    }
}
