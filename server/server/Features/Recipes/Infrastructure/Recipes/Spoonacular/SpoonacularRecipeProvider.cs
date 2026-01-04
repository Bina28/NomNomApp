using Microsoft.Extensions.Options;
using server.Domain;
using Server.Features.Recipes.FindByNutrients;
using System.Text.Json;

namespace Server.Features.Recipes.Infrastructure.Recipes.Spoonacular;

public class SpoonacularRecipeProvider : IRecipeProvider
{
    private readonly string _apiKey;
    private readonly HttpClient _client;
    public SpoonacularRecipeProvider(IOptions<SpoonacularSettings> options, HttpClient client)
    {
        _apiKey = options.Value.ApiKey;
        _client = client;
    }

    public async Task<Recipe?> GetRecipeById(int id)
    {
        var response = await _client.GetAsync($"recipes/{id}/information?apiKey={_apiKey}");
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        var apiRecipe = JsonSerializer.Deserialize<ApiRecipeDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (apiRecipe == null) return null;
        return MapToDomain(apiRecipe);
    }

    public async Task<List<Recipe>?> FindRecipesByNutrients(FindRecipesByNutrientsRequest request)
    {
        var url = $"recipes/findByNutrients?apiKey={_apiKey}&minCalories={request.Calories}&number={request.Number}";
        var response = await _client.GetAsync(url);
        if (!response.IsSuccessStatusCode)  return null;
        var jsonResult = await response.Content.ReadAsStringAsync();
        var results = JsonSerializer.Deserialize<List<ApiRecipeDto>>(jsonResult,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
 );
        if (results == null) return []; ;
        return [.. results.Select(MapToDomain)];
    }

    private static Recipe MapToDomain(ApiRecipeDto dto)
    {
        return new Recipe
        {
            Id = dto.Id,
            Title = dto.Title ?? string.Empty,
            Summary = dto.Summary ?? string.Empty,
            Instructions = dto.Instructions ?? string.Empty,
            Image = dto.Image ?? string.Empty,
            ExtendedIngredients = dto.ExtendedIngredients?
            .Select(i => new Ingredient
            {
            Original = i.Original ?? string.Empty
            })
           .ToList() ?? [],
        };
    }

}
