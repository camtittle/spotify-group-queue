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
                    Index = table.Column<int>(nullable: false),
                    AddedByUserId = table.Column<string>(nullable: true),
                    ForPartyId = table.Column<string>(nullable: true),
                    SpotifyUri = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Artist = table.Column<string>(nullable: true),
                    DurationMillis = table.Column<long>(nullable: false)
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
                });

            migrationBuilder.CreateIndex(
                name: "IX_QueueItems_AddedByUserId",
                table: "QueueItems",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_QueueItems_ForPartyId",
                table: "QueueItems",
                column: "ForPartyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QueueItems");
        }
    }
}
