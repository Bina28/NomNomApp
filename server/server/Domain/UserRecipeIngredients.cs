using System.ComponentModel.DataAnnotations.Schema;

namespace server.Domain;

public class UserRecipeIngredients
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Name { get; set; }
    public required string Amount { get; set; }

    public  string? RecipeId { get; set; }
    public UserRecipe Recipe { get; set; } = null!;
}
