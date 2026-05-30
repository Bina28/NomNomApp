namespace Server.Domain;

public class Recipe
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Summary { get; set; }
    public string? Instructions { get; set; }
    public string? Image { get; set; }

    public string? UserId { get; set; }
    public User? User { get; set; }

    public List<Ingredient> ExtendedIngredients { get; set; } = [];
    public List<RecipeIngredient> UserIngredients { get; set; } = [];
    public Photo? Photos { get; set; }
    public List<Comment> Comments { get; set; } = [];
}
