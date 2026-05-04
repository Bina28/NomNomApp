using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Features.Auth;
using Server.Features.Follows.DTOs;

namespace Server.Features.Follows;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class FollowsController : ControllerBase
{
    private readonly FollowsHandler _followsHandler;

    public FollowsController(FollowsHandler followsHandler)
    {
        _followsHandler = followsHandler;
    }


    [HttpPost("{userId}")]
    public async Task<ActionResult<bool>> FollowUser(string userId, CancellationToken ct)
    {
        var result = await _followsHandler.FollowUser(User.GetUserId(), userId, ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 400);
    }


    [HttpDelete("{userId}")]
    public async Task<ActionResult<bool>> UnfollowUser(string userId, CancellationToken ct)
    {
        var result = await _followsHandler.UnfollowUser(User.GetUserId(), userId, ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 400);
    }


    [HttpGet("followers")]
    public async Task<ActionResult<List<FollowerDto>>> GetFollowers(CancellationToken ct)
    {
        var result = await _followsHandler.GetFollowers(User.GetUserId(), ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 400);
    }

    [HttpGet("following")]
    public async Task<ActionResult<List<FollowingDto>>> GetFollowing(CancellationToken ct)
    {
        var result = await _followsHandler.GetFollowing(User.GetUserId(), ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 400);
    }


    [HttpGet("check/{userId}")]
    public async Task<ActionResult<bool>> IsFollowing(string userId, CancellationToken ct)
    {
        var result = await _followsHandler.IsFollowing(User.GetUserId(), userId, ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 400);
    }
}
