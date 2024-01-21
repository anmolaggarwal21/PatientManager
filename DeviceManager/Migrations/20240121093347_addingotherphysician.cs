using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceManager.Migrations
{
    public partial class addingotherphysician : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OtherPhysicianFullName",
                table: "PatientEntities",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OtherPhysicianNPI",
                table: "PatientEntities",
                type: "INTEGER",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtherPhysicianFullName",
                table: "PatientEntities");

            migrationBuilder.DropColumn(
                name: "OtherPhysicianNPI",
                table: "PatientEntities");
        }
    }
}
