using server.Domain;

namespace server.Features.Recipes.GetRecipeDetails;

public class RecipeMapper
{
    public static RecipeDto ToDto(Recipe recipe)
    {
        return new RecipeDto(
            recipe.Title,
            recipe.Summary,
            recipe.Instructions,
            recipe.ExtendedIngredients?
                .Select(i => i.Original ?? "")
                .ToList()
        );
    }
}
