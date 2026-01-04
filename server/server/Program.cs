using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using server.Data;
using server.Features.Auth;
using server.Features.Recipes.CreateRecipe;
using server.Features.Recipes.FindByNutrients;
using Server.Features.Recipes.GetRecipeById;
using Server.Features.Recipes.Infrastructure.Photo;
using Server.Features.Recipes.Infrastructure.Photo.CloudinaryPhoto;
using Server.Features.Recipes.Infrastructure.Recipes.Spoonacular;
using Server.Features.Recipes.SaveRecipe;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();


builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>>(sp =>
{
    var jwtOptions = sp.GetRequiredService<IOptions<JwtOptions>>().Value;

    return new ConfigureNamedOptions<JwtBearerOptions>(
        JwtBearerDefaults.AuthenticationScheme,
        options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtOptions.Key)
                )
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["access_token"];
                    return Task.CompletedTask;
                }
            };

        });
});


builder.Services.Configure<SpoonacularSettings>(
    builder.Configuration.GetSection("SpoonacularApi")
);
builder.Services.AddSpoonacularApiClient(builder.Configuration);
builder.Services.AddScoped<ISaveRecipeHandler, SaveRecipeHandler>();
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<IPhotoProvider, ClodinaryPhotoProvider>();
builder.Services.AddScoped<AuthHandler>();
builder.Services.AddScoped<CreateRecipeHandler>();
builder.Services.AddScoped<GetRecipeByIdHandler>();
builder.Services.AddScoped<FindRecipesByNutrientsHandler>();
builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddJwt();

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));


builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            "logs/log-.txt",
            rollingInterval: RollingInterval.Day
        );
});


var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors(x => x
    .WithOrigins("http://localhost:3000", "https://localhost:3000")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
);


app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
