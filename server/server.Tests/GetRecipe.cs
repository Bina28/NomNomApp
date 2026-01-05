using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using server.Domain;
using Server.Features.Recipes.GetRecipeById;
using Server.Features.Recipes.Infrastructure.Recipes;
using Server.Features.Recipes.SaveRecipe;

namespace server.Tests;

public class GetRecipe
{
    [Fact]
    public async Task GetRecipeById_ReturnsRecipeFromDb_WhenExists()
    {
        // --- GIVEN: There is a database with one recipe ---
        var recipe = new Recipe { Id = 1, Title = "Test Recipe" };

        var mockClient = Substitute.For<IRecipeProvider>();
        var mockApiHandler = Substitute.For<ISaveRecipeHandler>();
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

        // --- THEN: The method doesn't call external API---
        await mockClient.DidNotReceive().GetRecipeById(recipe.Id);


    }

    [Fact]
    public async Task GetRecipeById_SavesAndReturnsRecipe_WhenNotInDbButExistsInApi()
    {
        // --- GIVEN: Recipe doesn't exist in DB ---
        var repoMock = Substitute.For<IRecipeRepository>();
        repoMock.GetByIdWithDetailsAsync(1)
            .Returns((Recipe?)null);

        // --- GIVEN: Recipe exists in API ---
        var apiRecipe = new Recipe { Id = 1, Title = "Test Recipe" };

        var mockClient = Substitute.For<IRecipeProvider>();
        mockClient.GetRecipeById(1)
            .Returns(apiRecipe);


        // --- GIVEN: Recipe saved in DB ---
        var recipe = new Recipe { Id = 1, Title = "Test Recipe" };
        var mockApiHandler = Substitute.For<ISaveRecipeHandler>();
        mockApiHandler.SaveRecipe(apiRecipe)
            .Returns(recipe);

        var logger = Substitute.For<ILogger<GetRecipeByIdHandler>>();

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
            .ReturnsNull();

        // --- GIVEN: Recipe doesn't exists in API ---
        var mockClient = Substitute.For<IRecipeProvider>();
        mockClient.GetRecipeById(1)
             .ReturnsNull();

        // --- GIVEN: Nothing is saved to DB ---
        var mockApiHandler = Substitute.For<ISaveRecipeHandler>();

        var logger = Substitute.For<ILogger<GetRecipeByIdHandler>>();
        var sut = new GetRecipeByIdHandler(repoMock, mockClient, mockApiHandler, logger);

        // --- WHEN: We search for a recipe by id = 1 ---
        var result = await sut.GetRecipeById(1);

        // --- THEN: The method returns not found message ---
        Assert.False(result.Success);

        // --- THEN: Recipe is NOT saved ---
        await mockApiHandler.DidNotReceive().SaveRecipe(Arg.Any<Recipe>());

    }

}
