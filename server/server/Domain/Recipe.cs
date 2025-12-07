using System.Text.Json.Serialization;

namespace server.Domain;

public class Recipe
{
    public int Id { get; set; }
    public string? Title { get; set; }
   
    public string? Summary { get; set; }
    public string? Instructions { get; set; }


    [JsonPropertyName("extendedIngredients")]
    public List<Ingredient>? ExtendedIngredients { get; set; } = new();
}
