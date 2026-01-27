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

    public async Task<Result<List<CommentDto>>> GetCommentsForRecipe(int recipeId)
    {
        var comments = await _context.Comments
            .Include(c => c.User)
            .Where(c => c.RecipeId == recipeId)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CommentDto(
                c.Id,
                c.Text,
                c.Score,
                c.CreatedAt,
                c.User.UserName,
                c.UserId
            ))
            .ToListAsync();

        return Result<List<CommentDto>>.Ok(comments);
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

    public async Task<Result<CommentDto>> PostComment(CreateCommentRequest request, string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return Result<CommentDto>.Fail("User not found");
        }

        if (!int.TryParse(request.RecipeId, out var recipeId))
        {
            return Result<CommentDto>.Fail("Invalid recipe ID");
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

        await _sseManager.BroadcastToAll("new_comment", new
        {
            commentId = comment.Id,
            recipeId = comment.RecipeId,
            userName = user.UserName,
            text = comment.Text,
            score = comment.Score,
            createdAt = comment.CreatedAt
        });

        var commentDto = new CommentDto(
            comment.Id,
            comment.Text,
            comment.Score,
            comment.CreatedAt,
            user.UserName,
            comment.UserId
        );

        return Result<CommentDto>.Ok(commentDto);
    }
}
