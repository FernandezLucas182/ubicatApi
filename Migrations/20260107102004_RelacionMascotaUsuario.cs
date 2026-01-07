using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ubicatapi.Migrations
{
    /// <inheritdoc />
    public partial class RelacionMascotaUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_mascota_idUsuario",
                table: "mascota",
                column: "idUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_mascota_usuario_idUsuario",
                table: "mascota",
                column: "idUsuario",
                principalTable: "usuario",
                principalColumn: "idUsuario",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mascota_usuario_idUsuario",
                table: "mascota");

            migrationBuilder.DropIndex(
                name: "IX_mascota_idUsuario",
                table: "mascota");
        }
    }
}
