namespace Server.Features.Auth.Shared;

public record UserResponse
{
    public required string Id { get; init; }
    public required string UserName { get; init; }
    public required string Email { get; init; }
}
