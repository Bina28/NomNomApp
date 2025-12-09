using Microsoft.AspNetCore.Mvc;

namespace server.Features.Users;

[ApiController]
[Route("me/[controller]")]
public class CreateRecipeEndpoint : ControllerBase
{
    private readonly CreateRecipeService _service;

    public CreateRecipeEndpoint(CreateRecipeService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRecipe(UserRecipeDto userRecipe) {
        var result = await _service.Create(userRecipe);
        return Ok(result);
    }
}
