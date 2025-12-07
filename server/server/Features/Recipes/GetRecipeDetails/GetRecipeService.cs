using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using server.Data;
using server.Domain;
using System.Text.Json;


namespace server.Features.Recipes.GetRecipeDetails;

public class GetRecipeService
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _context;

    public GetRecipeService(IOptions<SpoonacularSettings> options, IHttpClientFactory clientFactory, AppDbContext context)
    {
        _apiKey = options.Value.ApiKey;
        _httpClient = clientFactory.CreateClient("SpoonacularClient");
        _context = context;
    }

    public async Task<RecipeDto> GetRecipeById(int id)
    {
        var recipeInDb = _context.Recipes
            .Include(r => r.ExtendedIngredients)
            .FirstOrDefault(x => x.Id == id);

        if (recipeInDb != null)
        {
            return new RecipeDto(
                recipeInDb.Title,
                recipeInDb.Summary,
                recipeInDb.Instructions,
                recipeInDb.ExtendedIngredients?.Select(i => i.Original ?? "").ToList()
            );
        }


        var response = await _httpClient.GetAsync($"recipes/{id}/information?apiKey={_apiKey}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        Console.WriteLine(json);
        var apiRecipe = JsonSerializer.Deserialize<Recipe>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Console.WriteLine(apiRecipe);

        var ingredients = new List<Ingredient>();
        if (apiRecipe.ExtendedIngredients != null)
        {
            foreach (var apiIng in apiRecipe.ExtendedIngredients)
            {
              
                var ingredient = _context.Ingredients.FirstOrDefault(x => x.Id == apiIng.Id);
                if (ingredient == null)
                {
                    ingredient = new Ingredient { Id = apiIng.Id, Original = apiIng.Original };
                    _context.Ingredients.Add(ingredient);
                }
                ingredients.Add(ingredient); 
            }
        }

        var recipeEntity = new Recipe
        {
            Id = apiRecipe.Id,
            Title = apiRecipe.Title,
            Summary = apiRecipe.Summary,
            Instructions = apiRecipe.Instructions,
            ExtendedIngredients = apiRecipe.ExtendedIngredients?.Select(i => new Ingredient { Id = i.Id, Original = i.Original }).ToList()
        };

        _context.Recipes.Add(recipeEntity);
        await _context.SaveChangesAsync();

        return new RecipeDto(
         recipeEntity.Title,
         recipeEntity.Summary,
         recipeEntity.Instructions,
         recipeEntity.ExtendedIngredients?
             .Select(i => i.Original ?? "")
             .ToList() 
     );

    }
}
