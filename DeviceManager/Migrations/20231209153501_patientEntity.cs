using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceManager.Migrations
{
    public partial class patientEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientEntities",
                columns: table => new
                {
                    PatientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    DOB = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Gender = table.Column<int>(type: "INTEGER", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: false),
                    state = table.Column<string>(type: "TEXT", nullable: false),
                    ZipCode = table.Column<string>(type: "TEXT", nullable: false),
                    AdmissionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DiagnosisCode1 = table.Column<string>(type: "TEXT", nullable: true),
                    DiagnosisCode2 = table.Column<string>(type: "TEXT", nullable: true),
                    DiagnosisCode3 = table.Column<string>(type: "TEXT", nullable: true),
                    DiagnosisCode4 = table.Column<string>(type: "TEXT", nullable: true),
                    DiagnosisCode5 = table.Column<string>(type: "TEXT", nullable: true),
                    AttendingPhysicianFullName = table.Column<string>(type: "TEXT", nullable: true),
                    AttendingPhysicianNPI = table.Column<string>(type: "TEXT", nullable: true),
                    ReferringPhysicianFullName = table.Column<string>(type: "TEXT", nullable: true),
                    ReferringPhysicianNPI = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProviderId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientEntities", x => x.PatientId);
                    table.ForeignKey(
                        name: "FK_PatientEntities_ProviderEntities_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "ProviderEntities",
                        principalColumn: "ProviderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientEntities_ProviderId",
                table: "PatientEntities",
                column: "ProviderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientEntities");
        }
    }
}
