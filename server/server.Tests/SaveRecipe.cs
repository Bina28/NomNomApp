using Microsoft.EntityFrameworkCore;
using NSubstitute;
using server.Data;
using server.Domain;
using Server.Features.Recipes.Infrastructure.Photo;
using Server.Features.Recipes.Infrastructure.Photo.CloudinaryPhoto;
using Server.Features.Recipes.SaveRecipe;

namespace Server.Tests;

public class SaveRecipe
{
    [Fact]
    public async Task SaveRecipe_WhenRecipeHasIngredientsAndImage_ShouldSaveRecipeAndUploadImage()
    {

        //Arrange 
        var dbName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        using var context = new AppDbContext(options);

        var photoMock = Substitute.For<IPhotoProvider>();
        photoMock
       .UploadImgFromUrl("Test image url")
       .Returns(new PhotoUploadResult("public-id", "uploaded-url"));

        var recipe = new Recipe
        {
            Id = 1,
            Image = "Test image url",
            ExtendedIngredients = [new Ingredient { Original = "Salt" },
            new Ingredient { Original = "Pepper" }]
        };

        //Act
        var sut = new SaveRecipeHandler(context, photoMock);
        await sut.SaveRecipe(recipe);

        //Assert
        Assert.Equal(1, context.Recipes.Count());
        Assert.Equal(2, context.Ingredients.Count());
        Assert.Equal(1, context.Photos.Count());
        Assert.NotNull(recipe.Photos);
        Assert.Equal("uploaded-url", recipe.Image);
    }

    [Fact]
    public async Task SaveRecipe_SavesRecipeWithIngredients_WithoutUploadingImage()
    {

        //Arrange
        var dbName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<AppDbContext>()
       .UseInMemoryDatabase(dbName)
       .Options;

        var photoMock = Substitute.For<IPhotoProvider>();

        var recipe = new Recipe
        {
            Id = 1,
            ExtendedIngredients = [new Ingredient { Original = "Salt" },
            new Ingredient { Original = "Pepper" }],
            Image = null

        };

        using var context = new AppDbContext(options);

        //Act
        var sut = new SaveRecipeHandler(context, photoMock);
        await sut.SaveRecipe(recipe);

        //Assert
        var savedRecipe = context.Recipes.First();
        Assert.Null(savedRecipe.Photos);
        Assert.Null(savedRecipe.Image);

        Assert.Equal(1, context.Recipes.Count());
        Assert.Equal(2, context.Ingredients.Count());

        await photoMock.DidNotReceive().UploadImgFromUrl(Arg.Any<string>());

    }

    [Fact]
    public async Task SaveRecipe_SavesRecipeWithDublicatIngredients_WithoutSavingDuplciatToDb()
    {

        //Arrange
        var dbName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        using var context = new AppDbContext(options);
        var photoMock = Substitute.For<IPhotoProvider>();

        var recipe = new Recipe
        {
            Id = 1,
            ExtendedIngredients = [new Ingredient {Original ="Salt" },
            new Ingredient {Original ="Pepper" },
            new Ingredient {Original = "Salt" }
            ]
        };

        //Act
        var sut = new SaveRecipeHandler(context, photoMock);
        await sut.SaveRecipe(recipe);

        //Assert
        Assert.Equal(2, context.Ingredients.Count());

    }

}
