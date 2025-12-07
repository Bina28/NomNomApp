namespace server.Domain;

public class RecipeDetail
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public List<ExtendedIngredients>? Ingredients { get; set; }
    public string? Summary { get; set; }
    public string? Instructions { get; set; }
    public List<AnalyzedInstruction>? AnalyzedInstructions { get; set; }

}
