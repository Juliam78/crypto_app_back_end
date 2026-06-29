using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoApp.Infrastructure.Infraestructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RedesignMovement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_movements_crypto_currencies_crypto_id",
                table: "movements");

            migrationBuilder.DropForeignKey(
                name: "FK_movements_persons_person_id",
                table: "movements");

            migrationBuilder.DropForeignKey(
                name: "FK_movements_portfolios_portfolio_id",
                table: "movements");

            migrationBuilder.DropIndex(
                name: "IX_movements_crypto_id",
                table: "movements");

            migrationBuilder.DropIndex(
                name: "IX_movements_person_id",
                table: "movements");

            migrationBuilder.DropIndex(
                name: "IX_movements_portfolio_id",
                table: "movements");

            migrationBuilder.DropColumn(
                name: "crypto_id",
                table: "movements");

            migrationBuilder.DropColumn(
                name: "person_id",
                table: "movements");

            migrationBuilder.DropColumn(
                name: "portfolio_id",
                table: "movements");

            migrationBuilder.AlterColumn<char>(
                name: "type",
                table: "movements",
                type: "char(1)",
                nullable: false,
                oldClrType: typeof(char),
                oldType: "character(1)");

            migrationBuilder.AddColumn<string>(
                name: "coin_id",
                table: "movements",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "coin_name",
                table: "movements",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "coin_symbol",
                table: "movements",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "currency",
                table: "movements",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "user_id",
                table: "movements",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "user_name",
                table: "movements",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_movements_user_id",
                table: "movements",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_movements_user_id_coin_id_currency",
                table: "movements",
                columns: new[] { "user_id", "coin_id", "currency" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_movements_user_id",
                table: "movements");

            migrationBuilder.DropIndex(
                name: "IX_movements_user_id_coin_id_currency",
                table: "movements");

            migrationBuilder.DropColumn(
                name: "coin_id",
                table: "movements");

            migrationBuilder.DropColumn(
                name: "coin_name",
                table: "movements");

            migrationBuilder.DropColumn(
                name: "coin_symbol",
                table: "movements");

            migrationBuilder.DropColumn(
                name: "currency",
                table: "movements");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "movements");

            migrationBuilder.DropColumn(
                name: "user_name",
                table: "movements");

            migrationBuilder.AlterColumn<char>(
                name: "type",
                table: "movements",
                type: "character(1)",
                nullable: false,
                oldClrType: typeof(char),
                oldType: "char(1)");

            migrationBuilder.AddColumn<int>(
                name: "crypto_id",
                table: "movements",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "person_id",
                table: "movements",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "portfolio_id",
                table: "movements",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_movements_crypto_id",
                table: "movements",
                column: "crypto_id");

            migrationBuilder.CreateIndex(
                name: "IX_movements_person_id",
                table: "movements",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_movements_portfolio_id",
                table: "movements",
                column: "portfolio_id");

            migrationBuilder.AddForeignKey(
                name: "FK_movements_crypto_currencies_crypto_id",
                table: "movements",
                column: "crypto_id",
                principalTable: "crypto_currencies",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_movements_persons_person_id",
                table: "movements",
                column: "person_id",
                principalTable: "persons",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_movements_portfolios_portfolio_id",
                table: "movements",
                column: "portfolio_id",
                principalTable: "portfolios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
