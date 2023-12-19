using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceManager.Migrations
{
    public partial class addClaimEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClaimEntities",
                columns: table => new
                {
                    ClaimId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProviderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PatientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RevenueCode = table.Column<string>(type: "TEXT", nullable: true),
                    CPTCode = table.Column<string>(type: "TEXT", nullable: true),
                    CoveredUnits = table.Column<string>(type: "TEXT", nullable: true),
                    NonCoveredUnits = table.Column<string>(type: "TEXT", nullable: true),
                    CoveredCharges = table.Column<decimal>(type: "TEXT", nullable: true),
                    NonCoveredCharges = table.Column<decimal>(type: "TEXT", nullable: true),
                    ServiceDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TotalCharges = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimEntities", x => x.ClaimId);
                    table.ForeignKey(
                        name: "FK_ClaimEntities_PatientEntities_PatientId",
                        column: x => x.PatientId,
                        principalTable: "PatientEntities",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClaimEntities_ProviderEntities_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "ProviderEntities",
                        principalColumn: "ProviderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClaimEntities_PatientId",
                table: "ClaimEntities",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimEntities_ProviderId",
                table: "ClaimEntities",
                column: "ProviderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClaimEntities");
        }
    }
}
