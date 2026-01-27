using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademiaLounge.Migrations
{
    /// <inheritdoc />
    public partial class CreateUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    email = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    telefone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    login = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    senha_hash = table.Column<string>(type: "text", nullable: false),
                    perfil = table.Column<int>(type: "integer", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_login",
                table: "usuarios",
                column: "login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
