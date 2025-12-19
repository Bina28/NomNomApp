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

        string? imageUrl = null;

        var recipe = new Recipe
        {
            Id = apiRecipe.Id,
            Title = apiRecipe.Title,
            Summary = apiRecipe.Summary,
            Instructions = apiRecipe.Instructions,
            Image = imageUrl,
            ExtendedIngredients = ingredients,
        };

        Photo? photo = null;

        if (!string.IsNullOrEmpty(apiRecipe.Image))
        {
            var cloudinaryResult = await _photoService.UploadImgFromUrl(apiRecipe.Image);

            if (cloudinaryResult != null)
            {
                imageUrl = cloudinaryResult.Url;
                photo = new Photo
                {
                    Url = cloudinaryResult.Url,
                    PublicId = cloudinaryResult.PublicId,
                    Recipe = recipe
                };
                recipe.Image = imageUrl;
                recipe.Photos = photo;
            }
        }

        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();

        return recipe;
    }
}