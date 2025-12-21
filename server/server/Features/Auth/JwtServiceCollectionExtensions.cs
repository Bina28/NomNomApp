namespace server.Features.Auth;

public static class JwtServiceCollectionExtensions
{
    public static IServiceCollection AddJwt(this IServiceCollection services)
    {
        services
          .AddOptions<JwtOptions>()
          .BindConfiguration(JwtOptions.SectionName)
           .Validate(o => !string.IsNullOrWhiteSpace(o.Key),
            "JWT Key must be provided")
        .Validate(o => o.Key!.Length >= 64,
            "JWT Key must be at least 64 characters long")
        .Validate(o => !string.IsNullOrWhiteSpace(o.Issuer),
            "JWT Issuer must be provided")
        .Validate(o => !string.IsNullOrWhiteSpace(o.Audience),
            "JWT Audience must be provided")
        .Validate(o => o.ExpiryMinutes > 0,
            "JWT ExpiryMinutes must be greater than zero")
        .Validate(o => o.ExpiryMinutes <= 1440,
            "JWT ExpiryMinutes must not exceed 24 hours")
          .ValidateOnStart();

        return services;
    }

    public static IServiceCollection AddJwt(this IServiceCollection services, Action<JwtOptions> configure)
      => services
          .AddJwt()
          .Configure(configure);
}