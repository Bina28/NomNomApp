namespace server.Domain;

public class Recipe
{
    public int Id { get; set; }
    public string? Title { get; set; }

    public string? Summary { get; set; }
    public string? Instructions { get; set; }
    public string? Image { get; set; }


    public List<Ingredient> ExtendedIngredients { get; set; } = [];
    public Photo? Photos { get; set; }

}
