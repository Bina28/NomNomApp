namespace server.Features.CreateRecipe;

public record RecipeDto(string Title,  List<IngredientDto> Ingredients);

