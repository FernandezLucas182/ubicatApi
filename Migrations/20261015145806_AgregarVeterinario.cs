using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ubicatapi.Migrations
{
    public partial class AgregarVeterinario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Crear la tabla veterinario
            migrationBuilder.CreateTable(
                name: "veterinario",
                columns: table => new
                {
                    idVeterinario = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "MySql:ValueGenerationStrategy",
                            MySqlValueGenerationStrategy.IdentityColumn),

                    idUsuario = table.Column<int>(
                        type: "int",
                        nullable: false),

                    matricula = table.Column<string>(
                        type: "varchar(50)",
                        maxLength: 50,
                        nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),

                    jurisdiccion = table.Column<string>(
                        type: "varchar(100)",
                        maxLength: 100,
                        nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),

                    documentacion = table.Column<string>(
                        type: "varchar(255)",
                        maxLength: 255,
                        nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),

                    estadoVerificacion = table.Column<string>(
                        type: "varchar(20)",
                        maxLength: 20,
                        nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),

                    fechaVerificacion = table.Column<DateTime>(
                        type: "datetime(6)",
                        nullable: true),

                    observacion = table.Column<string>(
                        type: "varchar(500)",
                        maxLength: 500,
                        nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_veterinario",
                        x => x.idVeterinario);

                    table.ForeignKey(
                        name: "FK_veterinario_usuario_idUsuario",
                        column: x => x.idUsuario,
                        principalTable: "usuario",
                        principalColumn: "idUsuario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Un registro de veterinario por usuario
            migrationBuilder.CreateIndex(
                name: "IX_veterinario_idUsuario",
                table: "veterinario",
                column: "idUsuario",
                unique: true);

            // Matrícula única por jurisdicción
            migrationBuilder.CreateIndex(
                name: "IX_veterinario_jurisdiccion_matricula",
                table: "veterinario",
                columns: new[] { "jurisdiccion", "matricula" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revertir únicamente la creación de esta tabla
            migrationBuilder.DropTable(
                name: "veterinario");
        }
    }
}