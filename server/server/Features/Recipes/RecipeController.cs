using Microsoft.AspNetCore.Mvc;
using server.Features.Recipes.FindByNutrients;
using server.Features.Recipes.GetRecipe;

namespace server.Features.Recipes;

[ApiController]
[Route("api/[controller]")]
public class RecipeController : ControllerBase
{
    private readonly GetRecipeByIdHandler _recipeByIdHandler;
    private readonly FindRecipesByNutrientsHandler _byNutrientsHandler;

    public RecipeController(GetRecipeByIdHandler recipeByIdHandler, FindRecipesByNutrientsHandler nutrientsHandler )
    {
        _recipeByIdHandler = recipeByIdHandler;
        _byNutrientsHandler = nutrientsHandler;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRecipeById(int id)
    {
        var recipe = await _recipeByIdHandler.GetRecipeById(id);
        var dto = RecipeMapper.ToDto(recipe); 
        return Ok(dto);
    }

    [HttpGet("search")]
    public async Task<IActionResult> FindRecipesByNutrients(int numberOfcalories, int number)
    {
        var recipes = await _byNutrientsHandler.FindRecipesByNutrients(numberOfcalories, number);
        return Ok(recipes);
    }
}
