
namespace Server.Features.Follows;

public class FollowsHandler
{
    public async Task FollowUser(string userId)
    {
        await _sseManager.SendEventAsync(targetUserId, "new_follower", new
        {
            followerId = currentUser.Id,
            followerName = currentUser.UserName
        });
    }

    public async Task GetFollowers()
    {

    }

    public async Task IsFollowing(string userId)
    {

    }

    public async Task UnfollowUser(string userId)
    {

    }
}
