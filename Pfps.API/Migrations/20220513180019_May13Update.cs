using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pfps.API.Migrations
{
    public partial class May13Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "Urls",
                table: "Uploads",
                type: "text[]",
                nullable: false);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Uploads_UploadId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_UploadId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Urls",
                table: "Uploads");

            migrationBuilder.DropColumn(
                name: "UploadId",
                table: "Tags");
        }
    }
}
