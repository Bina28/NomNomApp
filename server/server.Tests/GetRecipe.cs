using Microsoft.EntityFrameworkCore;
using Moq;
using server.Data;
using server.Domain;
using server.Features.Recipes.GetRecipe;
using server.Features.Recipes.Services.RecipeApiClients;
using Server.Features.Recipes.SaveRecipe;

namespace server.Tests;

public class GetRecipe
{
    [Fact]
    public async Task GetRecipeById_ReturnsRecipeFromDb_WhenExists()
    {
        // --- GIVEN: There is a database with one recipe ---
        var mockClient = new Mock<IRecipeApiClient>();
        var mockApiHandler = new Mock<ISaveRecipeFromApiHandler>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
          .UseInMemoryDatabase(databaseName: "TestDb1")
            .Options;


        using var context = new AppDbContext(options);
        context.Recipes.Add(new Recipe
        {
            Id = 1,
            Title = "Test Recipe",
            Summary = "Summary",
            Instructions = "Instructions",
            Image = "image.jpg",
            ExtendedIngredients =
            [
                new() { Id = 1, Original ="test ingredient" }
            ],
            Photos = null
        });
        context.SaveChanges();

        var service = new GetRecipeByIdHandler(context, mockClient.Object, mockApiHandler.Object);

        // --- WHEN: We search for a recipe by id = 1 ---
        var result = await service.GetRecipeById(1);

        // --- THEN: The method returns the correct recipe ---
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal($"{result.Title}", result.Title);
        Assert.Equal($"{result.Summary}", result.Summary);
        Assert.Equal($"{result.Instructions}", result.Instructions);

        Assert.NotNull(result.ExtendedIngredients);
        Assert.Single(result.ExtendedIngredients);
        Assert.Equal("test ingredient", result.ExtendedIngredients[0].Original);
        Assert.Equal(1, result.ExtendedIngredients[0].Id);

        Assert.NotNull(result.ExtendedIngredients);
        Assert.Null(result.Photos);
    }
}
