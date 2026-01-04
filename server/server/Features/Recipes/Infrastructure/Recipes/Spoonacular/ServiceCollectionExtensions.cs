namespace Server.Features.Recipes.Infrastructure.Recipes.Spoonacular;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpoonacularApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("SpoonacularApi").Get<SpoonacularSettings>();
        services.AddHttpClient<IRecipeProvider, SpoonacularRecipeProvider>(c =>
        {
            c.BaseAddress = new Uri("https://api.spoonacular.com/");
        });

        return services;
    }
}
