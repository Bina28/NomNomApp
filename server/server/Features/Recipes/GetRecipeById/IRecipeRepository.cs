using server.Domain;

namespace Server.Features.Recipes.GetRecipeById;

public interface IRecipeRepository
{
    Task<Recipe?> GetByIdWithDetailsAsync(int id);
   
}
