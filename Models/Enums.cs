namespace ExpenseTracker.Models;

public enum TransactionType
{
    Income,
    Expense
}

public enum Category
{
    Food,
    Rent,
    Transport,
    Utilities,
    Leisure,
    Other,
    Salary,
    Allowance,
    Investment,
    Freelance,
    Bonus
}

public static class CategoryExtensions
{
    public static readonly Category[] ExpenseCategories = new[]
    {
        Category.Food,
        Category.Rent,
        Category.Transport,
        Category.Utilities,
        Category.Leisure,
        Category.Other
    };

    public static readonly Category[] IncomeCategories = new[]
    {
        Category.Salary,
        Category.Allowance,
        Category.Freelance,
        Category.Investment,
        Category.Bonus,
        Category.Other
    };
}
