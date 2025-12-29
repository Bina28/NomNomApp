using Microsoft.AspNetCore.Mvc;
using server.Features.Recipes.FindByNutrients;
using server.Features.Recipes.GetRecipe;
using Server.Features.Recipes.FindByNutrients;

namespace server.Features.Recipes;

[ApiController]
[Route("api/[controller]")]
public class RecipeController : ControllerBase
{
    private readonly GetRecipeByIdHandler _recipeByIdHandler;
    private readonly FindRecipesByNutrientsHandler _byNutrientsHandler;

    public RecipeController(GetRecipeByIdHandler recipeByIdHandler, FindRecipesByNutrientsHandler nutrientsHandler)
    {
        _recipeByIdHandler = recipeByIdHandler;
        _byNutrientsHandler = nutrientsHandler;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRecipeById(int id)
    {
        var result = await _recipeByIdHandler.GetRecipeById(id);

        return result.Success ? Ok(result.Data) : NotFound(result.Error);
    }

    [HttpGet("search")]
    public async Task<IActionResult> FindRecipesByNutrients([FromQuery] FindRecipesByNutrientsRequest request)
    {
        var result = await _byNutrientsHandler.FindRecipesByNutrients(request);
        return result.Success ? Ok(result.Data) : NotFound(result.Error);
    }
}
