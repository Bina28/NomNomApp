namespace Server.Features.Recipes.Infrastructure.Recipes.Spoonacular;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpoonacularApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SpoonacularSettings>(configuration.GetSection("SpoonacularApi"));

        services.AddHttpClient<IRecipeProvider, SpoonacularRecipeProvider>(c =>
        {
            c.BaseAddress = new Uri("https://api.spoonacular.com/");
        });

        return services;
    }
}
