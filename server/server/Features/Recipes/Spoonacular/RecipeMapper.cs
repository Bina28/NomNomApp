using server.Domain;
using server.Features.Recipes.Spoonacular.Models;

namespace server.Features.Recipes.Spoonacular;

public static class RecipeMapper
{
    public static SpoonacularRecipeDto ToDto(Recipe apiRecipe)
    {
        return new SpoonacularRecipeDto(
            Title: apiRecipe.Title,
            Summary: apiRecipe.Summary,
            Instructions: apiRecipe.Instructions,
            ExtendedIngredients: apiRecipe.ExtendedIngredients?
                .Select(i => i.Original ?? string.Empty)
                .ToList()
        );
    }
}