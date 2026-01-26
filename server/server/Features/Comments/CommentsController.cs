using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;

namespace Server.Features.Comments;

[ApiController]
[Route("api/[controller]")]

public class CommentsController: ControllerBase
{
    private readonly CommentsHandler _commentsHandler;
    public CommentsController(CommentsHandler commentsHandler)
    {
        _commentsHandler = commentsHandler;
    }
    [HttpPost]
    public async Task<IActionResult> PostAsync()
    {
        var result = await _commentsHandler.PostComment();
        return result.Success ? Ok(result.Data) : BadRequest(result.Error);
    }

        [HttpGet("recipe/{recipeId}")]
    public IActionResult GetCommentsForRecipe(int recipeId)
    {   _commentsHandler.GetCommentsForRecipe(recipeId);
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {_commentsHandler.DeleteComment(id);
        return Ok();
    }

    [HttpGet("recipe/{recipeId}/score")]
    public IActionResult GetCommentsScore(int recipeId)
    {_commentsHandler.GetCommentsScore(recipeId);
        return Ok();
    }
}
