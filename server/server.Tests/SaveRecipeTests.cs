using NSubstitute;
using Server.Domain;
using Server.Features.Recipes.Infrastructure.Photo;
using Server.Features.Recipes.Infrastructure.Photo.CloudinaryPhoto;
using Server.Features.Recipes.SaveRecipe;

namespace Server.Tests;

public class SaveRecipeTests
{
    [Fact]
    public async Task SaveRecipe_WhenRecipeHasIngredientsAndImage_ShouldSaveRecipeAndUploadImage()
    {
        //Arrange
        using var db = new TestDb();

        var photoMock = Substitute.For<IPhotoProvider>();
        photoMock
            .UploadImgFromUrlAsync("Test image url")
            .Returns(new PhotoUploadResult("public-id", "uploaded-url"));

        var recipe = new Recipe
        {
            Id = 1,
            Image = "Test image url",
            ExtendedIngredients = [new Ingredient { Original = "Salt" },
            new Ingredient { Original = "Pepper" }]
        };

        //Act
        var sut = new SaveRecipeHandler(db.Context, photoMock);
        await sut.SaveRecipeAsync(recipe);

        //Assert
        Assert.Equal(1, db.Context.Recipes.Count());
        Assert.Equal(2, db.Context.Ingredients.Count());
        Assert.Equal(1, db.Context.Photos.Count());
        Assert.NotNull(recipe.Photos);
        Assert.Equal("uploaded-url", recipe.Image);
    }

    [Fact]
    public async Task SaveRecipe_SavesRecipeWithIngredients_WithoutUploadingImage()
    {
        //Arrange
        using var db = new TestDb();

        var photoMock = Substitute.For<IPhotoProvider>();

        var recipe = new Recipe
        {
            Id = 1,
            ExtendedIngredients = [new Ingredient { Original = "Salt" },
            new Ingredient { Original = "Pepper" }],
            Image = null
        };

        //Act
        var sut = new SaveRecipeHandler(db.Context, photoMock);
        await sut.SaveRecipeAsync(recipe);

        //Assert
        var savedRecipe = db.Context.Recipes.First();
        Assert.Null(savedRecipe.Photos);
        Assert.Null(savedRecipe.Image);

        Assert.Equal(1, db.Context.Recipes.Count());
        Assert.Equal(2, db.Context.Ingredients.Count());

        await photoMock.DidNotReceive().UploadImgFromUrlAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task SaveRecipe_SavesRecipeWithDublicatIngredients_WithoutSavingDuplciatToDb()
    {
        //Arrange
        using var db = new TestDb();
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
        var sut = new SaveRecipeHandler(db.Context, photoMock);
        await sut.SaveRecipeAsync(recipe);

        //Assert
        Assert.Equal(2, db.Context.Ingredients.Count());
    }
}
