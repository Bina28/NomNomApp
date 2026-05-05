using System.ComponentModel.DataAnnotations;

namespace server.Features.Recipes.CreateRecipe.DTOs;

public record RecipeDto
{
    [Required, MinLength(1), MaxLength(100)]    
    public required string Title { get; set; }
    [Required, MinLength(1), MaxLength(5000)]
    public required List<IngredientDto> Ingredients { get; set; }
}
    
    

