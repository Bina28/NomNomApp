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

    public async Task<Result<RecipeDto>> CreateRecipe(RecipeDto request, string userId)
    {
        if (string.IsNullOrEmpty(request.Title))
            return Result<RecipeDto>.Fail("Title is required");

        var titleExists = await _context.UserRecipes.AnyAsync(i => i.Title == request.Title);

        if (titleExists)
            return Result<RecipeDto>.Fail("Recipe with the same title already exists");

        if (request.Ingredients.Count == 0)
            return Result<RecipeDto>.Fail("Ingredients are required");

        var recipe = CreateRecipeMapper.ToEntity(request, userId);
        await _context.AddAsync(recipe);
        await _context.SaveChangesAsync();

        return Result<RecipeDto>.Ok(request);
    }

}







