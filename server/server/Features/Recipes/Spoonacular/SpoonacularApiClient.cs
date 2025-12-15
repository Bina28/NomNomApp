using Microsoft.Extensions.Options;
using server.Domain;
using server.Features.Recipes.Spoonacular.DTOs;
using System.Text.Json;

namespace server.Features.Recipes.Spoonacular;

public class SpoonacularApiClient
{
    private readonly string _apiKey;
    private readonly HttpClient _client;
    public SpoonacularApiClient(IOptions<SpoonacularSettings> options, HttpClient client)
    {
        _apiKey = options.Value.ApiKey;
        _client = client;
    }

    public async Task<Recipe?> GetRecipeById(int id)
    {
        var response = await _client.GetAsync($"recipes/{id}/information?apiKey={_apiKey}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var apiRecipe = JsonSerializer.Deserialize<Recipe>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return apiRecipe;
    }

    public async Task<List<SpoonacularRecipeResponse>> FindRecipesByNutrients(int minCalories,
    int maxResults)
    {
        var url = $"recipes/findByNutrients?apiKey={_apiKey}&minCalories={minCalories}&number={maxResults}";
        var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var jsonResult = await response.Content.ReadAsStringAsync();
        var results = JsonSerializer.Deserialize<List<SpoonacularRecipeResponse>>(jsonResult,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
 );

        return results ?? new List<SpoonacularRecipeResponse>();
    }


}