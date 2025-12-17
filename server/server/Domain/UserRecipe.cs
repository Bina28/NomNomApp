using System.ComponentModel.DataAnnotations.Schema;

namespace server.Domain;

public class UserRecipe
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Title { get; set; }

    public List<UserRecipeIngredients> Ingredients { get; set; } = [];
    public string? UserId { get; set; }
    public User User { get; set; } = null!;
}
