using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pfps.API.Migrations
{
    public partial class Sex2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "Urls",
                table: "Uploads",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Urls",
                table: "Uploads");
        }
    }
}
