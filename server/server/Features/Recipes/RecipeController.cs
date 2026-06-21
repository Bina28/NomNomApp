using Microsoft.AspNetCore.Mvc;
using Server.Features.Recipes.FindByNutrients;
using Server.Features.Recipes.GetRecipeById;
using Server.Features.Shared;

namespace Server.Features.Recipes;

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
    public async Task<ActionResult<RecipeResponse>> GetRecipeById(int id, CancellationToken ct)
    {
        var result = await _recipeByIdHandler.GetRecipeByIdAsync(id, ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 404);
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<FindRecipesByNutrientsResponse>>> FindRecipesByNutrients([FromQuery] FindRecipesByNutrientsRequest request, [FromQuery] PageParameters parameters,CancellationToken ct)
    {
        var result = await _byNutrientsHandler.FindRecipesByNutrientsAsync(request, parameters, ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 404);
    }
}
