using Server.Domain;

namespace server.Domain;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }

    //navigation properties
    public List<Comment> Comments { get; set; } = [];
    public List<Follow> Followers { get; set; } = [];
    public List<Follow> Following { get; set; } = [];
}
