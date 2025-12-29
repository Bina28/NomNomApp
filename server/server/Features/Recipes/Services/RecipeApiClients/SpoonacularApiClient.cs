using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Server.Features.Recipes.FindByNutrients;
using System.Text.Json;

namespace server.Features.Recipes.Services.RecipeApiClients;

public class SpoonacularApiClient : IRecipeApiClient
{
    private readonly string _apiKey;
    private readonly HttpClient _client;
    public SpoonacularApiClient(IOptions<SpoonacularSettings> options, HttpClient client)
    {
        _apiKey = options.Value.ApiKey;
        _client = client;
    }

    public async Task<ApiRecipeDto?> GetRecipeById(int id)
    {
        var response = await _client.GetAsync($"recipes/{id}/information?apiKey={_apiKey}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var apiRecipe = JsonSerializer.Deserialize<ApiRecipeDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return apiRecipe;
    }

    public async Task<List<ApiRecipeDto>> FindRecipesByNutrients( FindRecipesByNutrientsRequest request)
    {
        var url = $"recipes/findByNutrients?apiKey={_apiKey}&minCalories={request.Calories}&number={request.Number}";
        var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var jsonResult = await response.Content.ReadAsStringAsync();
        var results = JsonSerializer.Deserialize<List<ApiRecipeDto>>(jsonResult,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
 );

        return results ?? new List<ApiRecipeDto>();
    }


}