using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Server.Domain;
using Server.Features.Comments.PostComment;
using Server.Features.Sse;

namespace Server.Tests;


public class CommentsHandlerTests
{
    [Fact]
    public async Task PostComment_ValidRequest_ReturnsCommentResponse()
    {
        //Arrange
        using var db = new TestDb();
        db.Context.Recipes.Add(new Recipe { Id = 1 });
        db.Context.Users.Add(new User { Id = "user-123", UserName = "TestUser", Email = "testuser@example.com", PasswordHash = "hashedpassword" });
        await db.Context.SaveChangesAsync();

        var sseManager = new SetConnectionManager();
        var mockLogger = Substitute.For<ILogger<PostCommentHandler>>();

        var sut = new PostCommentHandler(db.Context, sseManager, mockLogger);

        //Act
        var result = await sut.PostComment(1, new CreateCommentRequest { Text = "Test comment", Score = 2 }, "user-123");

        //Assert       
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("Test comment", result.Data!.Text);
        Assert.Equal(2, result.Data.Score);
        Assert.Equal("user-123", result.Data.UserId);
        Assert.Equal("TestUser", result.Data.UserName);
        Assert.Equal(1, db.Context.Comments.Count());

    }

    [Fact]
    public async Task PostComment_NonExistentRecipe_ReturnsFailure()
    {
        //Arrange
        using var db = new TestDb();
        db.Context.Users.Add(new User { Id = "user-123", UserName = "TestUser", Email = "testuser@example.com", PasswordHash = "hashedpassword" });
        await db.Context.SaveChangesAsync();


        var sseManager = new SetConnectionManager();
        var mockLogger = Substitute.For<ILogger<PostCommentHandler>>();
        var sut = new PostCommentHandler(db.Context, sseManager, mockLogger);

        //Act
        var result = await sut.PostComment(1, new CreateCommentRequest { Text = "Test comment", Score = 2 }, "user-123");

        //Assert
        Assert.False(result.Success);
        Assert.Equal("Recipe not found", result.Error);
        Assert.Empty(db.Context.Comments);

    }

    [Fact]
    public async Task PostComment_NonExistentUser_ThrowsUnauthorizedAccessException()
    {
        //Arrange
        using var db = new TestDb();
        db.Context.Recipes.Add(new Recipe { Id = 1 });
        await db.Context.SaveChangesAsync();


        var sseManager = new SetConnectionManager();
        var mockLogger = Substitute.For<ILogger<PostCommentHandler>>();
        var sut = new PostCommentHandler(db.Context, sseManager, mockLogger);

        //Act
        var result =await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.PostComment(1, new CreateCommentRequest { Text = "Test comment", Score = 2 }, "non-existent-user")  );

        //Assert       
        Assert.Empty(db.Context.Comments);

    }
}
