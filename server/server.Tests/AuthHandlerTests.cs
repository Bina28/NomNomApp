using Microsoft.EntityFrameworkCore;
using NSubstitute;
using server.Data;
using server.Domain;
using server.Features.Auth;
using server.Features.Auth.DTOs;

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
        jwtServiceMock.GenereateToken(Arg.Any<string>(), Arg.Any<string>()).Returns("fake-jwt-token");

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testuser",
            PasswordHash = "hashedpassword",
            Email = "test@gmail.com"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        //act
        var authHandler = new AuthHandler(jwtServiceMock, context, hashServiceMock);
        var request = new LoginRequest("test@gmail.com", "password");
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

        //act
        var authHandler = new AuthHandler(jwtServiceMock, context, hashServiceMock);
        var request = new LoginRequest("test@gmail.com", "password");
        var result = await authHandler.LoginAsync(request);

        //assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("User not Found", result.Error);
        jwtServiceMock.DidNotReceive().GenereateToken(Arg.Any<string>(), Arg.Any<string>());

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

        //act
        var authHandler = new AuthHandler(jwtServiceMock, context, hashServiceMock);
        var request = new LoginRequest("someuser@gmail.com", "password");
        var result = await authHandler.LoginAsync(request);

        //assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("User not Found", result.Error);
        jwtServiceMock.DidNotReceive().GenereateToken(Arg.Any<string>(), Arg.Any<string>());
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
   
        jwtServiceMock.GenereateToken(Arg.Any<string>(), Arg.Any<string>()).Returns("fake-jwt-token");
        hashServiceMock.HashPassword(Arg.Any<string>()).Returns("hashedpassword");



        //act
        var authHandler = new AuthHandler(jwtServiceMock, context, hashServiceMock);
        var request = new RegisterRequest("newuser", "test@gmail.com", "password");
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
        jwtServiceMock.Received(1).GenereateToken(Arg.Any<string>(), Arg.Any<string>());

    }

}

