using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Features.Shared;
using Server.Domain;
using Server.Features.Comments.DTOs;
using Server.Features.Sse;

namespace Server.Features.Comments;

public class CommentsHandler
{
    private readonly AppDbContext _context;
    private readonly SetConnectionManager _sseManager;

    public CommentsHandler(AppDbContext context, SetConnectionManager sseManager)
    {
        _context = context;
        _sseManager = sseManager;
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

    public async Task<Result<List<Comment>>> GetCommentsForRecipe(int recipeId)
    {
        var comments = await _context.Comments
            .Include(c => c.User)
            .Where(c => c.RecipeId == recipeId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return Result<List<Comment>>.Ok(comments);
    }

    public async Task<Result<double>> GetCommentsScore(int recipeId)
    {
        var comments = await _context.Comments
            .Where(c => c.RecipeId == recipeId)
            .ToListAsync();

        if (comments.Count == 0)
        {
            return Result<double>.Ok(0);
        }

        var averageScore = comments.Average(c => c.Score);
        return Result<double>.Ok(averageScore);
    }

    public async Task<Result<Comment>> PostComment(CreateCommentRequest request, string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return Result<Comment>.Fail("User not found");
        }

        if (!int.TryParse(request.RecipeId, out var recipeId))
        {
            return Result<Comment>.Fail("Invalid recipe ID");
        }

        var comment = new Comment
        {
            Text = request.Text,
            Score = request.Score,
            UserId = userId,
            RecipeId = recipeId
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        await _sseManager.BroadcastToRecipeViewers(comment.RecipeId, "new_comment", new
        {
            commentId = comment.Id,
            recipeId = comment.RecipeId,
            userName = user.UserName,
            text = comment.Text,
            score = comment.Score,
            createdAt = comment.CreatedAt
        });

        return Result<Comment>.Ok(comment);
    }
}
