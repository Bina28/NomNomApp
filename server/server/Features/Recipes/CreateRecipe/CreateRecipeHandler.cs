using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Features.Recipes.CreateRecipe.DTOs;
using server.Features.Shared;

namespace server.Features.Recipes.CreateRecipe;

public class CreateRecipeHandler
{
    private readonly AppDbContext _context;

    public CreateRecipeHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ResultValue<RecipeDto>> CreateRecipe(RecipeDto request, string userId)
    {
        if (string.IsNullOrEmpty(request.Title))
            return Result.Fail<RecipeDto>("Title is required");

        var titleExists = await _context.UserRecipes.AnyAsync(i => i.Title == request.Title);

        if (titleExists)
            return Result.Fail<RecipeDto>("Recipe with the same title already exists");

        if (request.Ingredients.Count == 0)
            return Result.Fail<RecipeDto>("Ingredients are required");

        var recipe = CreateRecipeMapper.ToEntity(request, userId);
        await _context.AddAsync(recipe);
        await _context.SaveChangesAsync();
             
        return Result.Success(request);
    }

}







