using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ubicatapi.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDatosBusquedaMascota : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "fechaPerdida",
                table: "mascota",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "latitudPerdida",
                table: "mascota",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "longitudPerdida",
                table: "mascota",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "radioBusquedaMetros",
                table: "mascota",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fechaPerdida",
                table: "mascota");

            migrationBuilder.DropColumn(
                name: "latitudPerdida",
                table: "mascota");

            migrationBuilder.DropColumn(
                name: "longitudPerdida",
                table: "mascota");

            migrationBuilder.DropColumn(
                name: "radioBusquedaMetros",
                table: "mascota");
        }
    }
}
