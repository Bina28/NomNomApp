using Server.Domain;
using Server.Features.Recipes.CreateRecipe.DTOs;

namespace Server.Features.Recipes.CreateRecipe;

public class CreateRecipeMapper
{

    public static Recipe ToEntity(RecipeDto request, string userId)
    {
        return new Recipe
        {
            Title = request.Title,
            UserId = userId,
            UserIngredients = [.. request.Ingredients.Select(i => new RecipeIngredient
            {
                Name = i.Name,
                Amount = i.Amount
            })],
        };
    }

}
