namespace server.Features.Auth.DTOs;

public record RegisterRequest(string UserName, string Email, string Password);
