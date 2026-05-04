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
    private readonly ILogger<CommentsHandler> _logger;

    public CommentsHandler(AppDbContext context, SetConnectionManager sseManager, ILogger<CommentsHandler> logger)
    {
        _context = context;
        _sseManager = sseManager;
        _logger = logger;
    }

    public async Task<Result<bool>> DeleteComment(string commentId, CancellationToken ct = default)
    {
        var comment = await _context.Comments.FindAsync([commentId], ct);
        if (comment == null)
        {
            _logger.LogWarning("Comment {CommentId} not found for delete", commentId);
            return Result<bool>.Fail("Comment not found");
        }

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Deleted comment {CommentId}", commentId);
        return Result<bool>.Ok(true);
    }

    public async Task<Result<List<CommentDto>>> GetCommentsForRecipe(int recipeId, CancellationToken ct = default)
    {
        var comments = await _context.Comments
            .AsNoTracking()
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
            .ToListAsync(ct);

        return Result<List<CommentDto>>.Ok(comments);
    }

    public async Task<Result<double>> GetCommentsScore(int recipeId, CancellationToken ct = default)
    {
        var averageScore = await _context.Comments
          .Where(c => c.RecipeId == recipeId)
          .Select(s => (double?)s.Score)
          .AverageAsync(ct);

        return Result<double>.Ok(averageScore ?? 0);
    }

    public async Task<Result<CommentDto>> PostComment(CreateCommentRequest request, string userId, CancellationToken ct = default)
    {
        var userName = await _context.Users
            .Where(u => u.Id == userId)
            .AsNoTracking()
            .Select(u => u.UserName)
            .FirstOrDefaultAsync(ct);

        if (userName == null)
        {
            _logger.LogError("Authenticated user {UserId} not found in database", userId);
            throw new UnauthorizedAccessException("Authenticated user not found in database.");
        }

        if (!int.TryParse(request.RecipeId, out var recipeId))
        {
            _logger.LogWarning("Invalid recipe id {RawRecipeId} from user {UserId}", request.RecipeId, userId);
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
        await _context.SaveChangesAsync(ct);

        await _sseManager.BroadcastToAll("new_comment", new
        {
            commentId = comment.Id,
            recipeId = comment.RecipeId,
            userName,
            text = comment.Text,
            score = comment.Score,
            createdAt = comment.CreatedAt
        });

        var commentDto = new CommentDto(
            comment.Id,
            comment.Text,
            comment.Score,
            comment.CreatedAt,
            userName,
            comment.UserId
        );

        _logger.LogInformation("Created comment {CommentId} on recipe {RecipeId} by user {UserId}", comment.Id, comment.RecipeId, userId);
        return Result<CommentDto>.Ok(commentDto);
    }
}
