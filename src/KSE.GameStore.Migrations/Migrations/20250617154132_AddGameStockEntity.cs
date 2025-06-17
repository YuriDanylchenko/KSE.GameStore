using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KSE.GameStore.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddGameStockEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_game_stock",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    game_id = table.Column<int>(type: "int", nullable: false),
                    license = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_game_stock", x => new { x.user_id, x.game_id });
                    table.ForeignKey(
                        name: "FK_user_game_stock_games_game_id",
                        column: x => x.game_id,
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_game_stock_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_game_stock_game_id",
                table: "user_game_stock",
                column: "game_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_game_stock");
        }
    }
}
