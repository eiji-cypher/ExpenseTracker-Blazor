namespace ExpenseTracker.Services;

public enum ToastLevel
{
    Success,
    Error,
    Info
}

public record ToastMessage(Guid Id, string Text, ToastLevel Level);

/// <summary>
/// Tiny scoped service that lets any component raise a notification;
/// ToastHost (in the layout) subscribes and displays them.
/// </summary>
public class ToastService
{
    private readonly object _lock = new();
    private readonly List<ToastMessage> _toasts = new();

    public event Action? OnChange;

    public IReadOnlyList<ToastMessage> Toasts
    {
        get { lock (_lock) return _toasts.ToArray(); }
    }

    public void Show(string text, ToastLevel level = ToastLevel.Success)
    {
        var toast = new ToastMessage(Guid.NewGuid(), text, level);
        lock (_lock) _toasts.Add(toast);
        OnChange?.Invoke();

        // Auto-dismiss after 4 seconds.
        _ = Task.Delay(TimeSpan.FromSeconds(4)).ContinueWith(_ => Dismiss(toast.Id));
    }

    public void Dismiss(Guid id)
    {
        lock (_lock) _toasts.RemoveAll(t => t.Id == id);
        OnChange?.Invoke();
    }
}
