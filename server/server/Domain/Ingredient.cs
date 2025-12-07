namespace server.Domain;


public class Ingredient
{
    public int Id { get; set; }
    public string? Original { get; set; }

    public List<Recipe>? Recipes { get; set; } = new();
}