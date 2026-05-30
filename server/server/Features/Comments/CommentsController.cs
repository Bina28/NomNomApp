using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Features.Auth;
using Server.Features.Comments.DeleteComment;
using Server.Features.Comments.DTOs;
using Server.Features.Comments.GetComments;
using Server.Features.Comments.GetCommentsScore;
using Server.Features.Comments.PostComment;
using Server.Features.Shared;

namespace Server.Features.Comments;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly PostCommentHandler _postCommentHandler;
    private readonly DeleteCommentHandler _deleteCommentHandler;
    private readonly GetCommentsHandler _getCommentsHandler;
    private readonly GetCommentsScoreHandler _getCommentsScoreHandler;

    public CommentsController(PostCommentHandler postCommentHandler, DeleteCommentHandler deleteCommentHandler, GetCommentsHandler getCommentsHandler, GetCommentsScoreHandler getCommentsScoreHandler)
    {
        _postCommentHandler = postCommentHandler;
        _deleteCommentHandler = deleteCommentHandler;
        _getCommentsHandler = getCommentsHandler;
        _getCommentsScoreHandler = getCommentsScoreHandler;
    }

    [HttpPost("recipe/{recipeId}")]
    [Authorize]
    public async Task<ActionResult<CommentResponse>> PostAsync(int recipeId, [FromBody] CreateCommentRequest request, CancellationToken ct)
    {
        var result = await _postCommentHandler.PostComment(recipeId, request, User.GetUserId(), ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 400);
    }


    [HttpGet("recipe/{recipeId}")]
    public async Task<ActionResult<List<CommentResponse>>> GetCommentsForRecipe(int recipeId, PageParameters parameters, CancellationToken ct)
    {
        var result = await _getCommentsHandler.GetCommentsForRecipe(recipeId, parameters, ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 400);
    }


    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<bool>> Delete(string commentId, CancellationToken ct)
    {
        var result = await _deleteCommentHandler.DeleteComment(commentId, User.GetUserId(), ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 400);
    }


    [HttpGet("recipe/{recipeId}/score")]
    public async Task<ActionResult<double>> GetCommentsScore(int recipeId, CancellationToken ct)
    {
        var result = await _getCommentsScoreHandler.GetCommentsScore(recipeId, ct);
        return result.Success ? Ok(result.Data) : Problem(detail: result.Error, statusCode: 400);
    }
}
