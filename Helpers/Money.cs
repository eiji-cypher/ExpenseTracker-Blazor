using System.Globalization;

namespace ExpenseTracker.Helpers;

public static class Money
{
    // Philippine Peso symbol and currency code
    public const string Symbol = "₱";
    public const string CurrencyCode = "PHP";

    public static string Format(decimal value)
    {
        var sign = value < 0 ? "-" : "";
        return sign + Symbol + Math.Abs(value).ToString("N2", CultureInfo.InvariantCulture);
    }
}
