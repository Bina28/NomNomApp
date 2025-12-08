namespace server.Features.Recipes.GetRecipeDetails;

public class GetRecipeService
{
    private readonly RecipeProvider _provider;

    public GetRecipeService(RecipeProvider provider)
    {
        _provider = provider;
    }

    public async Task<RecipeDto> Handle(int id)
    {
        var recipe = await _provider.GetRecipeById(id);
        return RecipeMapper.ToDto(recipe);
    }


}
