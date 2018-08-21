using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace api.Migrations
{
    public partial class AddPendingParty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestPending",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "PendingPartyId",
                table: "Users",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PendingPartyId",
                table: "Users",
                column: "PendingPartyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Parties_PendingPartyId",
                table: "Users",
                column: "PendingPartyId",
                principalTable: "Parties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Parties_PendingPartyId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PendingPartyId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PendingPartyId",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "RequestPending",
                table: "Users",
                nullable: false,
                defaultValue: false);
        }
    }
}
