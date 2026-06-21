using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Server.Domain;
using Server.Features.Auth.Infrastructure.Jwt;
using Server.Features.Auth.Infrastructure.Password;
using Server.Features.Auth.RefreshTokens;
using Server.Features.Auth.GetAllUsers;
using Server.Features.Auth.GetCurrentUser;
using Server.Features.Auth.Login;
using Server.Features.Auth.Register;
using Server.Features.Shared;

namespace Server.Tests;

public class AuthHandlerTests
{
    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        //arrange
        using var db = new TestDb();

        var hashServiceMock = Substitute.For<IPasswordHasher>();
        var jwtServiceMock = Substitute.For<IJwtService>();
        var refreshTokenServiceMock = Substitute.For<IRefreshTokenService>();
        hashServiceMock.VerifyPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
        jwtServiceMock.GenerateToken(Arg.Any<string>()).Returns(("fake-access-token", "fake-refresh-token"));

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testuser",
            PasswordHash = "hashedpassword",
            Email = "test@gmail.com"
        };

        db.Context.Users.Add(user);
        await db.Context.SaveChangesAsync();

        var loggerMock = Substitute.For<ILogger<LoginHandler>>();
        var sut = new LoginHandler(db.Context, hashServiceMock, loggerMock, jwtServiceMock, refreshTokenServiceMock);
        var request = new LoginRequest { Email = "test@gmail.com", Password = "password" };

        //act
        var result = await sut.LoginAsync(request);

        //assert
        Assert.True(result.Success);
        Assert.Equal("fake-access-token", result.Data!.AccessToken);
        Assert.Equal("fake-refresh-token", result.Data.RefreshToken);
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsFail()
    {
        //arrange
        using var db = new TestDb();

        var hashServiceMock = Substitute.For<IPasswordHasher>();
        var jwtServiceMock = Substitute.For<IJwtService>();
        var refreshTokenServiceMock = Substitute.For<IRefreshTokenService>();
        hashServiceMock.VerifyPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testuser",
            PasswordHash = "hashedpassword",
            Email = "test@gmail.com"
        };

        db.Context.Users.Add(user);
        await db.Context.SaveChangesAsync();

        var loggerMock = Substitute.For<ILogger<LoginHandler>>();
        var sut = new LoginHandler(db.Context, hashServiceMock, loggerMock, jwtServiceMock, refreshTokenServiceMock);
        var request = new LoginRequest { Email = "test@gmail.com", Password = "password" };

        //act
        var result = await sut.LoginAsync(request);

        //assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("Invalid email or password", result.Error);
        jwtServiceMock.DidNotReceive().GenerateToken(Arg.Any<string>());
    }

    [Fact]
    public async Task LoginAsync_NonExistentUser_ReturnsFail()
    {
        //arrange
        using var db = new TestDb();

        var hashServiceMock = Substitute.For<IPasswordHasher>();
        var jwtServiceMock = Substitute.For<IJwtService>();
        var refreshTokenServiceMock = Substitute.For<IRefreshTokenService>();
        var loggerMock = Substitute.For<ILogger<LoginHandler>>();
        var sut = new LoginHandler(db.Context, hashServiceMock, loggerMock, jwtServiceMock, refreshTokenServiceMock);
        var request = new LoginRequest { Email = "someuser@gmail.com", Password = "password" };

        //act
        var result = await sut.LoginAsync(request);

        //assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("Invalid email or password", result.Error);
        jwtServiceMock.DidNotReceive().GenerateToken(Arg.Any<string>());
        hashServiceMock.DidNotReceive().VerifyPassword(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task RegisterAsync_NewUser_ReturnsToken()
    {
        //arrange
        using var db = new TestDb();

        var hashServiceMock = Substitute.For<IPasswordHasher>();
        var jwtServiceMock = Substitute.For<IJwtService>();
        var refreshTokenServiceMock = Substitute.For<IRefreshTokenService>();

        jwtServiceMock.GenerateToken(Arg.Any<string>()).Returns(("fake-access-token", "fake-refresh-token"));
        hashServiceMock.HashPassword(Arg.Any<string>()).Returns("hashedpassword");

        var loggerMock = Substitute.For<ILogger<LoginHandler>>();
        var sut = new RegisterHandler(db.Context, loggerMock, jwtServiceMock, new RegisterMapper(hashServiceMock), refreshTokenServiceMock);
        var request = new RegisterRequest { UserName = "newuser", Email = "test@gmail.com", Password = "password" };

        //act
        var result = await sut.RegisterAsync(request);

        //assert
        Assert.True(result.Success);
        Assert.Equal("fake-access-token", result.Data!.AccessToken);
        Assert.Equal("fake-refresh-token", result.Data.RefreshToken);
        Assert.Null(result.Error);

        var userInDb = await db.Context.Users.FirstOrDefaultAsync(u => u.Email == "test@gmail.com");
        Assert.NotNull(userInDb);
        Assert.Equal("newuser", userInDb.UserName);
        Assert.Equal("hashedpassword", userInDb.PasswordHash);

        hashServiceMock.Received(1).HashPassword("password");
        jwtServiceMock.Received(1).GenerateToken(Arg.Any<string>());
    }

    [Fact]
    public async Task GetCurrentUserAsync_ValidId_ReturnsUser()
    {
        //arrange
        using var db = new TestDb();

        var userId = Guid.NewGuid().ToString();
        db.Context.Users.Add(new User
        {
            Id = userId,
            UserName = "testuser",
            Email = "test@gmail.com",
            PasswordHash = "hashedpassword"
        });
        await db.Context.SaveChangesAsync();

        var sut = new GetCurrentUserHandler(db.Context, Substitute.For<ILogger<GetCurrentUserHandler>>());

        //act
        var result = await sut.GetCurrentUserAsync(userId);

        //assert
        Assert.True(result.Success);
        Assert.Null(result.Error);
        Assert.NotNull(result.Data);
        Assert.Equal(userId, result.Data!.Id);
        Assert.Equal("testuser", result.Data.UserName);
        Assert.Equal("test@gmail.com", result.Data.Email);
    }

    [Fact]
    public async Task GetAllUsersAsync_ExcludesCurrentUser()
    {
        //arrange
        using var db = new TestDb();

        var users = new List<User>
        {
            new() { Id = "1", UserName = "user1", Email = "user1@gmail.com", PasswordHash = "hash1" },
            new() { Id = "2", UserName = "user2", Email = "user2@gmail.com", PasswordHash = "hash2" },
            new() { Id = "3", UserName = "currentUser", Email = "currentUser@gmail.com", PasswordHash = "hash3" }
        };

        db.Context.Users.AddRange(users);
        await db.Context.SaveChangesAsync();

        var parameters = new PageParameters { PageNumber = 1, PageSize = 10 };
        var sut = new GetAllUsersHandler(db.Context);

        //act
        var result = await sut.GetUsersExceptCurrentAsync("3", parameters);

        //assert
        Assert.True(result.Success);
        Assert.Null(result.Error);
        Assert.NotNull(result.Data);

        Assert.DoesNotContain(result.Data!.Items, u => u.Id == "3");
        Assert.Contains(result.Data!.Items, u => u.Id == "1");
        Assert.Contains(result.Data!.Items, u => u.Id == "2");
        Assert.Equal(2, result.Data!.Count);
    }
}
