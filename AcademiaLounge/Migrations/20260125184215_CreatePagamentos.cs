using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademiaLounge.Migrations
{
    /// <inheritdoc />
    public partial class CreatePagamentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pagamentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    assinatura_id = table.Column<Guid>(type: "uuid", nullable: false),
                    competencia = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    valor = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    forma = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    data_pagamento = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    observacoes = table.Column<string>(type: "text", nullable: true),
                    criado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pagamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pagamentos_assinaturas_assinatura_id",
                        column: x => x.assinatura_id,
                        principalTable: "assinaturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_pagamentos_assinatura_id",
                table: "pagamentos",
                column: "assinatura_id");

            migrationBuilder.CreateIndex(
                name: "IX_pagamentos_assinatura_id_competencia",
                table: "pagamentos",
                columns: new[] { "assinatura_id", "competencia" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pagamentos_competencia",
                table: "pagamentos",
                column: "competencia");

            migrationBuilder.CreateIndex(
                name: "IX_pagamentos_status",
                table: "pagamentos",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pagamentos");
        }
    }
}
