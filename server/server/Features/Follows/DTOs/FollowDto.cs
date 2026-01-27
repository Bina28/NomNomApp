namespace Server.Features.Follows.DTOs;

public class FollowDto
{
    public required string Id { get; set; }
    public required string FollowerId { get; set; }
    public required string FollowingId { get; set; }
    public UserInfo? Follower { get; set; }
    public UserInfo? Following { get; set; }
}

public class UserInfo
{
    public required string UserName { get; set; }
}
