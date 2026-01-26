using Microsoft.AspNetCore.Mvc;

namespace Server.Features.Follows;

[ApiController]
[Route("api/[controller]")]

public class FollowsController: ControllerBase
{private readonly FollowsHandler _followsHandler;

    public FollowsController(FollowsHandler followsHandler)
    {
        _followsHandler = followsHandler;
    }

    [HttpPost("/{userId}")]
    public async Task<IActionResult> FollowUser(string userId)
    {
        var result = await _followsHandler.FollowUser(userId);
        return Ok();
    }


    [HttpDelete("/{userId}")]
    public async Task<IActionResult> UnfollowUser(string userId)
    {
        var result = await _followsHandler.UnfollowUser(userId);
        return Ok();
    }

    [HttpGet("/followers")]
    public async Task<IActionResult> GetFollowers()
    {
        var result = await _followsHandler.GetFollowers();
        return Ok();
    }

    [HttpGet("/following")]
    public async Task<IActionResult> GetFollowing()
    {
        var result = await _followsHandler.GetFollowing();
        return Ok();
    }

    [HttpGet("/check/{userid}")]
    public async Task<IActionResult> IsFollowing(string userId)
    {
        var result = await _followsHandler.IsFollowing(userId);
        return Ok();
    }


}
