using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Features.Comments.DTOs;
using System.Security.Claims;

namespace Server.Features.Comments;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly CommentsHandler _commentsHandler;

    public CommentsController(CommentsHandler commentsHandler)
    {
        _commentsHandler = commentsHandler;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> PostAsync([FromBody] CreateCommentRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized("User not found");
        }

        var result = await _commentsHandler.PostComment(request, userId);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("recipe/{recipeId}")]
    public async Task<IActionResult> GetCommentsForRecipe(int recipeId)
    {
        var result = await _commentsHandler.GetCommentsForRecipe(recipeId);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _commentsHandler.DeleteComment(id);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

    [HttpGet("recipe/{recipeId}/score")]
    public async Task<IActionResult> GetCommentsScore(int recipeId)
    {
        var result = await _commentsHandler.GetCommentsScore(recipeId);
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }
}
