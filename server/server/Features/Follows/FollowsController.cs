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

    // TODO: bør være ActionResult<FollowDto> for Swagger-støtte
    [HttpPost("{userId}")]
    [Authorize]
    public async Task<IActionResult> FollowUser(string userId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == null)
        {
            return Problem(detail: "User not found", statusCode: 401);
        }

        var result = await _followsHandler.FollowUser(currentUserId, userId);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 400);
    }

    // IActionResult: returnerer bare true/false
    [HttpDelete("{userId}")]
    [Authorize]
    public async Task<IActionResult> UnfollowUser(string userId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == null)
            return Problem(detail: "User not authenticated", statusCode: 401);

        var result = await _followsHandler.UnfollowUser(currentUserId, userId);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 400);
    }

    // TODO: bør være ActionResult<List<FollowDto>> for Swagger-støtte
    [HttpGet("followers")]
    [Authorize]
    public async Task<IActionResult> GetFollowers()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == null)
            return Problem(detail: "User not authenticated", statusCode: 401);

        var result = await _followsHandler.GetFollowers(currentUserId);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 400);
    }

    // TODO: bør være ActionResult<List<FollowDto>> for Swagger-støtte
    [HttpGet("following")]
    [Authorize]
    public async Task<IActionResult> GetFollowing()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == null)
            return Problem(detail: "User not authenticated", statusCode: 401);

        var result = await _followsHandler.GetFollowing(currentUserId);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 400);
    }

    // TODO: bør være ActionResult<bool> for Swagger-støtte
    [HttpGet("check/{userId}")]
    [Authorize]
    public async Task<IActionResult> IsFollowing(string userId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == null)
            return Problem(detail: "User not authenticated", statusCode: 401);

        var result = await _followsHandler.IsFollowing(currentUserId, userId);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 400);
    }
}
