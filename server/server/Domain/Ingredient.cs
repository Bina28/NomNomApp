namespace server.Domain;


public class Ingredient
{
    public string  Id { get; set; } = Guid.NewGuid().ToString();
    public string? Original { get; set; }

    public List<Recipe>? Recipes { get; set; } = new();
}