
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Features.Shared;

namespace Server.Features.Comments;

public class CommentsHandler
{
    private readonly AppDbContext _context;
    public CommentsHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> DeleteComment(string id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
        {
            return Result<bool>.Fail("Comment not found");
        }
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
        return Result<bool>.Ok(true);
    }

    public async Result<> GetCommentsForRecipe(int recipeId)
    {
        var comments = await _context.Comments
             .Where(c => c.RecipeId == recipeId)
             .ToListAsync();
        if (comments == null || comments.Count == 0)
        {
            return Result<>.Fail("No comments found for this recipe");
        }


        return Result<>.Ok();
    }

    public async Result<> GetCommentsScore(int recipeId)
    {

    }

    public async Result<> PostComment()
    {
        await _sseManager.BroadcastToRecipeViewers(comment.RecipeId, "new_comment", new
        {
            commentId = comment.Id,
            recipeId = comment.RecipeId,
            userName = user.UserName,
            text = comment.Text,
            score = comment.Score,
            createdAt = comment.CreatedAt
        });
    }
}
