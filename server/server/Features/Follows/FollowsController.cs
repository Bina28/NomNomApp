using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Server.Features.Follows;

[ApiController]
[Route("api/[controller]")]
public class FollowsController : ControllerBase
{
    private readonly FollowsHandler _followsHandler;

    public FollowsController(FollowsHandler followsHandler)
    {
        _followsHandler = followsHandler;
    }

    [HttpPost("{userId}")]
    [Authorize]
    public async Task<IActionResult> FollowUser(string userId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == null)
        {
            return Unauthorized();
        }

        var result = await _followsHandler.FollowUser(currentUserId, userId);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpDelete("{userId}")]
    [Authorize]
    public async Task<IActionResult> UnfollowUser(string userId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == null)
        {
            return Unauthorized();
        }

        var result = await _followsHandler.UnfollowUser(currentUserId, userId);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("followers")]
    [Authorize]
    public async Task<IActionResult> GetFollowers()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == null)
        {
            return Unauthorized();
        }

        var result = await _followsHandler.GetFollowers(currentUserId);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("following")]
    [Authorize]
    public async Task<IActionResult> GetFollowing()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == null)
        {
            return Unauthorized();
        }

        var result = await _followsHandler.GetFollowing(currentUserId);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("check/{userId}")]
    [Authorize]
    public async Task<IActionResult> IsFollowing(string userId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == null)
        {
            return Unauthorized();
        }

        var result = await _followsHandler.IsFollowing(currentUserId, userId);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }
}
