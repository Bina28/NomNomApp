using System.ComponentModel.DataAnnotations.Schema;

namespace server.Domain;

public class Photo
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Url { get; set; }
    public required string PublicId { get; set; }

    [ForeignKey("Recipe")]
    public int RecipeId { get; set; }
    public required Recipe Recipe { get; set; }
}
