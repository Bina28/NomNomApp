using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Domain;
using server.Features.Recipes.Services.Photo;

namespace server.Features.Recipes.SaveRecipe;

public class SaveRecipeFromApiHandler
{
    private readonly AppDbContext _context;
    private readonly IPhotoService _photoService;

    public SaveRecipeFromApiHandler(AppDbContext context, IPhotoService service)
    {
        _context = context;
        _photoService = service;
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
}