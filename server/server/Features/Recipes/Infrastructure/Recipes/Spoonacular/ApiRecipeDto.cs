namespace Server.Features.Recipes.Infrastructure.Recipes.Spoonacular;

public record ApiRecipeDto
{
    public int Id { get; init; }
    public string? Title { get; init; }
    public string? Summary { get; init; }
    public string? Instructions { get; init; }
    public string? Image { get; init; }

    public List<ApiIngredientDto> ExtendedIngredients { get; init; } = [];
}
