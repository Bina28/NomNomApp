namespace server.Features.Recipes.Spoonacular;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpoonacularApiClient(this IServiceCollection services)
    {
        services.AddHttpClient<SpoonacularApiClient>(c =>
        {
            c.BaseAddress = new Uri("https://api.spoonacular.com/");
        });

        return services;
    }
}
