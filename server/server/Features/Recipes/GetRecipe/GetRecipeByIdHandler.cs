using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Domain;
using server.Features.Recipes.SaveRecipe;
using server.Features.Recipes.Services.RecipeApiClients;

namespace server.Features.Recipes.GetRecipe;

public class GetRecipeByIdHandler
{
    private readonly AppDbContext _context;
    private readonly IRecipeApiClient _client;
    private readonly SaveRecipeFromApiHandler _apiHandler;

    public GetRecipeByIdHandler(AppDbContext context, IRecipeApiClient client, SaveRecipeFromApiHandler handler)
    {
        _context = context;
        _client = client;
        _apiHandler = handler;
    }

    public async Task<Recipe> GetRecipeById(int id)
    {
        var recipeInDb = await _context.Recipes
            .Include(r => r.ExtendedIngredients)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (recipeInDb != null) return recipeInDb;


        var apiRecipe = await _client.GetRecipeById(id) ?? throw new Exception("Recipe not found in API");
        var savedRecipe = await _apiHandler.SaveRecipe(apiRecipe);

        return savedRecipe;

    }
}
