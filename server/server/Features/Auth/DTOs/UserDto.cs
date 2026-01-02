using System;

namespace Server.Features.Auth.DTOs;

public class UserDto
{
  public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string UserName { get; set; }
    public required string Email { get; set; }

}
