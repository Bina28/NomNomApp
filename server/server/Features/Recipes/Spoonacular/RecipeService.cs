using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Domain;
using server.Features.Recipes.Photos;
using server.Features.Recipes.Spoonacular.DTOs;

namespace server.Features.Recipes.Spoonacular;

public class RecipeService
{

    private readonly AppDbContext _context;
    private readonly SpoonacularApiClient _client;
    private readonly IPhotoService _photoService;

    public RecipeService(AppDbContext context, SpoonacularApiClient client, IPhotoService service)
    {
        _context = context;
        _client = client;
        _photoService = service;
    }

    public async Task<Recipe> GetRecipeById(int id)
    {
        var recipeInDb = await _context.Recipes
            .Include(r => r.ExtendedIngredients)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (recipeInDb != null) return recipeInDb;


        var apiRecipe = await _client.GetRecipeById(id) ?? throw new Exception("Recipe not found in API");
        var savedRecipe = await SaveRecipe(apiRecipe);

        return savedRecipe;

    }
    public async Task<Recipe> SaveRecipe(Recipe apiRecipe)
    {
        var ingredients = new List<Ingredient>();

        if (apiRecipe.ExtendedIngredients != null)
        {
            foreach (var apiIng in apiRecipe.ExtendedIngredients)
            {
                var ingredient = _context.Ingredients.Local
                    .FirstOrDefault(x => x.Id == apiIng.Id)
                    ?? await _context.Ingredients.FirstOrDefaultAsync(x => x.Id == apiIng.Id);

                if (ingredient == null)
                {
                    ingredient = new Ingredient
                    {
                        Id = apiIng.Id,
                        Original = apiIng.Original
                    };
                    _context.Ingredients.Add(ingredient);
                }

                if (!ingredients.Any(x => x.Id == ingredient.Id))
                    ingredients.Add(ingredient);
            }
        }

        var recipe = new Recipe
        {
            Id = apiRecipe.Id,
            Title = apiRecipe.Title,
            Summary = apiRecipe.Summary,    
            Instructions = apiRecipe.Instructions,
            ExtendedIngredients = ingredients
        };

        _context.Recipes.Add(recipe);


        if (!string.IsNullOrEmpty(apiRecipe.Image))
        {
            var cloudinaryResult =
                await _photoService.UploadImgFromUrl(apiRecipe.Image);

            recipe.Photos = new Photo
            {
                Url = cloudinaryResult.Url,
                PublicId = cloudinaryResult.PublicId,
                Recipe = recipe
            };
        }

        await _context.SaveChangesAsync();
        return recipe;
    }


    public async Task<List<SpoonacularRecipeResponse>> FindRecipesByNutrients(int calories, int number)
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

          await SaveRecipe(detail);

        }

      
        return results;
    }
}
