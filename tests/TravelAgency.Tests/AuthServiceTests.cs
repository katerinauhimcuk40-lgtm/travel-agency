using TravelAgency.Infrastructure.Repositories;
using TravelAgency.Infrastructure.Services;

namespace TravelAgency.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_ShouldHashPasswordAndCreateUser()
    {
        // Arrange
        using var context = TestDb.CreateContext();
        var authService = new AuthService(new UnitOfWork(context));
        string rawPassword = "SecurePassword123!";

        // Act - додано null для параметра phone
        var user = await authService.RegisterAsync("user@example.com", rawPassword, "Test User", null);

        // Assert
        Assert.NotNull(user);
        Assert.Equal("user@example.com", user.Email);
        Assert.NotEqual(rawPassword, user.PasswordHash);
        Assert.Single(context.Users);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldSucceed()
    {
        // Arrange
        using var context = TestDb.CreateContext();
        var authService = new AuthService(new UnitOfWork(context));
        string rawPassword = "SecurePassword123!";

        await authService.RegisterAsync("login@example.com", rawPassword, "Login User", null);

        // Act
        var loggedInUser = await authService.LoginAsync("login@example.com", rawPassword);

        // Assert
        Assert.NotNull(loggedInUser);
        Assert.Equal("login@example.com", loggedInUser.Email);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldThrowException()
    {
        // Arrange
        using var context = TestDb.CreateContext();
        var authService = new AuthService(new UnitOfWork(context));

        await authService.RegisterAsync("login@example.com", "SecurePassword123!", "Login User", null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => authService.LoginAsync("login@example.com", "WrongPassword123!"));
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentEmail_ShouldThrowException()
    {
        // Arrange
        using var context = TestDb.CreateContext();
        var authService = new AuthService(new UnitOfWork(context));

        // Act & Assert - сервіс повертає UnauthorizedAccessException для безпеки
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => authService.LoginAsync("ghost@example.com", "AnyPassword123!"));
    }
}