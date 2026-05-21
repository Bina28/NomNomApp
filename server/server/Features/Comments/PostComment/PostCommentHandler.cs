using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Domain;
using Server.Features.Comments.DTOs;
using Server.Features.Shared;
using Server.Features.Sse;

namespace Server.Features.Comments.PostComment;

public class PostCommentHandler
{
    private readonly AppDbContext _context;
    private readonly SetConnectionManager _sseManager;
    private readonly ILogger<PostCommentHandler> _logger;

    public PostCommentHandler(AppDbContext context, SetConnectionManager sseManager, ILogger<PostCommentHandler> logger)
    {
        _context = context;
        _sseManager = sseManager;
        _logger = logger;
    }

    public async Task<Result<CommentResponse>> PostComment(int recipeId, CreateCommentRequest request, string userId, CancellationToken ct = default)
    {
        var recipeExists = await _context.Recipes
            .AsNoTracking()
            .AnyAsync(r => r.Id == recipeId, ct);

        if (!recipeExists)
        {
            _logger.LogWarning("Recipe {RecipeId} not found for comment creation", recipeId);
            return Result<CommentResponse>.Fail("Recipe not found");
        }

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null)
        {
            _logger.LogError("Authenticated user {UserId} not found in database", userId);
            throw new UnauthorizedAccessException("Authenticated user not found in database.");
        }

        var userName = user.UserName;

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

        var commentDto = new CommentResponse(
            comment.Id,
            comment.Text,
            comment.Score,
            comment.CreatedAt,
            userName,
            comment.UserId
        );

        _logger.LogInformation("Created comment {CommentId} on recipe {RecipeId} by user {UserId}", comment.Id, comment.RecipeId, userId);
        return Result<CommentResponse>.Ok(commentDto);
    }
}
