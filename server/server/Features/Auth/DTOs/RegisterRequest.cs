using System.ComponentModel.DataAnnotations;

namespace server.Features.Auth.DTOs;

public record RegisterRequest
{
    [Required, MinLength(3), MaxLength(30)]
    public required string UserName { get; init; }

    [Required, EmailAddress]
    public required string Email { get; init; }

    [Required, MinLength(6), MaxLength(100)]
    public required string Password { get; init; }
}
