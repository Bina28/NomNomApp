using server.Domain;
using server.Features.Recipes.FindByNutrients;

namespace server.Features.Recipes.Services.RecipeApiClients;

public interface IRecipeApiClient
{
    Task<Recipe?> GetRecipeById(int id);
    Task<List<RecipeResponse>> FindRecipesByNutrients(int minCalories, int maxResults);
}
