using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Features.Recipes.CreateRecipe.DTOs;
using System.Security.Claims;

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
    public async Task<IActionResult> Create([FromBody] RecipeDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized("User not found");
        }
        var result = await _service.CreateRecipe(request, userId);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }
}
