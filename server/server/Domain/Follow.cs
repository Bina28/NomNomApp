using server.Domain;

namespace Server.Domain;

public class Follow
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required string FollowerId { get; set; }
    public User Follower { get; set; } = null!;
    public required string FollowingId { get; set; }
    public User Following { get; set; } = null!;
}
