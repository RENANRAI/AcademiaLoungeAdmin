using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademiaLounge.Migrations
{
    /// <inheritdoc />
    public partial class CreateAssinaturas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "assinaturas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    aluno_id = table.Column<Guid>(type: "uuid", nullable: false),
                    plano_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    data_vencimento = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    observacoes = table.Column<string>(type: "text", nullable: true),
                    criado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assinaturas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_assinaturas_alunos_aluno_id",
                        column: x => x.aluno_id,
                        principalTable: "alunos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_assinaturas_planos_plano_id",
                        column: x => x.plano_id,
                        principalTable: "planos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_assinaturas_aluno_id",
                table: "assinaturas",
                column: "aluno_id");

            migrationBuilder.CreateIndex(
                name: "IX_assinaturas_data_vencimento",
                table: "assinaturas",
                column: "data_vencimento");

            migrationBuilder.CreateIndex(
                name: "IX_assinaturas_plano_id",
                table: "assinaturas",
                column: "plano_id");

            migrationBuilder.CreateIndex(
                name: "IX_assinaturas_status",
                table: "assinaturas",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "assinaturas");
        }
    }
}
