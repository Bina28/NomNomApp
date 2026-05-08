using System.ComponentModel.DataAnnotations;

namespace Server.Features.Auth.Login;

public class LoginRequest
{
    [Required, EmailAddress]
    public required string Email { get; init; }

    [Required]
    public required string Password { get; init; }
}
