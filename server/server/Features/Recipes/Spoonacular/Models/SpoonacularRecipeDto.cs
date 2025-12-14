using System.Text.Json.Serialization;

namespace server.Features.Recipes.Spoonacular.Models;

public record SpoonacularRecipeDto(
string? Title,
string? Summary,
string? Instructions,
[property: JsonPropertyName("extendedIngredients")]
List<string>?  ExtendedIngredients
);
