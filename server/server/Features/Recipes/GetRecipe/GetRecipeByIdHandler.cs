using server.Domain;
using server.Features.Recipes.Services.RecipeApiClients;
using server.Features.Shared;
using Server.Features.Recipes.GetRecipe;

namespace server.Features.Recipes.GetRecipe;

public class GetRecipeByIdHandler
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IRecipeApiClient _client;
    private readonly ISaveRecipeFromApiHandler _apiHandler;
    private readonly ILogger<GetRecipeByIdHandler> _logger;

    public GetRecipeByIdHandler(IRecipeRepository repository, IRecipeApiClient client, ISaveRecipeFromApiHandler handler, ILogger<GetRecipeByIdHandler> logger)
    {
        _recipeRepository = repository;
        _client = client;
        _apiHandler = handler;
        _logger = logger;
    }

    public async Task<Result<RecipeResponse>> GetRecipeById(int id)
    {
        _logger.LogInformation("GetRecipeById started. RecipeId={RecipeId}", id);
        var recipeInDb = await _recipeRepository.GetByIdWithDetailsAsync(id);
        if (recipeInDb != null)
        {
            _logger.LogInformation("Recipe found in DB. RecipeId={RecipeId}", id);
            var response = RecipeMapper.ToResponse(recipeInDb);
            return Result<RecipeResponse>.Ok(response);
        }

        _logger.LogInformation("Recipe not found in DB. Calling external API. RecipeId={RecipeId}", id);
        var apiRecipe = await _client.GetRecipeById(id);
        if (apiRecipe == null)
        {
            _logger.LogWarning("Recipe not found in external API. RecipeId={RecipeId}", id);
            return Result<RecipeResponse>.Fail("Recipe not found");
        }


        var savedRecipe = await _apiHandler.SaveRecipe(apiRecipe);
        _logger.LogInformation("Recipe saved successfully. RecipeId={RecipeId}", id);

        return Result<RecipeResponse>.Ok(RecipeMapper.ToResponse(savedRecipe));

    }

}
