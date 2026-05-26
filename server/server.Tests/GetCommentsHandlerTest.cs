using Server.Domain;
using Server.Features.Comments.GetComments;
using Server.Features.Comments.GetCommentsScore;

namespace Server.Tests;

public class GetCommentsHandlerTest
{
    [Fact]
    public async Task GetCommentsForRecipe_ReturnsOrderedByDate()
    {
        //Arrange
        using var db = new TestDb();

        db.Context.Recipes.Add(new Recipe { Id = 1 });
        db.Context.Users.AddRange(
            new User { Id = "1", UserName = "User1", Email = "u1@test.com", PasswordHash = "hash" },
            new User { Id = "2", UserName = "User2", Email = "u2@test.com", PasswordHash = "hash" },
            new User { Id = "3", UserName = "User3", Email = "u3@test.com", PasswordHash = "hash" }
        );
        db.Context.Comments.AddRange(
            new Comment { Id = "1", Text = "Comment1", UserId = "1", RecipeId = 1, CreatedAt = new DateTime(2026, 1, 3) },
            new Comment { Id = "2", Text = "Comment2", UserId = "2", RecipeId = 1, CreatedAt = new DateTime(2026, 1, 1) },
            new Comment { Id = "3", Text = "Comment3", UserId = "3", RecipeId = 1, CreatedAt = new DateTime(2026, 1, 2) }
        );
        await db.Context.SaveChangesAsync();

        var sut = new GetCommentsHandler(db.Context);
        //Act
        var result = await sut.GetCommentsForRecipe(1);

        //Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        var resultDates = result.Data!.Select(c => c.CreatedAt).ToList();
        Assert.Equal(resultDates.OrderByDescending(d => d).ToList(), resultDates);

    }

    [Fact]
    public async Task GetCommentsScore_NoComments_ReturnsZero()
    {
        //Arrange
        using var db = new TestDb();
        db.Context.Recipes.Add(new Recipe { Id = 1 });
        await db.Context.SaveChangesAsync();

        var sut = new GetCommentsHandler(db.Context);

        //Act
        var result = await sut.GetCommentsForRecipe(1);

        //Assert
        Assert.True(result.Success);
        Assert.NotNull(result);
        Assert.Empty(result.Data!);
    }


    [Fact]
    public async Task GetCommentsScore_WithComments_ReturnsAverage()
    {

        //Arrange
        using var db = new TestDb();
        db.Context.Recipes.Add(new Recipe { Id = 1 });
        db.Context.Users.AddRange(
          new User { Id = "1", UserName = "User1", Email = "u1@test.com", PasswordHash = "hash" },
          new User { Id = "2", UserName = "User2", Email = "u2@test.com", PasswordHash = "hash" },
          new User { Id = "3", UserName = "User3", Email = "u3@test.com", PasswordHash = "hash" }
      );
        db.Context.Comments.AddRange(
       new Comment { Id = "1", Text = "Comment1", UserId = "1", RecipeId = 1, CreatedAt = new DateTime(2026, 1, 3), Score = 5 },
       new Comment { Id = "2", Text = "Comment2", UserId = "2", RecipeId = 1, CreatedAt = new DateTime(2026, 1, 1), Score = 5 },
       new Comment { Id = "3", Text = "Comment3", UserId = "3", RecipeId = 1, CreatedAt = new DateTime(2026, 1, 2) }
   );
        await db.Context.SaveChangesAsync();

        var sut = new GetCommentsScoreHandler(db.Context);

        //Act
        var result = await sut.GetCommentsScore(1);

        //Assert
        Assert.True(result.Success);
        Assert.NotNull(result);
        Assert.Equal(10.0 / 3, result.Data, precision: 5);
    }
}
