using server.Domain;

namespace server.Features.Recipes.GetRecipe;

public static class RecipeMapper
{
    public static RecipeResponse ToResponse(Recipe apiRecipe)
    {
        return new RecipeResponse(
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