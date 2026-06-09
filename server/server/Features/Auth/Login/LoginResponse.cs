namespace Server.Features.Auth.Login;

public record LoginResponse(string AccessToken, string RefreshToken);
