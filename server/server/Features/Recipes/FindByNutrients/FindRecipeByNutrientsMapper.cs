using server.Domain;
using server.Features.Recipes.FindByNutrients;

namespace Server.Features.Recipes.FindByNutrients;

public class FindRecipeByNutrientsMapper
{

    public static FindRecipesByNutrientsResponse ToResponse(Recipe apiRecipe)
    {
        return new FindRecipesByNutrientsResponse(
            Id: apiRecipe.Id,
            Title: apiRecipe.Title    
        );
    }

}
