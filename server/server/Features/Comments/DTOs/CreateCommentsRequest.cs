namespace Server.Features.Comments.DTOs;

public record CreateCommentRequest(string RecipeId, string Text, int Score);
