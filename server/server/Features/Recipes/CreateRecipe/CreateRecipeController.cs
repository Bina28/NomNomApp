using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Features.Auth;
using server.Features.Recipes.CreateRecipe.DTOs;

namespace server.Features.Recipes.CreateRecipe;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CreateRecipeController : ControllerBase
{
    private readonly CreateRecipeHandler _service;

    public CreateRecipeController(CreateRecipeHandler service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RecipeDto request, CancellationToken ct)
    {
        var result = await _service.CreateRecipe(request, User.GetUserId(), ct);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }
}
