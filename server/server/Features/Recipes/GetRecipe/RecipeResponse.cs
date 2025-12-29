using System.Text.Json.Serialization;

namespace server.Features.Recipes.GetRecipe;

public record RecipeResponse(
string? Title,
string? Summary,
string? Instructions,
    string? Image,
[property: JsonPropertyName("extendedIngredients")]
List<string>?  ExtendedIngredients
);
