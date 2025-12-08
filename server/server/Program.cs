using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using server;
using server.Data;
using server.Features.Recipes.GetRecipeDetails;
using server.Features.Recipes.SearchRecipes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.Configure<SpoonacularSettings>(
    builder.Configuration.GetSection("SpoonacularApi")
);


builder.Services.AddHttpClient("SpoonacularClient", (serviceProvider, client) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<SpoonacularSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddScoped<SearchRecipesService>();
builder.Services.AddScoped<GetRecipeService>();
builder.Services.AddScoped<RecipeApiClient>();
builder.Services.AddScoped<RecipeSaver>();
builder.Services.AddScoped<RecipeProvider>();

var app = builder.Build();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
