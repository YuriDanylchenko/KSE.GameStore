using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KSE.GameStore.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class ChangeLicenseConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "license",
                table: "user_game_stock",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValueSql: "NEWID()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "license",
                table: "user_game_stock",
                type: "nvarchar(max)",
                nullable: false,
                defaultValueSql: "NEWID()",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
