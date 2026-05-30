namespace Server.Domain;

public class RecipeIngredient
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Name { get; set; }
    public required string Amount { get; set; }

    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;
}
