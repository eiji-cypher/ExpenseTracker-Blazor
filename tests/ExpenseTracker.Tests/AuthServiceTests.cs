using ExpenseTracker.Services;
using Xunit;

namespace ExpenseTracker.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_ValidCredentials_SetsAuthenticatedTrue()
    {
        var auth = new AuthService();
        await auth.LogoutAsync();
        Assert.False(auth.IsAuthenticated);

        var success = await auth.LoginAsync("admin@eijidev.com", "Password123!");
        Assert.True(success);
        Assert.True(auth.IsAuthenticated);
        Assert.Equal("admin", auth.CurrentUser);
    }

    [Fact]
    public async Task LoginAsync_EmptyCredentials_ReturnsFalse()
    {
        var auth = new AuthService();
        await auth.LogoutAsync();

        var success = await auth.LoginAsync("", "");
        Assert.False(success);
        Assert.False(auth.IsAuthenticated);
    }

    [Fact]
    public async Task LogoutAsync_ClearsAuthentication()
    {
        var auth = new AuthService();
        Assert.True(auth.IsAuthenticated);

        await auth.LogoutAsync();
        Assert.False(auth.IsAuthenticated);
        Assert.Equal("Guest", auth.CurrentUser);
    }
}

