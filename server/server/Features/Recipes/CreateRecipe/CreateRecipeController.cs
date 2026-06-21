using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Features.Auth;
using Server.Features.Recipes.CreateRecipe.DTOs;

namespace Server.Features.Recipes.CreateRecipe;

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
        var result = await _service.CreateRecipeAsync(request, User.GetUserId(), ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 400);
    }
}
