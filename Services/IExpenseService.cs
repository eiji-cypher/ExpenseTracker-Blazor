using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public interface IExpenseService
{
    IReadOnlyList<Transaction> Transactions { get; }
    IReadOnlyList<Transaction> DeletedTransactions { get; }

    decimal TotalIncome { get; }
    decimal TotalExpense { get; }
    decimal Balance { get; }

    event Action? OnChange;

    Task LoadAsync();
    Task AddAsync(Transaction transaction);
    Task UpdateAsync(Transaction transaction);
    Task SoftDeleteAsync(int id);
    Task RestoreAsync(int id);
    Task PurgeAsync(int id);
    Task ClearAllDeletedAsync();
    Task ClearAllRecordsAsync();
    Task DeleteAsync(int id);

    IEnumerable<Transaction> Filter(string? keyword, Category? category, TransactionType? type);
}
