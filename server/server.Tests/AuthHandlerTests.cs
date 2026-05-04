using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using server.Data;
using server.Domain;
using server.Features.Auth;
using server.Features.Auth.DTOs;
using Server.Features.Auth.DTOs;

namespace Server.Tests;


public class AuthHandlerTests
{
    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        //arrange
        var userDb = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(userDb)
            .Options;

        using var context = new AppDbContext(options);

        var hashServiceMock = Substitute.For<IPasswordHasher>();
        var jwtServiceMock = Substitute.For<IJwtService>();
        hashServiceMock.VerifyPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
        jwtServiceMock.GenerateToken(Arg.Any<string>(), Arg.Any<string>()).Returns("fake-jwt-token");

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testuser",
            PasswordHash = "hashedpassword",
            Email = "test@gmail.com"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
        var loggerMock = Substitute.For<ILogger<AuthHandler>>();
        var authHandler = new AuthHandler(jwtServiceMock, context, hashServiceMock, new RegisterMapper(hashServiceMock), loggerMock);
        var request = new LoginRequest("test@gmail.com", "password");

        //act       
        var result = await authHandler.LoginAsync(request);

        //assert   
        Assert.True(result.Success);
        Assert.Equal("fake-jwt-token", result.Data);
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsFail()
    {
        //arrange
        var userDb = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(userDb)
            .Options;

        using var context = new AppDbContext(options);
        var hashServiceMock = Substitute.For<IPasswordHasher>();
        var jwtServiceMock = Substitute.For<IJwtService>();
        hashServiceMock.VerifyPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testuser",
            PasswordHash = "hashedpassword",
            Email = "test@gmail.com"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
        var loggerMock = Substitute.For<ILogger<AuthHandler>>();
        var authHandler = new AuthHandler(jwtServiceMock, context, hashServiceMock, new RegisterMapper(hashServiceMock), loggerMock);
        var request = new LoginRequest("test@gmail.com", "password");

        //act      
        var result = await authHandler.LoginAsync(request);

        //assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("Invalid email or password", result.Error);
        jwtServiceMock.DidNotReceive().GenerateToken(Arg.Any<string>(), Arg.Any<string>());

    }

    [Fact]
    public async Task LoginAsync_NonExistentUser_ReturnsFail()
    {
        //arrange
        var userDb = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(userDb)
            .Options;

        using var context = new AppDbContext(options);
        var hashServiceMock = Substitute.For<IPasswordHasher>();
        var jwtServiceMock = Substitute.For<IJwtService>();
        var loggerMock = Substitute.For<ILogger<AuthHandler>>();
        var authHandler = new AuthHandler(jwtServiceMock, context, hashServiceMock, new RegisterMapper(hashServiceMock), loggerMock);
        var request = new LoginRequest("someuser@gmail.com", "password");

        //act
        var result = await authHandler.LoginAsync(request);

        //assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("Invalid email or password", result.Error);
        jwtServiceMock.DidNotReceive().GenerateToken(Arg.Any<string>(), Arg.Any<string>());
        hashServiceMock.DidNotReceive().VerifyPassword(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task RegisterAsync_NewUser_ReturnsToken()
    {
        //arrange
        var userDb = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(userDb)
            .Options;

        using var context = new AppDbContext(options);

        var hashServiceMock = Substitute.For<IPasswordHasher>();
        var jwtServiceMock = Substitute.For<IJwtService>();

        jwtServiceMock.GenerateToken(Arg.Any<string>(), Arg.Any<string>()).Returns("fake-jwt-token");
        hashServiceMock.HashPassword(Arg.Any<string>()).Returns("hashedpassword");
        var loggerMock = Substitute.For<ILogger<AuthHandler>>();
        var authHandler = new AuthHandler(jwtServiceMock, context, hashServiceMock, new RegisterMapper(hashServiceMock), loggerMock);
        var request = new RegisterRequest("newuser", "test@gmail.com", "password");

        //act
        var result = await authHandler.RegisterAsync(request);

        //assert   
        Assert.True(result.Success);
        Assert.Equal("fake-jwt-token", result.Data);
        Assert.Null(result.Error);

        var userInDb = await context.Users.FirstOrDefaultAsync(u => u.Email == "test@gmail.com");
        Assert.NotNull(userInDb);
        Assert.Equal("newuser", userInDb.UserName);
        Assert.Equal("hashedpassword", userInDb.PasswordHash);

        hashServiceMock.Received(1).HashPassword("password");
        jwtServiceMock.Received(1).GenerateToken(Arg.Any<string>(), Arg.Any<string>());

    }

    [Fact]
    public async Task GetCurrentUserAsync_ValidId_ReturnsUserDto()
    {
        //arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);

        var userId = Guid.NewGuid().ToString();
        context.Users.Add(new User
        {
            Id = userId,
            UserName = "testuser",
            Email = "test@gmail.com",
            PasswordHash = "hashedpassword"
        });
        await context.SaveChangesAsync();

        var expected = new UserDto
        {
            Id = userId,
            UserName = "testuser",
            Email = "test@gmail.com"
        };

        var hasher = Substitute.For<IPasswordHasher>();
        var authHandler = new AuthHandler(
            Substitute.For<IJwtService>(),
            context,
            hasher,
            new RegisterMapper(hasher),
            Substitute.For<ILogger<AuthHandler>>());

        //act
        var result = await authHandler.GetCurrentUserAsync(userId);

        //assert
        Assert.True(result.Success);
        Assert.Null(result.Error);
        Assert.Equal(expected, result.Data);
    }


[Fact]
    public async Task GetAllUsersAsync_ExcludesCurrentUser()
    {
        //arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
     .UseInMemoryDatabase(Guid.NewGuid().ToString())
     .Options;

        using var context = new AppDbContext(options);

        var hasher = Substitute.For<IPasswordHasher>();
        var authHandler = new AuthHandler(
     Substitute.For<IJwtService>(),
     context,
     hasher,
     new RegisterMapper(hasher),
     Substitute.For<ILogger<AuthHandler>>());

        var users = new List<User>
        {
            new() { Id = "1", UserName = "user1", Email = "user1@gmail.com", PasswordHash = "hash1" },
            new() { Id = "2", UserName = "user2", Email = "user2@gmail.com", PasswordHash = "hash2" },
            new() { Id = "3", UserName = "currentUser", Email = "currentUser@gmail.com", PasswordHash = "hash3" }
        };

        context.Users.AddRange(users);
        await context.SaveChangesAsync();

        //act
        var result = await authHandler.GetUsersExceptCurrentAsync("3");

        //assert
        Assert.True(result.Success);
        Assert.Null(result.Error);
        Assert.NotNull(result.Data);

        Assert.DoesNotContain(result.Data!, u => u.Id == "3");
        Assert.Contains(result.Data!, u => u.Id == "1");
        Assert.Contains(result.Data!, u => u.Id == "2");
        Assert.Equal(2, result.Data!.Count);

    }

}

