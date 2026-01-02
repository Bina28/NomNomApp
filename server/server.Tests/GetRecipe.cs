using Microsoft.Extensions.Logging;
using NSubstitute;
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

        var mockClient = Substitute.For<IRecipeApiClient>();
        var mockApiHandler = Substitute.For<ISaveRecipeFromApiHandler>();
        var logger = Substitute.For<ILogger<GetRecipeByIdHandler>>();
        var repoMock = Substitute.For<IRecipeRepository>();

        repoMock.GetByIdWithDetailsAsync(1)
                .Returns(recipe);

        var sut = new GetRecipeByIdHandler(repoMock, mockClient, mockApiHandler, logger);

        // --- WHEN: We search for a recipe by id = 1 ---
        var result = await sut.GetRecipeById(1);

        // --- THEN: The method returns the correct recipe ---
        Assert.True(result.Success);
        Assert.Equal("Test Recipe", result.Data?.Title);

        await mockClient.DidNotReceive().GetRecipeById(Arg.Any<int>());


    }

    [Fact]
    public async Task GetRecipeById_SavesAndReturnsRecipe_WhenNotInDbButExistsInApi()
    {
        // --- GIVEN: Recipe doesn't exist in DB ---
        var repoMock = Substitute.For<IRecipeRepository>();
        repoMock.GetByIdWithDetailsAsync(1)
            .Returns((Recipe?)null);

        // --- GIVEN: Recipe exists in API ---
        var apiRecipe = new ApiRecipeDto { Id = 1, Title = "Test Recipe" };

        var mockClient = Substitute.For<IRecipeApiClient>();
        mockClient.GetRecipeById(1)
            .Returns(apiRecipe);


        // --- GIVEN: Recipe saved in DB ---
        var recipe = new Recipe { Id = 1, Title = "Test Recipe" };
        var mockApiHandler = Substitute.For<ISaveRecipeFromApiHandler>();
        mockApiHandler.SaveRecipe(apiRecipe)
            .Returns(recipe);

        var logger =Substitute.For<ILogger<GetRecipeByIdHandler>>();

        var sut = new GetRecipeByIdHandler(repoMock, mockClient, mockApiHandler, logger);

        // --- WHEN: We search for a recipe by id = 1 ---
        var result = await sut.GetRecipeById(1);

        // --- THEN: The method returns the correct recipe ---
        Assert.True(result.Success);
        Assert.Equal("Test Recipe", result.Data?.Title);

        await mockApiHandler.Received(1).SaveRecipe(apiRecipe);
      
     
    }

    [Fact]
    public async Task GetRecipeById_ReturnsNotFound_WhenRecipeDoesNotExist()
    {
        // --- GIVEN: Recipe doesn't exists in DB ---
        var repoMock = Substitute.For<IRecipeRepository>();
        repoMock.GetByIdWithDetailsAsync(1)
            .Returns((Recipe?)null);

        // --- GIVEN: Recipe doesn't exists in API ---
        var mockClient = Substitute.For<IRecipeApiClient>();
        mockClient.GetRecipeById(1)
             .Returns((ApiRecipeDto?)null);

        // --- GIVEN: Nothing is saved to DB ---
        var mockApiHandler = Substitute.For<ISaveRecipeFromApiHandler>();

        var logger = Substitute.For<ILogger<GetRecipeByIdHandler>>();
        var sut = new GetRecipeByIdHandler(repoMock, mockClient, mockApiHandler, logger);

        // --- WHEN: We search for a recipe by id = 1 ---
        var result = await sut.GetRecipeById(1);

        // --- THEN: The method returns not found message ---
        Assert.False(result.Success);

        // --- THEN: Recipe is NOT saved ---
        await mockApiHandler.DidNotReceive().SaveRecipe(Arg.Any<ApiRecipeDto>());
         
    }

}
