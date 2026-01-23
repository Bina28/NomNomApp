using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Domain;
using Server.Features.Recipes.Infrastructure.Photo;

namespace Server.Features.Recipes.SaveRecipe;

public class SaveRecipeHandler : ISaveRecipeHandler
{
    private readonly AppDbContext _context;
    private readonly IPhotoProvider _photoService;

    public SaveRecipeHandler(AppDbContext context, IPhotoProvider provider)
    {
        _context = context;
        _photoService = provider;
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
        var result = new List<Ingredient>();
        var distinctIngredients = recipe.ExtendedIngredients
            .Where(i => !string.IsNullOrWhiteSpace(i.Original))
            .Select(i => i.Original?.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase);



        foreach (var original in distinctIngredients)
        {
            var ingredient = _context.Ingredients.Local.FirstOrDefault(x => string.Equals(x.Original, original, StringComparison.OrdinalIgnoreCase))
                ?? await _context.Ingredients.FirstOrDefaultAsync(x => x.Original != null && x.Original.ToLower() == original!.ToLower());

            if (ingredient == null)
            {
                ingredient = new Ingredient { Original = original };

                _context.Ingredients.Add(ingredient);
            }

            result.Add(ingredient);

        }
        return result;
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