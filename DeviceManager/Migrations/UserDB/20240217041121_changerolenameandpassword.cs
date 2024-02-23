using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceManager.Migrations.UserDB
{
    public partial class changerolenameandpassword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fbf6c1fa-d357-4a9a-ab52-eb0fc3ad7258",
                column: "NormalizedName",
                value: "MEMBER");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdc9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1bb42989-f7b5-415d-a84c-8b2014712a32", "AQAAAAEAACcQAAAAEP/S4lLxBITfgC5+9SXPf9DfAsBg9U9upe1SfSTovVaQhmzjNaPHsIMTZg+mWJzzoQ==", "fe7f2987-745f-4264-b00c-3226b1706ef4" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fbf6c1fa-d357-4a9a-ab52-eb0fc3ad7258",
                column: "NormalizedName",
                value: "Member");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdc9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2552256b-e913-4c06-be25-2b06686b0087", "AQAAAAEAACcQAAAAEFON+64LbmTteOuJBhw+U+YdRvmcKstEyhdLVtLhm3mM8UuDc13Kgxb5D6EXM6aAbQ==", "86082969-61fe-49a3-b136-198264f20216" });
        }
    }
}
