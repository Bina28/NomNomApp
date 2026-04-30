using System.Text.Json.Serialization;

namespace Server.Features.Recipes.GetRecipeById;

public record RecipeResponse(
    int Id,
string? Title,
string? Summary,
string? Instructions,
    string? Image,
[property: JsonPropertyName("extendedIngredients")]
List<string>?  ExtendedIngredients
);
