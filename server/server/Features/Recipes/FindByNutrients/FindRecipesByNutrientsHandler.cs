using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Features.Recipes.Infrastructure.Recipes;
using Server.Features.Recipes.SaveRecipe;
using Server.Features.Shared;

namespace Server.Features.Recipes.FindByNutrients;

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

    public async Task<Result<PageList<FindRecipesByNutrientsResponse>>> FindRecipesByNutrientsAsync(FindRecipesByNutrientsRequest request, PageParameters parameters, CancellationToken ct = default)
    {
        var results = await _client.FindRecipesByNutrientsAsync(request, ct);

        if (results == null)
            return Result<PageList<FindRecipesByNutrientsResponse>>
                .Fail("External API returned no data");

        var apiIds = results.Select(x => x.Id).ToList();

        var existingIds = await _context.Recipes
            .AsNoTracking()
            .Where(r => apiIds.Contains(r.Id))
            .Select(r => r.Id)
            .ToListAsync(ct);

        var missingIds = apiIds.Except(existingIds);


        foreach (var id in missingIds)
        {
            var detail = await _client.GetRecipeByIdAsync(id, ct);
            if (detail == null) continue;

            await _apiHandler.SaveRecipeAsync(detail, ct);

        }

        var query = results
        .Select(FindRecipeByNutrientsMapper.ToResponse)
        .ToList();

        var pagedResponse = PageList<FindRecipesByNutrientsResponse>.Create(query, parameters.PageNumber, parameters.PageSize);

        return Result<PageList<FindRecipesByNutrientsResponse>>.Ok(pagedResponse);
    }
}
