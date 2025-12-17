using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Features.Recipes.SaveRecipe;
using server.Features.Recipes.Services.RecipeApiClients;

namespace server.Features.Recipes.FindByNutrients;

public class FindRecipesByNutrientsHandler
{
    private readonly AppDbContext _context;
    private readonly  IRecipeApiClient _client;
    private readonly SaveRecipeFromApiHandler _apiHandler;
    public FindRecipesByNutrientsHandler(AppDbContext context, IRecipeApiClient client, SaveRecipeFromApiHandler handler)
    {
        _context = context;
        _client = client;
        _apiHandler = handler;
    }

    public async Task<List<RecipeResponse>> FindRecipesByNutrients(int calories, int number)
    {
        var results = await _client.FindRecipesByNutrients(calories, number);

        var apiIds = results.Select(x => x.Id);

        var existingIds = await _context.Recipes
            .Where(r => apiIds.Contains(r.Id))
            .Select(r => r.Id)
            .ToListAsync();

        var missingIds = apiIds.Except(existingIds);


        foreach (var id in missingIds)
        {
            var detail = await _client.GetRecipeById(id);
            if (detail == null) continue;

            await _apiHandler.SaveRecipe(detail);

        }

        return results;
    }
}
