namespace G1.Stripe.Maui;

/// <summary>
/// Result of presenting the CustomerSheet (Payment Method Settings Sheet).
/// </summary>
public abstract record CustomerSheetResult
{
    /// <summary>
    /// User cancelled the sheet. Selection may include the previous payment option.
    /// </summary>
    public record Canceled(CustomerSheetPaymentOption? PreviousSelection) : CustomerSheetResult;

    /// <summary>
    /// User selected a payment method. May be null if they removed the last one.
    /// </summary>
    public record Selected(CustomerSheetPaymentOption? Selection) : CustomerSheetResult;

    /// <summary>
    /// An error occurred.
    /// </summary>
    public record Failed(Exception Error) : CustomerSheetResult;
}

/// <summary>
/// Represents a payment option in CustomerSheet (card, Apple Pay, etc.).
/// </summary>
public abstract record CustomerSheetPaymentOption
{
    /// <summary>
    /// A saved payment method (e.g. card).
    /// </summary>
    public record PaymentMethod(string PaymentMethodId, string? Label) : CustomerSheetPaymentOption;

    /// <summary>
    /// Apple Pay (iOS) or Google Pay (Android).
    /// </summary>
    public record Wallet(string Label) : CustomerSheetPaymentOption;
}
