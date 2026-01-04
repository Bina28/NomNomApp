using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Domain;
using Server.Features.Recipes.Infrastructure.Photo;

namespace Server.Features.Recipes.SaveRecipe;

public class SaveRecipeHandler : ISaveRecipeHandler
{
    private readonly AppDbContext _context;
    private readonly IPhotoProvider _photoService;

    public SaveRecipeHandler(AppDbContext context, IPhotoProvider service)
    {
        _context = context;
        _photoService = service;
    }


    public async Task<Recipe> SaveRecipe(Recipe recipe)
    {
        recipe.ExtendedIngredients = await ResolveIngredients(recipe);
        await HandleImage(recipe);

        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();

        return recipe;
    }

    private async Task<List<Ingredient>> ResolveIngredients(Recipe recipe)
    {
        var ingredients = new List<Ingredient>();

        foreach (var ing in recipe.ExtendedIngredients)
        {
            var existing = await _context.Ingredients.FirstOrDefaultAsync(x => x.Original == ing.Original);

            if (existing == null)
            {
                existing = new Ingredient
                {
                    Original = ing.Original
                };
                _context.Ingredients.Add(existing);
            }

            if (!ingredients.Any(x => x.Original == existing.Original))
                ingredients.Add(existing);

        }
        return ingredients;
    }

    private async Task HandleImage(Recipe recipe)
    {
        if (string.IsNullOrEmpty(recipe.Image)) return;

        var uploadPhoto = await _photoService.UploadImgFromUrl(recipe.Image);

        if (uploadPhoto == null) return;

        var photo = new Photo
        {
            Url = uploadPhoto.Url,
            PublicId = uploadPhoto.PublicId,
            Recipe = recipe
        };
        recipe.Image = uploadPhoto.Url;
        recipe.Photos = photo;

    }

}