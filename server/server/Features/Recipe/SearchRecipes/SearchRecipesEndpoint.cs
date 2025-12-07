using Microsoft.AspNetCore.Mvc;

namespace server.Features.Recipe.SearchRecipes;


[ApiController]
[Route("api/[controller]")]
public class SearchRecipesEndpoint : ControllerBase
{
    private readonly SearchRecipesService _searchRecipesService;

    public SearchRecipesEndpoint(SearchRecipesService service)
    {
        _searchRecipesService = service;
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchRecipes( int numberOfcalories, int number)
    {
        var recipes = await _searchRecipesService.GetRecipes(numberOfcalories, number);
        return Ok(recipes);
    }
}
