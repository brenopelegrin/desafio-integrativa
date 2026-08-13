using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GerenciadorProcessos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Partes");

            migrationBuilder.RenameColumn(
                name: "TipoParte",
                table: "Partes",
                newName: "PoloTipo");

            migrationBuilder.AddColumn<bool>(
                name: "FlagDeleted",
                table: "Processos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TipoProcesso",
                table: "Processos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "EntidadeLegalId",
                table: "Partes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "FlagDeleted",
                table: "Partes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FlagDeleted",
                table: "Andamentos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "EntidadesLegais",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    NumeroDocumento = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntidadesLegais", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Partes_EntidadeLegalId",
                table: "Partes",
                column: "EntidadeLegalId");

            migrationBuilder.CreateIndex(
                name: "IX_EntidadesLegais_NumeroDocumento",
                table: "EntidadesLegais",
                column: "NumeroDocumento",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Partes_EntidadesLegais_EntidadeLegalId",
                table: "Partes",
                column: "EntidadeLegalId",
                principalTable: "EntidadesLegais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partes_EntidadesLegais_EntidadeLegalId",
                table: "Partes");

            migrationBuilder.DropTable(
                name: "EntidadesLegais");

            migrationBuilder.DropIndex(
                name: "IX_Partes_EntidadeLegalId",
                table: "Partes");

            migrationBuilder.DropColumn(
                name: "FlagDeleted",
                table: "Processos");

            migrationBuilder.DropColumn(
                name: "TipoProcesso",
                table: "Processos");

            migrationBuilder.DropColumn(
                name: "EntidadeLegalId",
                table: "Partes");

            migrationBuilder.DropColumn(
                name: "FlagDeleted",
                table: "Partes");

            migrationBuilder.DropColumn(
                name: "FlagDeleted",
                table: "Andamentos");

            migrationBuilder.RenameColumn(
                name: "PoloTipo",
                table: "Partes",
                newName: "TipoParte");

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Partes",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
