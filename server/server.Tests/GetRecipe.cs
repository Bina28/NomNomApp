using Moq;
using server.Domain;
using server.Features.Recipes.GetRecipe;
using server.Features.Recipes.Services.RecipeApiClients;
using Server.Features.Recipes.GetRecipe;

namespace server.Tests;

public class GetRecipe
{
    [Fact]
    public async Task GetRecipeById_ReturnsRecipeFromDb_WhenExists()
    {
        // --- GIVEN: There is a database with one recipe ---
        var recipe = new Recipe { Id = 1, Title = "Test Recipe" };

        var mockClient = new Mock<IRecipeApiClient>();
        var mockApiHandler = new Mock<ISaveRecipeFromApiHandler>();


        var repoMock = new Mock<IRecipeRepository>();
        repoMock.Setup(r => r.GetByIdWithDetailsAsync(1))
                .ReturnsAsync(recipe);

        var service = new GetRecipeByIdHandler(repoMock.Object, mockClient.Object, mockApiHandler.Object);

        // --- WHEN: We search for a recipe by id = 1 ---
        var result = await service.GetRecipeById(1);

        // --- THEN: The method returns the correct recipe ---
        Assert.True(result.Success);
        Assert.Equal("Test Recipe", result.Data?.Title);

        mockClient.Verify(c => c.GetRecipeById(It.IsAny<int>()),
        Times.Never
    );
    }

    [Fact]
    public async Task GetRecipeById_SavesAndReturnsRecipe_WhenNotInDbButExistsInApi()
    {
        // --- GIVEN: Recipe doesn't exist in DB ---
        var repoMock = new Mock<IRecipeRepository>();
        repoMock
            .Setup(r => r.GetByIdWithDetailsAsync(1))
            .ReturnsAsync((Recipe?)null);

        // --- GIVEN: Recipe exists in API ---
        var apiRecipe = new ApiRecipeDto { Id = 1, Title = "Test Recipe" };

        var mockClient = new Mock<IRecipeApiClient>();
        mockClient
            .Setup(c => c.GetRecipeById(1))
            .ReturnsAsync(apiRecipe);


        // --- GIVEN: Recipe saved in DB ---
        var recipe = new Recipe { Id = 1, Title = "Test Recipe" };
        var mockApiHandler = new Mock<ISaveRecipeFromApiHandler>();
        mockApiHandler
            .Setup(h => h.SaveRecipe(apiRecipe))
            .ReturnsAsync(recipe);


        var service = new GetRecipeByIdHandler(repoMock.Object, mockClient.Object, mockApiHandler.Object);

        // --- WHEN: We search for a recipe by id = 1 ---
        var result = await service.GetRecipeById(1);

        // --- THEN: The method returns the correct recipe ---
        Assert.True(result.Success);
        Assert.Equal("Test Recipe", result.Data?.Title);

        mockApiHandler.Verify(h => h.SaveRecipe(apiRecipe),
            Times.Once
     );
    }

    [Fact]
    public async Task GetRecipeById_ReturnsNotFound_WhenRecipeDoesNotExist()
    {
        // --- GIVEN: Recipe doesn't exists in DB ---
        var repoMock = new Mock<IRecipeRepository>();
        repoMock
            .Setup(r => r.GetByIdWithDetailsAsync(1))
            .ReturnsAsync((Recipe?)null);

        // --- GIVEN: Recipe doesn't exists in API ---
        var mockClient = new Mock<IRecipeApiClient>();
        mockClient
            .Setup(c => c.GetRecipeById(1))
             .ReturnsAsync((ApiRecipeDto?)null);

        // --- GIVEN: Nothing is saved to DB ---
        var mockApiHandler = new Mock<ISaveRecipeFromApiHandler>();    

        var service = new GetRecipeByIdHandler(repoMock.Object, mockClient.Object, mockApiHandler.Object);

        // --- WHEN: We search for a recipe by id = 1 ---
        var result = await service.GetRecipeById(1);

        // --- THEN: The method returns not found message ---
        Assert.False(result.Success);

        // --- THEN: Recipe is NOT saved ---
        mockApiHandler.Verify(
            h => h.SaveRecipe(It.IsAny<ApiRecipeDto>()),
            Times.Never);
    }

}
