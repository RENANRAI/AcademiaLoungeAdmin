using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademiaLounge.Migrations
{
    /// <inheritdoc />
    public partial class CreatePlanos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "planos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    descricao = table.Column<string>(type: "text", nullable: true),
                    duracao_dias = table.Column<int>(type: "integer", nullable: false),
                    valor = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_planos_ativo",
                table: "planos",
                column: "ativo");

            migrationBuilder.CreateIndex(
                name: "IX_planos_nome",
                table: "planos",
                column: "nome");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "planos");
        }
    }
}
