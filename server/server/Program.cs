using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Exceptions;
using Server.Data;
using Server.Features.Auth.Infrastructure.Jwt;
using Server.Features.Auth.Infrastructure.Password;
using Server.Features.Auth.RefreshTokens;
using Server.Features.Auth.GetAllUsers;
using Server.Features.Auth.GetCurrentUser;
using Server.Features.Auth.Login;
using Server.Features.Auth.Register;
using Server.Features.Comments.DeleteComment;
using Server.Features.Comments.GetComments;
using Server.Features.Comments.GetCommentsScore;
using Server.Features.Comments.PostComment;
using Server.Features.Follows.CheckFollowStatus;
using Server.Features.Follows.Follow;
using Server.Features.Follows.GetFollowers;
using Server.Features.Follows.GetFollowing;
using Server.Features.Follows.Unfollow;
using Server.Features.Recipes.CreateRecipe;
using Server.Features.Recipes.FindByNutrients;
using Server.Features.Recipes.GetRecipeById;
using Server.Features.Recipes.Infrastructure.Photo;
using Server.Features.Recipes.Infrastructure.Photo.CloudinaryPhoto;
using Server.Features.Recipes.Infrastructure.Recipes.Spoonacular;
using Server.Features.Recipes.SaveRecipe;
using Server.Features.Sse;
using Server.Middleware;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddCors();

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

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("auth", httpContext =>
    {
        var key = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
        {
            Window = TimeSpan.FromMinutes(1),
            PermitLimit = 5,
            QueueLimit = 0
        });
    });

    options.OnRejected = async (context, ct) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter =
                ((int)retryAfter.TotalSeconds).ToString();
        }

        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        var problemDetailsService = context.HttpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = context.HttpContext,
            ProblemDetails =
            {
                Status = 429,
                Title = "Too many requests",
                Detail = "Please slow down and try again later.",
                Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}"
            }
        });
    };
});


builder.Services.Configure<SpoonacularSettings>(
    builder.Configuration.GetSection("SpoonacularApi")
);
builder.Services.AddSpoonacularApiClient(builder.Configuration);
builder.Services.AddScoped<ISaveRecipeHandler, SaveRecipeHandler>();
builder.Services.AddScoped<IPhotoProvider, ClodinaryPhotoProvider>();
builder.Services.AddScoped<LoginHandler>();
builder.Services.AddScoped<RegisterHandler>();
builder.Services.AddScoped<GetCurrentUserHandler>();
builder.Services.AddScoped<GetAllUsersHandler>();
builder.Services.AddScoped<RegisterMapper>();
builder.Services.AddScoped<CreateRecipeHandler>();
builder.Services.AddScoped<GetRecipeByIdHandler>();
builder.Services.AddScoped<FindRecipesByNutrientsHandler>();
builder.Services.AddSingleton<SseConnectionManager>();
builder.Services.AddScoped<PostCommentHandler>();
builder.Services.AddScoped<DeleteCommentHandler>();
builder.Services.AddScoped<GetCommentsHandler>();
builder.Services.AddScoped<GetCommentsScoreHandler>();
builder.Services.AddScoped<FollowHandler>();
builder.Services.AddScoped<UnfollowHandler>();
builder.Services.AddScoped<GetFollowersHandler>();
builder.Services.AddScoped<GetFollowingHandler>();
builder.Services.AddScoped<CheckFollowStatusHandler>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddJwt();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));


builder.Services.AddHealthChecks();

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithEnvironmentName()
        .Enrich.WithExceptionDetails()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {CorrelationId} {Message:lj}{NewLine}{Exception}")
        .WriteTo.File(
            "logs/log-.txt",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {CorrelationId} {Message:lj}{NewLine}{Exception}"
        );
});


var app = builder.Build();

app.MapHealthChecks("/api/health");

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

if (!app.Environment.IsProduction())
    app.UseHttpsRedirection();

app.UseCors(x => x
    .WithOrigins("http://localhost:3000", "https://localhost:3000", "http://localhost:8081")
    .WithHeaders("Content-Type", "Authorization")
    .WithMethods("GET", "POST", "PUT", "DELETE")
    .AllowCredentials()
);


app.UseAuthentication();
app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();
