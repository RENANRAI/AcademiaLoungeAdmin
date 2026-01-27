using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademiaLounge.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "alunos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    telefone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    email = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    DataNascimento = table.Column<DateOnly>(type: "date", nullable: true),
                    documento = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    observacoes = table.Column<string>(type: "text", nullable: true),
                    criado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alunos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_alunos_nome",
                table: "alunos",
                column: "nome");

            migrationBuilder.CreateIndex(
                name: "IX_alunos_status",
                table: "alunos",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_alunos_telefone",
                table: "alunos",
                column: "telefone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alunos");
        }
    }
}
