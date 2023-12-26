using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceManager.Migrations
{
    public partial class toservicedate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ServiceDate",
                table: "ClaimEntities",
                newName: "ToServiceDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "FromServiceDate",
                table: "ClaimEntities",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromServiceDate",
                table: "ClaimEntities");

            migrationBuilder.RenameColumn(
                name: "ToServiceDate",
                table: "ClaimEntities",
                newName: "ServiceDate");
        }
    }
}
