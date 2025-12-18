using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using server.Data;
using server.Features.Auth;
using server.Features.Recipes.CreateRecipe;
using server.Features.Recipes.FindByNutrients;
using server.Features.Recipes.GetRecipe;
using server.Features.Recipes.SaveRecipe;
using server.Features.Recipes.Services.Photo;
using server.Features.Recipes.Services.RecipeApiClients;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
            )
        };
    });

builder.Services.Configure<SpoonacularSettings>(
    builder.Configuration.GetSection("SpoonacularApi")
);
builder.Services.AddSpoonacularApiClient(builder.Configuration);

builder.Services.AddScoped<AuthHandler>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<CreateRecipeHandler>();
builder.Services.AddScoped<GetRecipeByIdHandler>();
builder.Services.AddScoped<SaveRecipeFromApiHandler>();
builder.Services.AddScoped<FindRecipesByNutrientsHandler>();
builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddSingleton<JwtService>();

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod()
.WithOrigins("http://localhost:3000", "https://localhost:3000"));

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
