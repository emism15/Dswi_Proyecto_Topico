using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sistema_Tópico_Médico.Migrations
{
    /// <inheritdoc />
    public partial class AddFechaRegistroToCompra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DebeCambiarContraseña",
                table: "Usuarios",
                newName: "DebecambiarContraseña");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "Compras",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "Compras");

            migrationBuilder.RenameColumn(
                name: "DebecambiarContraseña",
                table: "Usuarios",
                newName: "DebeCambiarContraseña");
        }
    }
}
