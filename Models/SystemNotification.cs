namespace ExpenseTracker.Models;

public enum NotificationType
{
    IncomeAdded,
    ExpenseAdded,
    TransactionUpdated,
    TransactionDeleted,
    TransactionRestored,
    TransactionPurged,
    Authentication,
    System
}

public class SystemNotification
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.System;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public bool IsRead { get; set; } = false;
    public string? LinkUrl { get; set; }

    public string TimeAgo
    {
        get
        {
            var span = DateTime.Now - Timestamp;
            if (span.TotalSeconds < 60) return "Just now";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes}m ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours}h ago";
            if (span.TotalDays < 7) return $"{(int)span.TotalDays}d ago";
            return Timestamp.ToString("MMM dd");
        }
    }
}
