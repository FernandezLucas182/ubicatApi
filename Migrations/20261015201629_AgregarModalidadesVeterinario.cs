using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ubicatapi.Migrations
{
    /// <inheritdoc />
    public partial class AgregarModalidadesVeterinario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ofreceServiciosPagos",
                table: "veterinario",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ofreceVoluntariado",
                table: "veterinario",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ofreceServiciosPagos",
                table: "veterinario");

            migrationBuilder.DropColumn(
                name: "ofreceVoluntariado",
                table: "veterinario");
        }
    }
}
