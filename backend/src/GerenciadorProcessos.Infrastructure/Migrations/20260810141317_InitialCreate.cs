using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GerenciadorProcessos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Processos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NumeroProcesso = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    Assunto = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DataCriacao = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Andamentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ProcessoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Andamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Andamentos_Processos_ProcessoId",
                        column: x => x.ProcessoId,
                        principalTable: "Processos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Partes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TipoParte = table.Column<int>(type: "integer", nullable: false),
                    ProcessoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Partes_Processos_ProcessoId",
                        column: x => x.ProcessoId,
                        principalTable: "Processos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Andamentos_ProcessoId",
                table: "Andamentos",
                column: "ProcessoId");

            migrationBuilder.CreateIndex(
                name: "IX_Partes_ProcessoId",
                table: "Partes",
                column: "ProcessoId");

            migrationBuilder.CreateIndex(
                name: "IX_Processos_NumeroProcesso",
                table: "Processos",
                column: "NumeroProcesso",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Andamentos");

            migrationBuilder.DropTable(
                name: "Partes");

            migrationBuilder.DropTable(
                name: "Processos");
        }
    }
}
