using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Features.Comments.DTOs;
using Server.Features.Shared;

namespace Server.Features.Comments.GetComments;

public class GetCommentsHandler
{
    private readonly AppDbContext _context;

    public GetCommentsHandler(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Result<List<CommentResponse>>> GetCommentsForRecipe(int recipeId, CancellationToken ct = default)
    {
        var comments = await _context.Comments
            .AsNoTracking()
            .Where(c => c.RecipeId == recipeId)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CommentResponse(
                c.Id,
                c.Text,
                c.Score,
                c.CreatedAt,
                c.User.UserName,
                c.UserId
            ))
            .ToListAsync(ct);

        return Result<List<CommentResponse>>.Ok(comments);
    }
}
