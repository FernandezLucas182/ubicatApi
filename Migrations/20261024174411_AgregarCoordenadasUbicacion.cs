using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ubicatapi.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCoordenadasUbicacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "latitud",
                table: "reporte_mascota",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "longitud",
                table: "reporte_mascota",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "latitudActual",
                table: "qr",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "longitudActual",
                table: "qr",
                type: "double",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "latitud",
                table: "reporte_mascota");

            migrationBuilder.DropColumn(
                name: "longitud",
                table: "reporte_mascota");

            migrationBuilder.DropColumn(
                name: "latitudActual",
                table: "qr");

            migrationBuilder.DropColumn(
                name: "longitudActual",
                table: "qr");
        }
    }
}
