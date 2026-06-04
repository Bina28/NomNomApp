namespace Server.Features.Recipes.FindByNutrients;

public record FindRecipesByNutrientsRequest(int MinimumCalories, int Number);

