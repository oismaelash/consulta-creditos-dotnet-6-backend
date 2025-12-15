using System;
using ConsultaCreditos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ConsultaCreditos.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241215000001_AddAuditoriaConsulta")]
    public partial class AddAuditoriaConsulta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "auditoria_consulta",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    message_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    occurred_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tipo_consulta = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    chave_consulta = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    correlation_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auditoria_consulta", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_consulta_tipo_consulta",
                table: "auditoria_consulta",
                column: "tipo_consulta");

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_consulta_chave_consulta",
                table: "auditoria_consulta",
                column: "chave_consulta");

            migrationBuilder.CreateIndex(
                name: "IX_auditoria_consulta_occurred_at",
                table: "auditoria_consulta",
                column: "occurred_at");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auditoria_consulta");
        }
    }
}

