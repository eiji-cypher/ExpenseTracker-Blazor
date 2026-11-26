using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public interface INotificationService
{
    IReadOnlyList<SystemNotification> Notifications { get; }
    int UnreadCount { get; }
    event Action? OnChange;

    void Add(string title, string message, NotificationType type, string? linkUrl = null);
    void MarkAsRead(string id);
    void MarkAllAsRead();
    void ClearAll();
}
