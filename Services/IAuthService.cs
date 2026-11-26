namespace ExpenseTracker.Services;

public interface IAuthService
{
    bool IsAuthenticated { get; }
    string CurrentUser { get; }
    string UserName { get; }
    bool HasConfiguredName { get; }
    string Email { get; }
    string Role { get; }
    event Action? OnAuthStateChanged;

    void SetUserName(string name);
    Task<bool> LoginAsync(string usernameOrEmail, string password, bool rememberMe = false);
    Task LogoutAsync();
}
