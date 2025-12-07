namespace server.Domain;

public class AnalyzedInstruction
{
    public string? Name { get; set; }
    public List<RecipeStep>? Steps { get; set; }
}