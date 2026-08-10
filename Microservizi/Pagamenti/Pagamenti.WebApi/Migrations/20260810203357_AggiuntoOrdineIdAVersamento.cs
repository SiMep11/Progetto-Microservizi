using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pagamenti.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AggiuntoOrdineIdAVersamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ordineId",
                table: "Versamenti",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ordineId",
                table: "Versamenti");
        }
    }
}
