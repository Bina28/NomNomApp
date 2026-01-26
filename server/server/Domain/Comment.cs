using server.Domain;

namespace Server.Domain;

public class Comment
{
    public string Id { get; set; } =Guid.NewGuid().ToString();
    public required string  Text { get; set; }
    public int Score { get; set; }
    public DateTime CreatedAt{ get; set; }=DateTime.UtcNow;

    //navigation properties
    public required string UserId { get; set; }
    public User User { get; set; } = null!;
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;
}
