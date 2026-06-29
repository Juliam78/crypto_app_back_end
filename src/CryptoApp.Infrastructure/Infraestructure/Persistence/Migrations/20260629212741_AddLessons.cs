using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CryptoApp.Infrastructure.Infraestructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLessons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lessons",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    kind = table.Column<char>(type: "char(1)", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    coin_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    coin_symbol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    recommendation = table.Column<char>(type: "char(1)", nullable: true),
                    author_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    author_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    published = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lessons", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_lessons_kind",
                table: "lessons",
                column: "kind");

            migrationBuilder.CreateIndex(
                name: "IX_lessons_published",
                table: "lessons",
                column: "published");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lessons");
        }
    }
}
