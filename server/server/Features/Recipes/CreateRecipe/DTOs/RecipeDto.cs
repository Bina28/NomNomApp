namespace server.Features.Recipes.CreateRecipe.DTOs;

public record RecipeDto(string Title,  List<IngredientDto> Ingredients);

