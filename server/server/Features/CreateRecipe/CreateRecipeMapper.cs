using server.Domain;


namespace server.Features.CreateRecipe;

public class CreateRecipeMapper
{

    public static UserRecipe ToEntity(RecipeDto request, string userId)
    {
        return new UserRecipe
        {
            Title = request.Title,
            UserId = userId,
            Ingredients = [.. request.Ingredients.Select(i => new UserRecipeIngredients
            {
                Name = i.Name,
                Amount = i.Amount
            })],

        };
    }



}
