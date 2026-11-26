using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services;

public class NotificationService : INotificationService
{
    private readonly IDbContextFactory<AppDbContext>? _factory;
    private readonly List<SystemNotification> _notifications = new();
    private readonly object _lock = new();
    private bool _loaded;

    public event Action? OnChange;

    public IReadOnlyList<SystemNotification> Notifications
    {
        get
        {
            EnsureLoaded();
            lock (_lock)
            {
                return _notifications.ToList().AsReadOnly();
            }
        }
    }

    public int UnreadCount
    {
        get
        {
            EnsureLoaded();
            lock (_lock)
            {
                return _notifications.Count(n => !n.IsRead);
            }
        }
    }

    public NotificationService()
    {
        // Parameterless constructor for tests and lightweight in-memory execution
        _loaded = true;
    }

    public NotificationService(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
        EnsureLoaded();
    }

    private void EnsureLoaded()
    {
        if (_loaded || _factory is null) return;

        lock (_lock)
        {
            if (_loaded) return;
            try
            {
                using var db = _factory.CreateDbContext();
                db.Database.ExecuteSqlRaw(@"
                    CREATE TABLE IF NOT EXISTS Notifications (
                        Id TEXT PRIMARY KEY,
                        Title TEXT NOT NULL,
                        Message TEXT NOT NULL,
                        Type TEXT NOT NULL,
                        Timestamp TEXT NOT NULL,
                        IsRead INTEGER NOT NULL,
                        LinkUrl TEXT
                    );");

                var items = db.Notifications
                    .AsNoTracking()
                    .OrderByDescending(n => n.Timestamp)
                    .Take(100)
                    .ToList();

                _notifications.Clear();
                _notifications.AddRange(items);
                _loaded = true;
            }
            catch
            {
                // Fallback to in-memory mode if DB is temporarily locked
                _loaded = true;
            }
        }
    }

    public void Add(string title, string message, NotificationType type, string? linkUrl = null)
    {
        EnsureLoaded();

        var item = new SystemNotification
        {
            Title = title,
            Message = message,
            Type = type,
            Timestamp = DateTime.Now,
            IsRead = false,
            LinkUrl = linkUrl
        };

        lock (_lock)
        {
            _notifications.Insert(0, item);
        }

        Notify();

        if (_factory is not null)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await using var db = await _factory.CreateDbContextAsync();
                    db.Notifications.Add(item);
                    await db.SaveChangesAsync();
                }
                catch
                {
                    // Non-critical logging catch
                }
            });
        }
    }

    public void MarkAsRead(string id)
    {
        EnsureLoaded();

        bool updated = false;
        lock (_lock)
        {
            var item = _notifications.FirstOrDefault(n => n.Id == id);
            if (item is not null && !item.IsRead)
            {
                item.IsRead = true;
                updated = true;
            }
        }

        if (updated)
        {
            Notify();

            if (_factory is not null)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await using var db = await _factory.CreateDbContextAsync();
                        var entity = await db.Notifications.FindAsync(id);
                        if (entity is not null)
                        {
                            entity.IsRead = true;
                            await db.SaveChangesAsync();
                        }
                    }
                    catch { }
                });
            }
        }
    }

    public void MarkAllAsRead()
    {
        EnsureLoaded();

        bool changed = false;
        lock (_lock)
        {
            foreach (var item in _notifications)
            {
                if (!item.IsRead)
                {
                    item.IsRead = true;
                    changed = true;
                }
            }
        }

        if (changed)
        {
            Notify();

            if (_factory is not null)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await using var db = await _factory.CreateDbContextAsync();
                        await db.Database.ExecuteSqlRawAsync("UPDATE Notifications SET IsRead = 1;");
                    }
                    catch { }
                });
            }
        }
    }

    public void ClearAll()
    {
        EnsureLoaded();

        lock (_lock)
        {
            if (_notifications.Count == 0) return;
            _notifications.Clear();
        }

        Notify();

        if (_factory is not null)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await using var db = await _factory.CreateDbContextAsync();
                    await db.Database.ExecuteSqlRawAsync("DELETE FROM Notifications;");
                }
                catch { }
            });
        }
    }

    private void Notify() => OnChange?.Invoke();
}
