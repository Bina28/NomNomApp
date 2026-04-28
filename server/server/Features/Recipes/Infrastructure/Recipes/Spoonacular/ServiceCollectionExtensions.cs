using Microsoft.Extensions.Options;

namespace Server.Features.Recipes.Infrastructure.Recipes.Spoonacular;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpoonacularApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SpoonacularSettings>(configuration.GetSection("SpoonacularApi"));

        services.AddHttpClient<IRecipeProvider, SpoonacularRecipeProvider>((sp,c) =>
        {
            var settings = sp.GetRequiredService<IOptions<SpoonacularSettings>>().Value;
            c.BaseAddress = new Uri("https://api.spoonacular.com/");
            c.DefaultRequestHeaders.Add("X-Api-Key", settings.ApiKey);
        });

        return services;
    }
}
