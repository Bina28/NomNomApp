using System.Text.Json.Serialization;

namespace server.Features.Recipes.Spoonacular.DTOs;

public record SpoonacularRecipeDto(
string? Title,
string? Summary,
string? Instructions,
    string? Image,
[property: JsonPropertyName("extendedIngredients")]
List<string>?  ExtendedIngredients
);
