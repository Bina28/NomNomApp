using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Features.Auth;
using Server.Features.Follows.CheckFollowStatus;
using Server.Features.Follows.Follow;
using Server.Features.Follows.GetFollowers;
using Server.Features.Follows.GetFollowing;
using Server.Features.Follows.Unfollow;

namespace Server.Features.Follows;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class FollowsController : ControllerBase
{
    private readonly FollowHandler _followHandler;
    private readonly UnfollowHandler _unfollowHandler;
    private readonly GetFollowersHandler _getFollowersHandler;
    private readonly GetFollowingHandler _getFollowingHandler;
    private readonly CheckFollowStatusHandler _checkFollowStatusHandler;


    public FollowsController(FollowHandler followHandler, UnfollowHandler unfollowHandler, GetFollowersHandler getFollowersHandler, GetFollowingHandler getFollowingHandler, CheckFollowStatusHandler followStatusHandler)
    {
        _followHandler = followHandler;
        _unfollowHandler = unfollowHandler;
        _getFollowingHandler = getFollowingHandler;
        _getFollowersHandler = getFollowersHandler;
        _checkFollowStatusHandler = followStatusHandler;
    }


    [HttpPost("{userId}")]
    public async Task<ActionResult<bool>> FollowUser(string userId, CancellationToken ct)
    {
        var result = await _followHandler.FollowUser(User.GetUserId(), userId, ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 400);
    }


    [HttpDelete("{userId}")]
    public async Task<ActionResult<bool>> UnfollowUser(string userId, CancellationToken ct)
    {
        var result = await _unfollowHandler.UnfollowUser(User.GetUserId(), userId, ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 400);
    }


    [HttpGet("followers")]
    public async Task<ActionResult<List<FollowerResponse>>> GetFollowers(CancellationToken ct)
    {
        var result = await _getFollowersHandler.GetFollowers(User.GetUserId(), ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 400);
    }

    [HttpGet("following")]
    public async Task<ActionResult<List<FollowingResponse>>> GetFollowing(CancellationToken ct)
    {
        var result = await _getFollowingHandler.GetFollowing(User.GetUserId(), ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 400);
    }


    [HttpGet("check/{userId}")]
    public async Task<ActionResult<bool>> IsFollowing(string userId, CancellationToken ct)
    {
        var result = await _checkFollowStatusHandler.IsFollowing(User.GetUserId(), userId, ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 400);
    }
}
