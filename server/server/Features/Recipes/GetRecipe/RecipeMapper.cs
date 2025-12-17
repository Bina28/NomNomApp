using server.Domain;

namespace server.Features.Recipes.GetRecipe;

public static class RecipeMapper
{
    public static RecipeDto ToDto(Recipe apiRecipe)
    {
        return new RecipeDto(
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