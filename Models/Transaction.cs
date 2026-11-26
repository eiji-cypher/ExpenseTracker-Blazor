using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models;

public class Transaction : Entity, IValidatableObject
{
    [Range(0.01, 1000000000.0, ErrorMessage = "Please enter an amount greater than ₱0.00")]
    public decimal Amount { get; set; }

    public TransactionType Type { get; set; } = TransactionType.Expense;

    public Category Category { get; set; } = Category.Food;

    [Required(ErrorMessage = "Please pick a date.")]
    public DateTime Date { get; set; } = DateTime.Today;

    [StringLength(200, ErrorMessage = "Description can be at most 200 characters.")]
    public string? Description { get; set; }

    public bool IsDeleted { get; set; } = false;

    public DateTime? DeletedAt { get; set; }

    public decimal SignedAmount => Type == TransactionType.Income ? Amount : -Amount;

    public string RecordTimeFormatted =>
        (CreatedAt != default ? CreatedAt.ToLocalTime() : DateTime.Now).ToString("hh:mm tt");

    public Transaction Clone() => (Transaction)MemberwiseClone();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Date.Date > DateTime.Today)
        {
            yield return new ValidationResult(
                "Transaction date cannot be in the future.",
                new[] { nameof(Date) });
        }
    }
}
