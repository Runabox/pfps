using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pfps.API.Migrations
{
    public partial class Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Uploads_UploadId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_UploadId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "UploadId",
                table: "Tags");

            migrationBuilder.AddColumn<Guid[]>(
                name: "Tags",
                table: "Uploads",
                type: "uuid[]",
                nullable: false,
                defaultValue: new Guid[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Uploads");

            migrationBuilder.AddColumn<Guid>(
                name: "UploadId",
                table: "Tags",
                type: "uuid",
                nullable: true);

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
        }
    }
}
