namespace Server.Features.Comments.DTOs;


public record CommentDto(
string Id,
string Text,
int Score,
DateTime CreatedAt,
string UserName,
string UserId
);


