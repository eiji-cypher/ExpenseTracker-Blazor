using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Xunit;

namespace ExpenseTracker.Tests;

public class NotificationServiceTests
{
    [Fact]
    public void InitialState_IsCleanWithZeroMockNotifications()
    {
        var service = new NotificationService();
        Assert.Empty(service.Notifications);
        Assert.Equal(0, service.UnreadCount);
    }

    [Fact]
    public void Add_InsertsNewNotificationAtTop()
    {
        var service = new NotificationService();
        service.Add("Salary Credited", "Income of ₱15,000 received", NotificationType.IncomeAdded, "history");

        Assert.Single(service.Notifications);
        var first = service.Notifications[0];
        Assert.Equal("Salary Credited", first.Title);
        Assert.Equal(NotificationType.IncomeAdded, first.Type);
        Assert.False(first.IsRead);
        Assert.Equal(1, service.UnreadCount);
    }

    [Fact]
    public void MarkAsRead_UpdatesIsReadAndDecrementsUnreadCount()
    {
        var service = new NotificationService();
        service.Add("Deposit", "Credited", NotificationType.IncomeAdded);
        service.Add("Rent", "Paid", NotificationType.ExpenseAdded);

        var unreadItem = service.Notifications.First(n => !n.IsRead);
        var prevUnreadCount = service.UnreadCount;

        service.MarkAsRead(unreadItem.Id);

        Assert.True(unreadItem.IsRead);
        Assert.Equal(prevUnreadCount - 1, service.UnreadCount);
    }

    [Fact]
    public void MarkAllAsRead_SetsUnreadCountToZero()
    {
        var service = new NotificationService();
        service.Add("Deposit", "Credited", NotificationType.IncomeAdded);
        service.Add("Rent", "Paid", NotificationType.ExpenseAdded);
        Assert.True(service.UnreadCount > 0);

        service.MarkAllAsRead();

        Assert.Equal(0, service.UnreadCount);
        Assert.All(service.Notifications, n => Assert.True(n.IsRead));
    }

    [Fact]
    public void ClearAll_RemovesAllNotifications()
    {
        var service = new NotificationService();
        service.Add("Deposit", "Credited", NotificationType.IncomeAdded);
        Assert.NotEmpty(service.Notifications);

        service.ClearAll();

        Assert.Empty(service.Notifications);
        Assert.Equal(0, service.UnreadCount);
    }
}
