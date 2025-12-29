using server.Domain;

namespace Server.Features.Recipes.GetRecipe;

public interface IRecipeRepository
{
    Task<Recipe?> GetByIdWithDetailsAsync(int id);
   
}
