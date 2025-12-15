using server.Domain;
using server.Features.Recipes.Spoonacular.DTOs;

namespace server.Features.Recipes.Spoonacular;

public static class RecipeMapper
{
    public static SpoonacularRecipeDto ToDto(Recipe apiRecipe)
    {
        return new SpoonacularRecipeDto(
            Title: apiRecipe.Title,
            Summary: apiRecipe.Summary,
            Instructions: apiRecipe.Instructions,
            Image: apiRecipe.Photos?.Url,
            ExtendedIngredients: apiRecipe.ExtendedIngredients?
                .Select(i => i.Original ?? string.Empty)
                .ToList()
        );
    }
}