using Microsoft.Extensions.Options;
using System.Text.Json;

namespace server.Features.Recipe.SearchRecipes;

public class SearchRecipesService
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    public SearchRecipesService(IOptions<SpoonacularSettings> options, IHttpClientFactory clientFactory)
    {
        _apiKey = options.Value.ApiKey;
        _httpClient = clientFactory.CreateClient("SpoonacularClient");

    }

    public async Task<List<SearchRecipesResponse>> GetRecipes(int numberOfCalories, int numberOfRecipes)
    {
        var url = $"recipes/findByNutrients?apiKey={_apiKey}&minCalories={numberOfCalories}&number={numberOfRecipes}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var jsonResult = await response.Content.ReadAsStringAsync();
        var results = JsonSerializer.Deserialize<List<SearchRecipesResponse>>(jsonResult,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
 );

        return results ?? new List<SearchRecipesResponse>();
    }
}
