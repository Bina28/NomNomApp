using Microsoft.EntityFrameworkCore;
using server;
using server.Data;
using server.Features.Recipes.Spoonacular;

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
builder.Services.AddSpoonacularApiClient();


//builder.Services.AddHttpClient("SpoonacularClient", (serviceProvider, client) =>
//{
//    var settings = serviceProvider.GetRequiredService<IOptions<SpoonacularSettings>>().Value;
//    client.BaseAddress = new Uri(settings.BaseUrl);
//    client.DefaultRequestHeaders.Add("Accept", "application/json");
//});


builder.Services.AddScoped<RecipeService>();




var app = builder.Build();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod()
.WithOrigins("http://localhost:3000", "https://localhost:3000"));

app.Run();
