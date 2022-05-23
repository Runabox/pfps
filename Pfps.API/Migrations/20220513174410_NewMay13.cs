using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pfps.API.Migrations
{
    public partial class NewMay13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Uploads_UploadId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Users_CreatorId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_CreatorId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_UploadId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "UploadId",
                table: "Tags");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "Tags",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UploadId",
                table: "Tags",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CreatorId",
                table: "Tags",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_UploadId",
                table: "Tags",
                column: "UploadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Uploads_UploadId",
                table: "Tags",
                column: "UploadId",
                principalTable: "Uploads",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Users_CreatorId",
                table: "Tags",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
