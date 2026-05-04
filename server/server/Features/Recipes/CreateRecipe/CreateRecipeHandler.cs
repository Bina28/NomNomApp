using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Features.Recipes.CreateRecipe.DTOs;
using server.Features.Shared;

namespace server.Features.Recipes.CreateRecipe;

public class CreateRecipeHandler
{
    private readonly AppDbContext _context;
    private readonly ILogger<CreateRecipeHandler> _logger;

    public CreateRecipeHandler(AppDbContext context, ILogger<CreateRecipeHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<RecipeDto>> CreateRecipe(RecipeDto request, string userId, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(request.Title))
        {
            _logger.LogWarning("User {UserId} attempted to create a recipe without a title", userId);
            return Result<RecipeDto>.Fail("Title is required");
        }


        var titleExists = await _context.UserRecipes.AnyAsync(i => i.Title == request.Title, ct);

        if (titleExists)
        {
            _logger.LogWarning("User {UserId} attempted to create a recipe with a duplicate title: {Title}", userId, request.Title);
            return Result<RecipeDto>.Fail("Recipe with the same title already exists");
        }

        if (request.Ingredients.Count == 0)
        {
            _logger.LogWarning("User {UserId} attempted to create a recipe without ingredients", userId);
            return Result<RecipeDto>.Fail("Ingredients are required");
        }


        var recipe = CreateRecipeMapper.ToEntity(request, userId);
        await _context.AddAsync(recipe, ct);
        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Created recipe {RecipeId} titled {Title} by user {UserId}", recipe.Id, recipe.Title, userId);

        return Result<RecipeDto>.Ok(request);
    }

}







