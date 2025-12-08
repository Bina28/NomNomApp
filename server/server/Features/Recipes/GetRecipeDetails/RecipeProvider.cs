using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Domain;

namespace server.Features.Recipes.GetRecipeDetails;

public class RecipeProvider
{

    private readonly AppDbContext _context;
    private readonly RecipeApiClient _client;
    private readonly RecipeSaver _saver;

    public RecipeProvider(AppDbContext context, RecipeApiClient client, RecipeSaver saver)
    {
        _context = context;
        _client = client;
        _saver = saver;
    }

    public async Task<Recipe> GetRecipeById(int id)
    {
        var recipeInDb = _context.Recipes
            .Include(r => r.ExtendedIngredients)
            .FirstOrDefault(x => x.Id == id);

        if (recipeInDb != null) return recipeInDb;


        var apiRecipe = await _client.GetRecipeFromApi(id) ?? throw new Exception("Recipe not found in API");
        var savedRecipe = await _saver.SaveRecipe(apiRecipe);

        return savedRecipe;

    }
}
