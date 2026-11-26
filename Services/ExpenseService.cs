using System.Globalization;
using ExpenseTracker.Data;
using ExpenseTracker.Helpers;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services;

public class ExpenseService : IExpenseService
{
    private readonly IDbContextFactory<AppDbContext> _factory;
    private readonly INotificationService? _notifications;
    private List<Transaction> _allTransactions = new();
    private bool _loaded;

    public ExpenseService(IDbContextFactory<AppDbContext> factory, INotificationService? notifications = null)
    {
        _factory = factory;
        _notifications = notifications;
    }

    public event Action? OnChange;

    public IReadOnlyList<Transaction> Transactions =>
        _allTransactions.Where(t => !t.IsDeleted).ToList().AsReadOnly();

    public IReadOnlyList<Transaction> DeletedTransactions =>
        _allTransactions.Where(t => t.IsDeleted).OrderByDescending(t => t.DeletedAt ?? t.CreatedAt).ToList().AsReadOnly();

    public decimal TotalIncome =>
        Transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);

    public decimal TotalExpense =>
        Transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

    public decimal Balance => TotalIncome - TotalExpense;

    public async Task LoadAsync()
    {
        if (_loaded) return;

        await using var db = await _factory.CreateDbContextAsync();
        _allTransactions = await db.Transactions
            .AsNoTracking()
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.Id)
            .ToListAsync();

        _loaded = true;
        Notify();
    }

    public async Task AddAsync(Transaction transaction)
    {
        await LoadAsync();
        Normalize(transaction);
        if (transaction.CreatedAt == default)
        {
            transaction.CreatedAt = DateTime.UtcNow;
        }
        transaction.IsDeleted = false;
        transaction.DeletedAt = null;

        await using var db = await _factory.CreateDbContextAsync();
        db.Transactions.Add(transaction);
        await db.SaveChangesAsync();

        _allTransactions.Add(transaction.Clone());
        SortAndNotify();

        if (transaction.Type == TransactionType.Income)
        {
            _notifications?.Add("Income Entry Added", $"{transaction.Description ?? transaction.Category.ToString()} (+{Money.Format(transaction.Amount)})", NotificationType.IncomeAdded, "history");
        }
        else
        {
            _notifications?.Add("Expense Entry Added", $"{transaction.Description ?? transaction.Category.ToString()} (-{Money.Format(transaction.Amount)})", NotificationType.ExpenseAdded, "history");
        }
    }

    public async Task UpdateAsync(Transaction transaction)
    {
        await LoadAsync();
        Normalize(transaction);

        await using var db = await _factory.CreateDbContextAsync();
        db.Transactions.Update(transaction);
        await db.SaveChangesAsync();

        var index = _allTransactions.FindIndex(t => t.Id == transaction.Id);
        if (index >= 0)
            _allTransactions[index] = transaction.Clone();
        else
            _allTransactions.Add(transaction.Clone());

        SortAndNotify();

        _notifications?.Add("Transaction Modified", $"{transaction.Description ?? transaction.Category.ToString()} updated to {Money.Format(transaction.Amount)}", NotificationType.TransactionUpdated, "history");
    }

    public async Task SoftDeleteAsync(int id)
    {
        await LoadAsync();

        var toDelete = _allTransactions.FirstOrDefault(t => t.Id == id);
        if (toDelete is null) return;

        var desc = toDelete.Description ?? toDelete.Category.ToString();
        var amt = Money.Format(toDelete.Amount);

        await using var db = await _factory.CreateDbContextAsync();
        var entity = await db.Transactions.FindAsync(id);
        if (entity is not null)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }

        toDelete.IsDeleted = true;
        toDelete.DeletedAt = DateTime.UtcNow;
        Notify();

        _notifications?.Add("Moved to Trash", $"Moved \"{desc}\" ({amt}) to Recently Deleted.", NotificationType.TransactionDeleted, "deleted");
    }

    public async Task RestoreAsync(int id)
    {
        await LoadAsync();

        var toRestore = _allTransactions.FirstOrDefault(t => t.Id == id);
        if (toRestore is null) return;

        var desc = toRestore.Description ?? toRestore.Category.ToString();

        await using var db = await _factory.CreateDbContextAsync();
        var entity = await db.Transactions.FindAsync(id);
        if (entity is not null)
        {
            entity.IsDeleted = false;
            entity.DeletedAt = null;
            await db.SaveChangesAsync();
        }

        toRestore.IsDeleted = false;
        toRestore.DeletedAt = null;
        SortAndNotify();

        _notifications?.Add("Transaction Restored", $"Restored \"{desc}\" to active transactions.", NotificationType.TransactionRestored, "history");
    }

    public async Task PurgeAsync(int id)
    {
        await LoadAsync();

        var toPurge = _allTransactions.FirstOrDefault(t => t.Id == id);
        var desc = toPurge?.Description ?? toPurge?.Category.ToString() ?? $"#{id}";

        await using var db = await _factory.CreateDbContextAsync();
        await db.Transactions.Where(t => t.Id == id).ExecuteDeleteAsync();

        _allTransactions.RemoveAll(t => t.Id == id);
        Notify();

        _notifications?.Add("Transaction Purged", $"Permanently deleted \"{desc}\".", NotificationType.TransactionPurged, "deleted");
    }

    public async Task ClearAllDeletedAsync()
    {
        await LoadAsync();

        await using var db = await _factory.CreateDbContextAsync();
        await db.Transactions.Where(t => t.IsDeleted).ExecuteDeleteAsync();

        _allTransactions.RemoveAll(t => t.IsDeleted);
        Notify();

        _notifications?.Add("Trash Emptied", "All soft-deleted records have been permanently cleared.", NotificationType.TransactionPurged, "deleted");
    }

    public async Task ClearAllRecordsAsync()
    {
        await LoadAsync();

        await using var db = await _factory.CreateDbContextAsync();
        await db.Transactions.ExecuteDeleteAsync();

        _allTransactions.Clear();
        Notify();

        _notifications?.Add("All Data Cleared", "All transactions were completely cleared.", NotificationType.System, "dashboard");
    }

    public Task DeleteAsync(int id) => SoftDeleteAsync(id);

    public IEnumerable<Transaction> Filter(string? keyword, Category? category, TransactionType? type)
    {
        IEnumerable<Transaction> query = Transactions;

        if (category is { } c)
            query = query.Where(t => t.Category == c);

        if (type is { } ty)
            query = query.Where(t => t.Type == ty);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var k = keyword.Trim();
            query = query.Where(t =>
                (t.Description?.Contains(k, StringComparison.OrdinalIgnoreCase) ?? false) ||
                t.Category.ToString().Contains(k, StringComparison.OrdinalIgnoreCase) ||
                t.Amount.ToString(CultureInfo.InvariantCulture).Contains(k));
        }

        return query;
    }

    private static void Normalize(Transaction t)
    {
        t.Date = t.Date.Date;
        t.Description = string.IsNullOrWhiteSpace(t.Description) ? null : t.Description.Trim();
    }

    private void SortAndNotify()
    {
        _allTransactions = _allTransactions
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.Id)
            .ToList();
        Notify();
    }

    private void Notify() => OnChange?.Invoke();
}
