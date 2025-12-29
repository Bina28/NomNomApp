namespace server.Features.Recipes.Services.RecipeApiClients;

public class ApiRecipeDto {
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? Instructions { get; set; }
    public string? Image { get; set; }

    public List<ApiIngredientDto>? ExtendedIngredients { get; set; }
}