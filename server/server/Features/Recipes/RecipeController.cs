using Microsoft.AspNetCore.Mvc;
using server.Features.Recipes.FindByNutrients;
using Server.Features.Recipes.FindByNutrients;
using Server.Features.Recipes.GetRecipeById;

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
    public async Task<ActionResult<RecipeResponse>> GetRecipeById(int id, CancellationToken ct)
    {
        var result = await _recipeByIdHandler.GetRecipeById(id, ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 404);
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<FindRecipesByNutrientsResponse>>> FindRecipesByNutrients([FromQuery] FindRecipesByNutrientsRequest request, CancellationToken ct)
    {
        var result = await _byNutrientsHandler.FindRecipesByNutrients(request, ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 404);
    }
}
