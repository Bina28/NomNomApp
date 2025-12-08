using Microsoft.Extensions.Options;
using server.Domain;
using System.Text.Json;

namespace server.Features.Recipes.GetRecipeDetails;

public class RecipeApiClient
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;
    public RecipeApiClient(IOptions<SpoonacularSettings> options, IHttpClientFactory clientFactory)
    {
        _apiKey = options.Value.ApiKey;
        _httpClient = clientFactory.CreateClient("SpoonacularClient");
    }

    public async Task<Recipe?> GetRecipeFromApi(int id)
    {
        var response = await _httpClient.GetAsync($"recipes/{id}/information?apiKey={_apiKey}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var apiRecipe = JsonSerializer.Deserialize<Recipe>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return apiRecipe;
    }

}
