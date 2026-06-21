using Server.Domain;

namespace Server.Features.Recipes.SaveRecipe;

public interface ISaveRecipeHandler
{
    Task<Recipe> SaveRecipeAsync(Recipe recipe, CancellationToken ct = default);
}
