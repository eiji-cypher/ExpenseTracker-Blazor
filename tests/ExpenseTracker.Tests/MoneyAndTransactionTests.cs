using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Helpers;
using ExpenseTracker.Models;
using Xunit;

namespace ExpenseTracker.Tests;

public class MoneyTests
{
    [Fact]
    public void Format_PositiveAmount_FormatsWithPesoSign()
    {
        var result = Money.Format(2650.32m);
        Assert.Equal("₱2,650.32", result);
    }

    [Fact]
    public void Format_NegativeAmount_FormatsWithMinusAndPesoSign()
    {
        var result = Money.Format(-3035.00m);
        Assert.Equal("-₱3,035.00", result);
    }

    [Fact]
    public void Format_ZeroAmount_FormatsProperly()
    {
        var result = Money.Format(0m);
        Assert.Equal("₱0.00", result);
    }

    [Fact]
    public void CurrencyProperties_MatchPhilippinePeso()
    {
        Assert.Equal("₱", Money.Symbol);
        Assert.Equal("PHP", Money.CurrencyCode);
    }
}

public class TransactionTests
{
    [Fact]
    public void SignedAmount_WhenIncome_IsPositive()
    {
        var tx = new Transaction
        {
            Amount = 12340m,
            Type = TransactionType.Income
        };

        Assert.Equal(12340m, tx.SignedAmount);
    }

    [Fact]
    public void SignedAmount_WhenExpense_IsNegative()
    {
        var tx = new Transaction
        {
            Amount = 400m,
            Type = TransactionType.Expense
        };

        Assert.Equal(-400m, tx.SignedAmount);
    }

    [Fact]
    public void FutureDate_FailsValidation()
    {
        var tx = new Transaction
        {
            Amount = 500m,
            Type = TransactionType.Expense,
            Category = Category.Food,
            Date = DateTime.Today.AddDays(1)
        };

        var context = new ValidationContext(tx);
        var results = tx.Validate(context).ToList();

        Assert.Single(results);
        Assert.Contains("future", results[0].ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TodayOrPastDate_PassesValidation()
    {
        var tx = new Transaction
        {
            Amount = 500m,
            Type = TransactionType.Expense,
            Category = Category.Food,
            Date = DateTime.Today
        };

        var context = new ValidationContext(tx);
        var results = tx.Validate(context).ToList();

        Assert.Empty(results);
    }

    [Fact]
    public void Clone_CreatesIndependentCopy()
    {
        var original = new Transaction
        {
            Id = 1,
            Amount = 400m,
            Type = TransactionType.Expense,
            Category = Category.Other,
            Description = "Money Transfer to Thomas Johnson",
            Date = new DateTime(2026, 11, 19),
            IsDeleted = false
        };

        var clone = original.Clone();

        Assert.NotSame(original, clone);
        Assert.Equal(original.Id, clone.Id);
        Assert.Equal(original.Amount, clone.Amount);
        Assert.Equal(original.Description, clone.Description);

        clone.Description = "Modified";
        Assert.NotEqual(original.Description, clone.Description);
    }
}
