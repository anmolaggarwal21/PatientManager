using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeviceManager.Migrations.UserDB
{
    public partial class addUserchange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "ea79cf84-1a0d-4b51-b083-b4a6e4bd9050", "SUPERUSER", "AQAAAAEAACcQAAAAENqejw4vlOPnyeL7GHdUJkn41cKEEYLfC2VpeEVXk+BeZcxlCnEU14hMMWXYmXukyA==", "495c0fb4-4819-4ac1-bbe8-28a727049c46", "SuperUser" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "758ed0b8-77b1-4676-ad06-597de656bf59", "SUPERNAME", "AQAAAAEAACcQAAAAEPbmYu475Egn5NuQAzaoqvc7nhFDvO9DgiOtlxiUd2PO6ivOGYRVgAJ9r51fPoMJ3g==", "fc19ec45-b81a-4286-8046-88b5ce9dd5de", "SuperName" });
        }
    }
}
