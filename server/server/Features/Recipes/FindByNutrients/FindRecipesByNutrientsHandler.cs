using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Features.Recipes.Services.RecipeApiClients;
using server.Features.Shared;
using Server.Features.Recipes.FindByNutrients;
using Server.Features.Recipes.GetRecipe;

namespace server.Features.Recipes.FindByNutrients;

public class FindRecipesByNutrientsHandler
{
    private readonly AppDbContext _context;
    private readonly IRecipeApiClient _client;
    private readonly ISaveRecipeFromApiHandler _apiHandler;
    public FindRecipesByNutrientsHandler(AppDbContext context, IRecipeApiClient client, ISaveRecipeFromApiHandler handler)
    {
        _context = context;
        _client = client;
        _apiHandler = handler;
    }

    public async Task<Result<List<ApiRecipeDto>>> FindRecipesByNutrients(FindRecipesByNutrientsRequest request)
    {
        var results = await _client.FindRecipesByNutrients(request);

        if (results == null)
            return Result<List<ApiRecipeDto>>
                .Fail("External API returned no data");

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

        return Result<List<ApiRecipeDto>>.Ok(results);
    }
}
