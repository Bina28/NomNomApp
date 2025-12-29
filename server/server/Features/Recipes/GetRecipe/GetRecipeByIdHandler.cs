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

    public GetRecipeByIdHandler(IRecipeRepository repository, IRecipeApiClient client, ISaveRecipeFromApiHandler handler)
    {
        _recipeRepository = repository;
        _client = client;
        _apiHandler = handler;
    }

    public async Task<Result<RecipeResponse>> GetRecipeById(int id)
    {
        var recipeInDb = await _recipeRepository.GetByIdWithDetailsAsync(id);
        if (recipeInDb != null)

        {
            var response = RecipeMapper.ToResponse(recipeInDb);
            return Result<RecipeResponse>.Ok(response);
        }


        var apiRecipe = await _client.GetRecipeById(id);
        if (apiRecipe == null) return Result<RecipeResponse>.Fail("Recipe not found");

        var savedRecipe = await _apiHandler.SaveRecipe(apiRecipe);
        var savedResponse = RecipeMapper.ToResponse(savedRecipe);

        return Result<RecipeResponse>.Ok(savedResponse);

    }

}
