using server.Domain;
using server.Features.Recipes.Services.RecipeApiClients;

namespace Server.Features.Recipes.GetRecipe;

public interface ISaveRecipeFromApiHandler
{
    Task<Recipe> SaveRecipe(ApiRecipeDto recipe);
}
