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
    public async Task<Result<PageList<CommentResponse>>> GetCommentsForRecipe(int recipeId, PageParameters parameters, CancellationToken ct = default)
    {
        var query = _context.Comments
            .AsNoTracking()
            .Include(c=> c.User)
            .Where(c => c.RecipeId == recipeId)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CommentResponse(
                c.Id,
                c.Text,
                c.Score,
                c.CreatedAt,
                c.User.UserName,
                c.UserId
            ));

        var pagedComments = await PageList<CommentResponse>.CreateAsync(query, parameters.PageNumber, parameters.PageSize);

        return Result<PageList<CommentResponse>>.Ok(pagedComments);
    }


}
