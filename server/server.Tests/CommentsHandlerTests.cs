using Microsoft.Extensions.Logging;
using NSubstitute;
using Server.Domain;
using Server.Features.Comments.DeleteComment;
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

        var sseManager = new SseConnectionManager();
        var mockLogger = Substitute.For<ILogger<PostCommentHandler>>();

        var sut = new PostCommentHandler(db.Context, sseManager, mockLogger);

        //Act
        var result = await sut.PostCommentAsync(1, new CreateCommentRequest { Text = "Test comment", Score = 2 }, "user-123");

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


        var sseManager = new SseConnectionManager();
        var mockLogger = Substitute.For<ILogger<PostCommentHandler>>();
        var sut = new PostCommentHandler(db.Context, sseManager, mockLogger);

        //Act
        var result = await sut.PostCommentAsync(1, new CreateCommentRequest { Text = "Test comment", Score = 2 }, "user-123");

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


        var sseManager = new SseConnectionManager();
        var mockLogger = Substitute.For<ILogger<PostCommentHandler>>();
        var sut = new PostCommentHandler(db.Context, sseManager, mockLogger);

        //Act
        var result = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sut.PostCommentAsync(1, new CreateCommentRequest { Text = "Test comment", Score = 2 }, "non-existent-user"));

        //Assert       
        Assert.Empty(db.Context.Comments);

    }

    [Fact]
    public async Task DeleteComment_ExistingComment_ReturnTrue()
    {
        //Arrange
        using var db = new TestDb();
        db.Context.Recipes.Add(new Recipe { Id = 1 });
        db.Context.Users.Add(new User { Id = "1", UserName = "User1", Email = "u1@test.com", PasswordHash = "hash" });
        db.Context.Comments.Add(new Comment { Id = "2", Text = "Test", UserId = "1", RecipeId = 1 });
        await db.Context.SaveChangesAsync();

        var mockLogger = Substitute.For<ILogger<DeleteCommentHandler>>();

        var sut = new DeleteCommentHandler(db.Context, mockLogger);
        //Act
        var result = await sut.DeleteCommentAsync("2", "1");

        //Assert
        Assert.True(result.Success);
        Assert.True(result.Data);
        Assert.Empty(db.Context.Comments);
    }

    [Fact]
    public async Task DeleteComment_NonExistingComment_ReturnFalse()
    {
        //Arrange
        using var db = new TestDb();


        var mockLogger = Substitute.For<ILogger<DeleteCommentHandler>>();

        var sut = new DeleteCommentHandler(db.Context, mockLogger);
        //Act
        var result = await sut.DeleteCommentAsync("2", "1");

        //Assert
        Assert.False(result.Success);
        Assert.Equal("Comment not found", result.Error);


    }
}
