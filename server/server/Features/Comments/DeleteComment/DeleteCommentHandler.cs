using Server.Data;
using Server.Features.Shared;

namespace Server.Features.Comments.DeleteComment;

public class DeleteCommentHandler
{
    private readonly AppDbContext _context;
    private readonly ILogger<DeleteCommentHandler> _logger;

    public DeleteCommentHandler(AppDbContext context, ILogger<DeleteCommentHandler> logger)
    {
        _context = context;
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

}
