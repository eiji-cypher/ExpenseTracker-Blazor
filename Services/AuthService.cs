using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public class AuthService : IAuthService
{
    private readonly INotificationService? _notifications;
    private bool _isAuthenticated = true;
    private string _currentUser = "Eiji";
    private string _userName = "Eiji";
    private string _email = "eiji@eijidev.budget";
    private string _role = "Account Owner";

    public bool IsAuthenticated => _isAuthenticated;
    public string CurrentUser => _currentUser;
    public string UserName => _userName;
    public bool HasConfiguredName => !string.IsNullOrWhiteSpace(_userName);
    public string Email => _email;
    public string Role => _role;

    public event Action? OnAuthStateChanged;

    public AuthService(INotificationService? notifications = null)
    {
        _notifications = notifications;
    }

    public void SetUserName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;
        _userName = name.Trim();
        _currentUser = _userName;
        OnAuthStateChanged?.Invoke();
    }

    public Task<bool> LoginAsync(string usernameOrEmail, string password, bool rememberMe = false)
    {
        if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password))
        {
            return Task.FromResult(false);
        }

        _isAuthenticated = true;
        _currentUser = usernameOrEmail.Contains('@') 
            ? usernameOrEmail.Split('@')[0] 
            : usernameOrEmail;
        _userName = _currentUser;
        _email = usernameOrEmail.Contains('@') ? usernameOrEmail : $"{usernameOrEmail.ToLower()}@eijidev.budget";
        _role = "Account Owner";

        OnAuthStateChanged?.Invoke();
        return Task.FromResult(true);
    }

    public Task LogoutAsync()
    {
        _isAuthenticated = false;
        _currentUser = "Guest";
        _userName = "Guest";
        _email = "";
        _role = "Guest";

        OnAuthStateChanged?.Invoke();
        return Task.CompletedTask;
    }
}
