using Server.Domain;
using Server.Features.Recipes.FindByNutrients;

namespace Server.Features.Recipes.Infrastructure.Recipes;

public interface IRecipeProvider
{
    Task<Recipe?> GetRecipeByIdAsync(int id, CancellationToken ct = default);
    Task<List<Recipe>?> FindRecipesByNutrientsAsync(FindRecipesByNutrientsRequest request, CancellationToken ct = default);
}
