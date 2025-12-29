using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Domain;

namespace Server.Features.Recipes.GetRecipe;

public class RecipeRepository : IRecipeRepository
{
    private readonly AppDbContext _context;

    public RecipeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Recipe?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Recipes
             .Include(r => r.ExtendedIngredients)
             .Include(r => r.Photos)
             .FirstOrDefaultAsync(x => x.Id == id);
    }


}
