using Microsoft.AspNetCore.Mvc;
using server.Features.Recipes.Spoonacular;

namespace server.Features.Recipes;

[ApiController]
[Route("api/[controller]")]
public class RecipeController : ControllerBase
{
    private readonly RecipeService _service;

    public RecipeController(RecipeService service)
    {
        _service = service;

    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRecipeById(int id)
    {
        var recipe = await _service.GetRecipeById(id);
        var dto = RecipeMapper.ToDto(recipe); 
        return Ok(dto);
    }

    [HttpGet("search")]
    public async Task<IActionResult> FindRecipesByNutrients(int numberOfcalories, int number)
    {
        var recipes = await _service.FindRecipesByNutrients(numberOfcalories, number);
        return Ok(recipes);
    }
}
