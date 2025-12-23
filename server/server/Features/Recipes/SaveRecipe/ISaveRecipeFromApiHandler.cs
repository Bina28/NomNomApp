using server.Domain;

namespace Server.Features.Recipes.SaveRecipe;

public interface ISaveRecipeFromApiHandler
{
    Task<Recipe> SaveRecipe(Recipe recipe);
}
