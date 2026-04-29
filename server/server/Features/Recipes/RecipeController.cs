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

    // TODO: bør være ActionResult<RecipeResponse> for Swagger-støtte
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRecipeById(int id, CancellationToken ct)
    {
        var result = await _recipeByIdHandler.GetRecipeById(id, ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 404);
    }

    // TODO: bør være ActionResult<List<FindRecipesByNutrientsResponse>> for Swagger-støtte
    [HttpGet("search")]
    public async Task<IActionResult> FindRecipesByNutrients([FromQuery] FindRecipesByNutrientsRequest request, CancellationToken ct)
    {
        var result = await _byNutrientsHandler.FindRecipesByNutrients(request, ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 404);
    }
}
