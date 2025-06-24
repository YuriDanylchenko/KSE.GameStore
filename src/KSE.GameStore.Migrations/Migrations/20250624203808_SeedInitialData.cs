using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KSE.GameStore.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "genres",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Action" },
                    { 2, "Adventure" },
                    { 3, "Role-playing" },
                    { 4, "Strategy" },
                    { 5, "Sports" },
                    { 6, "Racing" },
                    { 7, "Simulation" },
                    { 8, "Puzzle" },
                    { 9, "Horror" },
                    { 10, "Fighting" }
                });

            migrationBuilder.InsertData(
                table: "platforms",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "PC" },
                    { 2, "PlayStation 5" },
                    { 3, "Xbox Series" },
                    { 4, "Nintendo Switch" },
                    { 5, "PlayStation 4" },
                    { 6, "Xbox One" }
                });

            migrationBuilder.InsertData(
                table: "publishers",
                columns: new[] { "id", "name", "website_url", "description" },
                values: new object[,]
                {
                    { 1, "Electronic Arts", "https://www.ea.com", "Leading game publisher" },
                    { 2, "Ubisoft", "https://www.ubisoft.com", "Creators of Assassin's Creed" },
                    { 3, "Activision Blizzard", "https://www.activisionblizzard.com", "Call of Duty publisher" },
                    { 4, "Nintendo", "https://www.nintendo.com", "Mario and Zelda creators" },
                    { 5, "Sony Interactive Entertainment", "https://www.playstation.com", "PlayStation creators" }
                });

            migrationBuilder.InsertData(
                table: "regions",
                columns: new[] { "id", "code", "name" },
                values: new object[,]
                {
                    { 1, "NA", "North America" },
                    { 2, "EU", "Europe" },
                    { 3, "AS", "Asia" },
                    { 4, "SA", "South America" },
                    { 5, "OC", "Oceania" },
                    { 6, "AF", "Africa" }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "name", "email", "role", "region_id" },
                values: new object[]
                {
                    Guid.NewGuid(), 
                    "Admin", 
                    "admin@gamestore.com", 
                    "Administrator",
                    1
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM users WHERE email = 'admin@gamestore.com'");
            migrationBuilder.Sql("DELETE FROM regions WHERE id IN (1, 2, 3, 4, 5, 6)");
            migrationBuilder.Sql("DELETE FROM publishers WHERE id IN (1, 2, 3, 4, 5)");
            migrationBuilder.Sql("DELETE FROM platforms WHERE id IN (1, 2, 3, 4, 5, 6)");
            migrationBuilder.Sql("DELETE FROM genres WHERE id IN (1, 2, 3, 4, 5, 6, 7, 8, 9, 10)");
        }
    }
}
