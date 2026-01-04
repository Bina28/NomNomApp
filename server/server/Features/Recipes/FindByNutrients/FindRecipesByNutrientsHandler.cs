using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Features.Shared;
using Server.Features.Recipes.FindByNutrients;
using Server.Features.Recipes.Infrastructure.Recipes;
using Server.Features.Recipes.SaveRecipe;

namespace server.Features.Recipes.FindByNutrients;

public class FindRecipesByNutrientsHandler
{
    private readonly AppDbContext _context;
    private readonly IRecipeProvider _client;
    private readonly ISaveRecipeHandler _apiHandler;
    public FindRecipesByNutrientsHandler(AppDbContext context, IRecipeProvider client, ISaveRecipeHandler handler)
    {
        _context = context;
        _client = client;
        _apiHandler = handler;
    }

    public async Task<Result<List<FindRecipesByNutrientsResponse>>> FindRecipesByNutrients(FindRecipesByNutrientsRequest request)
    {
        var results = await _client.FindRecipesByNutrients(request);

        if (results == null)
            return Result<List<FindRecipesByNutrientsResponse>>
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

        var response = results
        .Select(FindRecipeByNutrientsMapper.ToResponse)
        .ToList();


        return Result<List<FindRecipesByNutrientsResponse>>.Ok(response);
    }
}
