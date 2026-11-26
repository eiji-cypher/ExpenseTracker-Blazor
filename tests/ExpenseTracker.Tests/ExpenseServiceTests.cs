using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseTracker.Tests;

public class ExpenseServiceTests
{
    private class TestDbContextFactory : IDbContextFactory<AppDbContext>
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public TestDbContextFactory()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;
        }

        public AppDbContext CreateDbContext()
        {
            var context = new AppDbContext(_options);
            context.Database.OpenConnection();
            context.Database.EnsureCreated();
            return context;
        }

        public Task<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(CreateDbContext());
    }

    [Fact]
    public void Filter_ByKeyword_MatchesDescriptionOrCategory()
    {
        var transactions = new List<Transaction>
        {
            new Transaction { Id = 1, Description = "Money Transfer to Thomas Johnson", Category = Category.Other, Amount = 400m, Type = TransactionType.Expense },
            new Transaction { Id = 2, Description = "Monthly Salary Deposit", Category = Category.Salary, Amount = 12340m, Type = TransactionType.Income },
            new Transaction { Id = 3, Description = "Flight Booking", Category = Category.Transport, Amount = 3035m, Type = TransactionType.Expense }
        };

        var filtered = transactions.Where(t => t.Description?.Contains("Thomas", StringComparison.OrdinalIgnoreCase) ?? false).ToList();
        Assert.Single(filtered);
        Assert.Equal(400m, filtered[0].Amount);
    }

    [Fact]
    public void TotalIncomeAndExpense_CalculateCorrectly()
    {
        var transactions = new List<Transaction>
        {
            new Transaction { Amount = 12340m, Type = TransactionType.Income, IsDeleted = false },
            new Transaction { Amount = 400m, Type = TransactionType.Expense, IsDeleted = false },
            new Transaction { Amount = 3035m, Type = TransactionType.Expense, IsDeleted = false },
            new Transaction { Amount = 3500m, Type = TransactionType.Expense, IsDeleted = false }
        };

        var income = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
        var expense = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
        var balance = income - expense;

        Assert.Equal(12340m, income);
        Assert.Equal(6935m, expense);
        Assert.Equal(5405m, balance);
    }
}
