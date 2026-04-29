using server.Domain;
using Server.Features.Recipes.FindByNutrients;

namespace Server.Features.Recipes.Infrastructure.Recipes;

public interface IRecipeProvider
{
    Task<Recipe?> GetRecipeById(int id, CancellationToken ct = default);
    Task<List<Recipe>?> FindRecipesByNutrients(FindRecipesByNutrientsRequest request, CancellationToken ct = default);
}
