namespace ExpenseTracker.Models;

/// <summary>
/// OOP: Abstraction + Inheritance.
/// Common identity/audit fields that every persisted object shares.
/// Cannot be instantiated directly; concrete models (e.g. Transaction) inherit from it.
/// </summary>
public abstract class Entity
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
