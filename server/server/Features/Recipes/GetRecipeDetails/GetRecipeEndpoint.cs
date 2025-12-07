using Microsoft.AspNetCore.Mvc;

namespace server.Features.Recipes.GetRecipeDetails;

[ApiController]
[Route("api/[controller]")]
public class GetRecipeEndpoint : ControllerBase
{
    private readonly GetRecipeService _service;

    public GetRecipeEndpoint(GetRecipeService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRecipeById(int id)
    {
       var recipe = await _service.GetRecipeById(id);
        return Ok(recipe);
    }
}
