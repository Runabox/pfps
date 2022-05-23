using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pfps.API.Migrations
{
    public partial class Sex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Uploads_Users_UploaderId",
                table: "Uploads");

            migrationBuilder.DropIndex(
                name: "IX_Uploads_UploaderId",
                table: "Uploads");

            migrationBuilder.DropColumn(
                name: "UploaderId",
                table: "Uploads");

            migrationBuilder.AddColumn<Guid>(
                name: "Uploader",
                table: "Uploads",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Uploader",
                table: "Uploads");

            migrationBuilder.AddColumn<Guid>(
                name: "UploaderId",
                table: "Uploads",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Uploads_UploaderId",
                table: "Uploads",
                column: "UploaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Uploads_Users_UploaderId",
                table: "Uploads",
                column: "UploaderId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
