namespace server.Features.Recipes.Services.RecipeApiClients;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpoonacularApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("SpoonacularApi").Get<SpoonacularSettings>();
        services.AddHttpClient<IRecipeApiClient, SpoonacularApiClient>(c =>
        {
            c.BaseAddress = new Uri("https://api.spoonacular.com/");
        });

        return services;
    }
}
