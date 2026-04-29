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


    public async Task<Recipe> SaveRecipe(Recipe recipe, CancellationToken ct = default)
    {
        recipe.ExtendedIngredients = await ResolveIngredients(recipe, ct);
        await HandleImage(recipe, ct);

        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync(ct);

        return recipe;
    }

    private async Task<List<Ingredient>> ResolveIngredients(Recipe recipe, CancellationToken ct = default)
    {
        var result = new List<Ingredient>();
        var distinctIngredients = recipe.ExtendedIngredients
            .Where(i => !string.IsNullOrWhiteSpace(i.Original))
            .Select(i => i.Original?.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase);



        foreach (var original in distinctIngredients)
        {
            var ingredient = _context.Ingredients.Local.FirstOrDefault(x => string.Equals(x.Original, original, StringComparison.OrdinalIgnoreCase))
                ?? await _context.Ingredients.FirstOrDefaultAsync(x => x.Original != null && x.Original.ToLower() == original!.ToLower(), ct);

            if (ingredient == null)
            {
                ingredient = new Ingredient { Original = original };

                _context.Ingredients.Add(ingredient);
            }

            result.Add(ingredient);

        }
        return result;
    }

    private async Task HandleImage(Recipe recipe, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(recipe.Image)) return;

        var uploadPhoto = await _photoService.UploadImgFromUrl(recipe.Image, ct);
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