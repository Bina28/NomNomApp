using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using server;
using server.Data;
using server.Features.Auth;
using server.Features.CreateRecipe;
using server.Features.Recipes.Photos;
using server.Features.Recipes.Spoonacular;
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
builder.Services.AddSpoonacularApiClient();

builder.Services.AddScoped<RecipeService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<CreateRecipeService>();
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
