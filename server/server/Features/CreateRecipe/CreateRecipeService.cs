using server.Data;
using server.Features.Shared;

namespace server.Features.CreateRecipe;

public class CreateRecipeService
{
    private readonly AppDbContext _context;

    public CreateRecipeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ResultValue<RecipeDto>> CreateRecipe(RecipeDto request, string userId)
    {
        if (string.IsNullOrEmpty(request.Title))
            return Result.Fail<RecipeDto>("Title is required");

        if (request.Ingredients.Count == 0)
            return Result.Fail<RecipeDto>("Ingredients are required");

        var recipe = CreateRecipeMapper.ToEntity(request, userId);
        await _context.AddAsync(recipe);
        await _context.SaveChangesAsync();
             
        return Result.Success(request);
    }

}







