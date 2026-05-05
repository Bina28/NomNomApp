using System.ComponentModel.DataAnnotations;

namespace Server.Features.Comments.DTOs;

public record CreateCommentRequest
{
    [Required, MinLength(1), MaxLength(500)]
    public required string Text { get; set; }

    [Required, Range(1, 5)]
    public int Score { get; set; }
}


