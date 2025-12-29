using Server.Features.Recipes.FindByNutrients;

namespace server.Features.Recipes.Services.RecipeApiClients;

public interface IRecipeApiClient
{
    Task<ApiRecipeDto?> GetRecipeById(int id);
    Task<List<ApiRecipeDto>> FindRecipesByNutrients(FindRecipesByNutrientsRequest request);
}
