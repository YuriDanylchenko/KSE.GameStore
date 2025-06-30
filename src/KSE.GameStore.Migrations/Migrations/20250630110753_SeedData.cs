using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KSE.GameStore.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
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
                columns: new[] { "id", "name", "email", "HashedPassword", "PasswordSalt", "region_id" },
                values: new object[]
                {
                    new Guid("485930e2-9bdd-4768-b410-cbea12d230f0"), 
                    "Admin", 
                    "admin@gamestore.com", 
                    // password1234
                    "tLtrdg3TDLCdpPcm+SSAc6n7XNdub/ocZN7YrGrh2VAEF72AKtD3hiak6K6nX6c6MQ89oeCYRkL3wVEScEst3A==",
                    "0370758ad27447e3871ddccd2c2bacac0370758ad27447e3871ddccd2c2bacac",
                    1
                });
            
            migrationBuilder.InsertData(
                table: "user_roles",
                columns: new[] { "UserId", "RoleId", "Id" },
                values: new object[]
                {
                    new Guid("485930e2-9bdd-4768-b410-cbea12d230f0"), 
                    1,
                    1
                });
        }

        /// <inheritdoc />
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
