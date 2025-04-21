using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CasoPractico2.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0A7AEBC7-A3BD-4369-AE34-8689052AF9A6", "3A4F1A4A-951F-4E18-ACF1-3B506C11E3FC", "Administrador", "ADMINISTRADOR" },
                    { "2EACDB14-CE5F-4B62-8F2D-D511E204A98B", "4F05C542-DF4C-4A8C-A165-26C115A7E267", "Usuario", "USUARIO" },
                    { "C971527E-B9FB-49A4-A748-84BEA4AB67B0", "9842DF61-EDCF-4543-876C-CEB19F2ABB81", "Organizador", "ORGANIZADOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0A7AEBC7-A3BD-4369-AE34-8689052AF9A6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2EACDB14-CE5F-4B62-8F2D-D511E204A98B");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "C971527E-B9FB-49A4-A748-84BEA4AB67B0");
        }
    }
}
