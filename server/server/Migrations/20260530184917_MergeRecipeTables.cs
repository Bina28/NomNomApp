using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class MergeRecipeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRecipeIngredients");

            migrationBuilder.DropTable(
                name: "UserRecipes");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Recipes",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RecipeIngredients",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<string>(type: "text", nullable: false),
                    RecipeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeIngredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeIngredients_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_UserId",
                table: "Recipes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_RecipeId",
                table: "RecipeIngredients",
                column: "RecipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Users_UserId",
                table: "Recipes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Users_UserId",
                table: "Recipes");

            migrationBuilder.DropTable(
                name: "RecipeIngredients");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_UserId",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Recipes");

            migrationBuilder.CreateTable(
                name: "UserRecipes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRecipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRecipes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserRecipeIngredients",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    RecipeId = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRecipeIngredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRecipeIngredients_UserRecipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "UserRecipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRecipeIngredients_RecipeId",
                table: "UserRecipeIngredients",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRecipes_UserId",
                table: "UserRecipes",
                column: "UserId");
        }
    }
}
