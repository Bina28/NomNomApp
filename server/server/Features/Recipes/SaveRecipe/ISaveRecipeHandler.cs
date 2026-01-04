using server.Domain;

namespace Server.Features.Recipes.SaveRecipe;

public interface ISaveRecipeHandler
{
    Task<Recipe> SaveRecipe(Recipe recipe);
}
