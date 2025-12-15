using Microsoft.EntityFrameworkCore;
using server.Data;

namespace server.Features.Us;

public class CreateRecipeService
{
    private readonly AppDbContext _context;

    public CreateRecipeService(AppDbContext context)
    {
        _context = context;
    }

    //public async Task<UserRecipe> Create(UserRecipeDto recipeDto)
    //{
    //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == recipeDto.UserId) 
    //        ?? throw new Exception("User not found");

    //    var recipe = new UserRecipe
    //    {
    //        Title = recipeDto.Title,
    //        User = user,
    //        Instructions = [.. recipeDto.Instructions.Select(i => new UserInstruction
    //        {
    //            StepDescription = i.StepDescription,
    //            Recipe = null!
    //        })],
    //        Ingredients = [.. recipeDto.Ingredients.Select(ing => new UserIngredient
    //        {
    //            Name = ing.Name,
    //            Amount = ing.Amount,
    //            Recipe = null! 
    //        })]
    //    };

    //    await _context.SaveChangesAsync();
    //    return recipe;
    //}
}
public class User
{
    public required string Id { get; set; }
}
public class UserRecipe
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Title { get; set; }
    public  List<UserInstruction> Instructions { get; set; } = [];
    public  List<UserIngredient> Ingredients { get; set; } = [];
    public User User { get; set; }
}


public class UserIngredient
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Name { get; set; }
    public required string Amount { get; set; }
    public UserRecipe Recipe { get; set; }

}

public class UserInstruction
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string StepDescription { get; set; }
    public  UserRecipe Recipe { get; set; }
}

public class UserRecipeDto
{
    public required string Title { get; set; }
    public required List<UserInstruction> Instructions { get; set; } = [];
    public required List<UserIngredient> Ingredients { get; set; } = [];
    public required string UserId { get; set; }
}


public record IngredientDto(string Name, string Amount);

public record InstructionDto(string StepDescription);

