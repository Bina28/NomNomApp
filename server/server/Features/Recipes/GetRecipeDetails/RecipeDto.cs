
using System.Text.Json.Serialization;

namespace server.Features.Recipes.GetRecipeDetails;

public record RecipeDto(
 string? Title,
  string? Summary,
 string? Instructions,
 [property: JsonPropertyName("extendedIngredients")]
 List<string>?  ExtendedIngredients
);

