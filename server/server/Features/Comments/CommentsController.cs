using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Features.Auth;
using Server.Features.Comments.DTOs;

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
    public async Task<ActionResult<CommentDto>> PostAsync([FromBody] CreateCommentRequest request, CancellationToken ct)
    {
        var result = await _commentsHandler.PostComment(request, User.GetUserId(), ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 400);
    }


    [HttpGet("recipe/{recipeId}")]
    public async Task<ActionResult<List<CommentDto>>> GetCommentsForRecipe(int recipeId, CancellationToken ct)
    {
        var result = await _commentsHandler.GetCommentsForRecipe(recipeId, ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 400);
    }


    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<bool>> Delete(string id, CancellationToken ct)
    {
        var result = await _commentsHandler.DeleteComment(id, ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 400);
    }


    [HttpGet("recipe/{recipeId}/score")]
    public async Task<ActionResult<double>> GetCommentsScore(int recipeId, CancellationToken ct)
    {
        var result = await _commentsHandler.GetCommentsScore(recipeId, ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 400);
    }
}
