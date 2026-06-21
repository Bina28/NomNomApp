using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Features.Shared;

namespace Server.Features.Comments.GetCommentsScore;

public class GetCommentsScoreHandler
{
    private readonly AppDbContext _context;

    public GetCommentsScoreHandler(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Result<double>> GetCommentsScoreAsync(int recipeId, CancellationToken ct = default)
    {
        var averageScore = await _context.Comments
          .Where(c => c.RecipeId == recipeId)
          .Select(s => (double?)s.Score)
          .AverageAsync(ct);

        return Result<double>.Ok(averageScore ?? 0);
    }
}
