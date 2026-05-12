using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Server.Domain;
using Server.Features.Recipes.GetRecipeById;
using Server.Features.Recipes.Infrastructure.Recipes;
using Server.Features.Recipes.SaveRecipe;

namespace Server.Tests;

public class GetRecipeTests
{
    [Fact]
    public async Task GetRecipeById_ReturnsRecipeFromDb_WhenExists()
    {
        //Arrange
        using var db = new TestDb();
        db.Context.Recipes.Add(new Recipe { Id = 1, Title = "Test Recipe" });
        await db.Context.SaveChangesAsync();

        var mockClient = Substitute.For<IRecipeProvider>();
        var mockApiHandler = Substitute.For<ISaveRecipeHandler>();
        var logger = Substitute.For<ILogger<GetRecipeByIdHandler>>();

        var sut = new GetRecipeByIdHandler(db.Context, mockClient, mockApiHandler, logger);

        // Act
        var result = await sut.GetRecipeById(1);

        //Assert
        Assert.True(result.Success);
        Assert.Equal("Test Recipe", result.Data?.Title);
        await mockClient.DidNotReceive().GetRecipeById(1);
    }

    [Fact]
    public async Task GetRecipeById_SavesAndReturnsRecipe_WhenNotInDbButExistsInApi()
    {
        //Arrange
        using var db = new TestDb();

        var apiRecipe = new Recipe { Id = 1, Title = "Test Recipe" };

        var mockClient = Substitute.For<IRecipeProvider>();
        mockClient.GetRecipeById(1)
            .Returns(apiRecipe);

        var savedRecipe = new Recipe { Id = 1, Title = "Test Recipe" };
        var mockApiHandler = Substitute.For<ISaveRecipeHandler>();
        mockApiHandler.SaveRecipe(apiRecipe)
            .Returns(savedRecipe);

        var logger = Substitute.For<ILogger<GetRecipeByIdHandler>>();

        var sut = new GetRecipeByIdHandler(db.Context, mockClient, mockApiHandler, logger);

        //Act
        var result = await sut.GetRecipeById(1);

        //Assert
        Assert.True(result.Success);
        Assert.Equal("Test Recipe", result.Data?.Title);
        await mockApiHandler.Received(1).SaveRecipe(apiRecipe);
    }

    [Fact]
    public async Task GetRecipeById_ReturnsNotFound_WhenRecipeDoesNotExist()
    {
        //Arrange
        using var db = new TestDb();

        var mockClient = Substitute.For<IRecipeProvider>();
        mockClient.GetRecipeById(1)
             .ReturnsNull();

        var mockApiHandler = Substitute.For<ISaveRecipeHandler>();

        var logger = Substitute.For<ILogger<GetRecipeByIdHandler>>();
        var sut = new GetRecipeByIdHandler(db.Context, mockClient, mockApiHandler, logger);

        //Act
        var result = await sut.GetRecipeById(1);

        //Assert
        Assert.False(result.Success);
        await mockApiHandler.DidNotReceive().SaveRecipe(Arg.Any<Recipe>());
    }
}
