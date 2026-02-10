namespace G1.Stripe.Maui.Options;

/// <summary>
/// Configuration for CustomerSheet (Payment Method Settings Sheet).
/// Used for managing saved payment methods in app settings.
/// See: https://docs.stripe.com/elements/customer-sheet
/// </summary>
public class CustomerSheetOptions
{
    /// <summary>
    /// Merchant/business name displayed in the sheet.
    /// </summary>
    public required string MerchantDisplayName { get; set; }

    /// <summary>
    /// Customer ID from your backend (e.g. cus_xxx).
    /// Create via POST /v1/customers or use an existing customer.
    /// </summary>
    public required string CustomerId { get; set; }

    /// <summary>
    /// CustomerSession client secret from your backend (starts with cuss_).
    /// Create via POST /v1/customer_sessions with the customer ID.
    /// </summary>
    public required string CustomerSessionClientSecret { get; set; }

    /// <summary>
    /// SetupIntent client secret when the user adds a new payment method (starts with seti_).
    /// Create via POST /v1/setup_intents with customer={{CustomerId}}.
    /// Required if users can add payment methods. Supply one per sheet session; creating another
    /// requires closing and reopening the sheet.
    /// </summary>
    public string? SetupIntentClientSecret { get; set; }

    /// <summary>
    /// Header text for the payment method selection screen.
    /// Default: "Manage your payment method"
    /// </summary>
    public string? HeaderTextForSelectionScreen { get; set; }

    /// <summary>
    /// Optional: Return URL for redirect-based payment flows (e.g. SEPA, bank redirects).
    /// </summary>
    public string? ReturnURL { get; set; }

    /// <summary>
    /// Optional: Stripe Connect account ID when saving on behalf of another account.
    /// </summary>
    public string? OnBehalfOf { get; set; }

    /// <summary>
    /// Optional: Stripe Connect account ID when using CustomerSheet against a connected account.
    /// When set, the iOS implementation configures <c>STPAPIClient.SharedClient.StripeAccount</c>
    /// so that the SDK talks directly to that connected account (matching your backend's
    /// <c>Stripe-Account</c> header).
    /// </summary>
    public string? StripeAccountId { get; set; }

    /// <summary>
    /// Optional: Payment method types to show (e.g. ["card", "us_bank_account", "sepa_debit"]).
    /// If null, defaults from Stripe Dashboard are used.
    /// </summary>
    public List<string>? PaymentMethodTypes { get; set; }
}
