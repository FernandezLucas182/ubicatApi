using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ubicatapi.Migrations
{
    /// <inheritdoc />
    public partial class FechaUltimoEscaneoQR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "fechaUltimoEscaneo",
                table: "qr",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fechaUltimoEscaneo",
                table: "qr");
        }
    }
}
