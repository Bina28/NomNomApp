using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace server.Features.CreateRecipe;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CreateRecipeController : ControllerBase
{
    private readonly CreateRecipeService _service;

    public CreateRecipeController(CreateRecipeService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRecipe([FromBody] RecipeDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized("User not found");
        }
        var result = await _service.CreateRecipe(request, userId);
        return result.Ok ? Ok(result.Data) : BadRequest(result.Error);
    }
}
