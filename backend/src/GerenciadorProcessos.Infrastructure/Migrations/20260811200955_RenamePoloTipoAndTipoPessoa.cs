using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GerenciadorProcessos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenamePoloTipoAndTipoPessoa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PoloTipo",
                table: "Partes",
                newName: "TipoPolo");

            migrationBuilder.RenameColumn(
                name: "Tipo",
                table: "EntidadesLegais",
                newName: "TipoEntidade");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TipoPolo",
                table: "Partes",
                newName: "PoloTipo");

            migrationBuilder.RenameColumn(
                name: "TipoEntidade",
                table: "EntidadesLegais",
                newName: "Tipo");
        }
    }
}
