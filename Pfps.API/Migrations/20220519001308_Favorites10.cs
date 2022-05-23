using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pfps.API.Migrations
{
    public partial class Favorites10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Upload",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "User",
                table: "Favorites");

            migrationBuilder.AddColumn<Guid>(
                name: "UploadId",
                table: "Favorites",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UploadId",
                table: "Favorites",
                column: "UploadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Uploads_UploadId",
                table: "Favorites",
                column: "UploadId",
                principalTable: "Uploads",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Uploads_UploadId",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_UploadId",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "UploadId",
                table: "Favorites");

            migrationBuilder.AddColumn<Guid>(
                name: "Upload",
                table: "Favorites",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "User",
                table: "Favorites",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
