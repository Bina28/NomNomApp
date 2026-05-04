namespace Server.Features.Follows.DTOs;

public record FollowerDto(string Id, string FollowerId, string UserName);

public record FollowingDto(string Id, string FollowingId, string UserName);
