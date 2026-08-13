using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Approvvigionamenti.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fornitori",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RagioneSociale = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fornitori", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrdiniFornitore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FornitoreId = table.Column<int>(type: "int", nullable: false),
                    ProdottoId = table.Column<int>(type: "int", nullable: false),
                    Quantita = table.Column<int>(type: "int", nullable: false),
                    PrezzoUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DataOrdine = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataRicezione = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Stato = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdiniFornitore", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdiniFornitore_Fornitori_FornitoreId",
                        column: x => x.FornitoreId,
                        principalTable: "Fornitori",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProdottiForniti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FornitoreId = table.Column<int>(type: "int", nullable: false),
                    ProdottoId = table.Column<int>(type: "int", nullable: false),
                    PrezzoAcquisto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    QuantitaRiordino = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdottiForniti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProdottiForniti_Fornitori_FornitoreId",
                        column: x => x.FornitoreId,
                        principalTable: "Fornitori",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdiniFornitore_FornitoreId",
                table: "OrdiniFornitore",
                column: "FornitoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdottiForniti_FornitoreId",
                table: "ProdottiForniti",
                column: "FornitoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdottiForniti_ProdottoId",
                table: "ProdottiForniti",
                column: "ProdottoId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdiniFornitore");

            migrationBuilder.DropTable(
                name: "ProdottiForniti");

            migrationBuilder.DropTable(
                name: "Fornitori");
        }
    }
}
