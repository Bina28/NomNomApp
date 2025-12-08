using server.Data;
using server.Domain;

namespace server.Features.Recipes.GetRecipeDetails;

public class RecipeSaver
{
    private readonly AppDbContext _context;

    public RecipeSaver(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Recipe> SaveRecipe(Recipe apiRecipe)
    {
        var ingredients = new List<Ingredient>();
        if (apiRecipe.ExtendedIngredients != null)
        {
            foreach (var apiIng in apiRecipe.ExtendedIngredients)
            {

                var ingredient = _context.Ingredients.FirstOrDefault(x => x.Id == apiIng.Id);
                if (ingredient == null)
                {
                    ingredient = new Ingredient
                    {
                        Id = apiIng.Id,
                        Original = apiIng.Original
                    };
                    _context.Ingredients.Add(ingredient);
                }
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
        await _context.SaveChangesAsync();

        return recipe;
    }
}
