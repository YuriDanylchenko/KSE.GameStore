using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KSE.GameStore.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserGameStockCompositeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_user_game_stock",
                table: "user_game_stock");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_game_stock",
                table: "user_game_stock",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_user_game_stock_user_id",
                table: "user_game_stock",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_user_game_stock",
                table: "user_game_stock");

            migrationBuilder.DropIndex(
                name: "IX_user_game_stock_user_id",
                table: "user_game_stock");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_game_stock",
                table: "user_game_stock",
                columns: new[] { "user_id", "game_id" });
        }
    }
}
