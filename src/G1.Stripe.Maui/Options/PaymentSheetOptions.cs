namespace G1.Stripe.Maui.Options;

/// <summary>
/// Configuration options for Stripe PaymentSheet, a prebuilt checkout UI for Android and iOS.
/// </summary>
public partial class PaymentSheetOptions
{
    /// <summary>
    /// Optional Stripe Connect account ID used to perform the payment on behalf of
    /// another account. When set, the underlying Stripe SDK will send requests
    /// with this account context (where supported).
    ///
    /// Note: For this to work, your backend must create the PaymentIntent for
    /// the same connected account (using the Stripe-Account header or the
    /// connected account's secret key).
    /// </summary>
    public string? StripeAccountId { get; set; }

    /// <summary>
    /// Google Pay configuration. If null, Google Pay is disabled.
    /// </summary>
    public GooglePayOptions? GooglePay { get; set; }

    /// <summary>
    /// Apple Pay configuration. If null, Apple Pay is disabled.
    /// </summary>
    public ApplePayOptions? ApplePay { get; set; }

    /// <summary>
    /// The client secret of the PaymentIntent or SetupIntent created on your server.
    /// Required to initialize and present the PaymentSheet.
    /// </summary>
    public required string ClientSecret { get; set; }

    /// <summary>
    /// Merchant name displayed at the top of the PaymentSheet and in Apple/Google Pay.
    /// Default: Your Stripe account’s business name.
    /// </summary>
    public required string MerchantDisplayName { get; set; }

    /// <summary>
    /// Whether to allow payment methods that can require delayed confirmation,
    /// such as SEPA or Sofort.
    /// Default: false.
    /// </summary>
    public bool AllowsDelayedPaymentMethods { get; set; }

    /// <summary>
    /// Information about the customer, enabling returning users to see and reuse
    /// saved payment methods.
    /// Optional — omit if the user is a guest.
    /// </summary>
    public PaymentSheetCustomerOptions? Customer { get; set; }

    /// <summary>
    /// Controls how much billing information the PaymentSheet collects (e.g., name,
    /// email, phone, address) and which fields are required.
    /// Default: Stripe decides automatically based on payment method and country.
    /// </summary>
    public PaymentSheetBillingDetailsCollectionOptions? BillingDetails { get; set; }

    /// <summary>
    /// Visual styling for the PaymentSheet UI (colors, typography, corner radius,
    /// primary button style, etc.).
    /// Default: Stripe’s built-in light/dark theme.
    /// </summary>
    public PaymentSheetAppearanceOptions? Appearance { get; set; }

    /// <summary>
    /// Custom label text for the primary call-to-action button.
    /// Only supported on iOS.
    /// Default: “Pay”.
    /// </summary>
    public string? PrimaryButtonLabel { get; set; }

    /// <summary>
    /// Limits the set of payment methods shown to the user.
    /// Example: <c>["card", "bancontact", "ideal"]</c>.
    /// Default: all supported payment methods for your account and region.
    /// </summary>
    public List<string>? PaymentMethodTypes { get; set; }

    /// <summary>
    /// Whether to use PaymentSheet in custom flow mode, allowing manual confirmation
    /// of the PaymentIntent/SetupIntent after collecting payment details.
    /// Advanced option. Default: false.
    /// </summary>
    public bool? CustomFlow { get; set; }

    /// <summary>
    /// Indicates if the payment method should be saved for future use.
    /// Default: not saved unless required by payment method type.
    /// </summary>
    public bool? SetupFutureUsage { get; set; }

    /// <summary>
    /// Intent configuration for sending payment intents on behalf of another Stripe account (Connect).
    /// Optional — omit if not using Stripe Connect.
    /// </summary>
    public PaymentSheetIntentConfigurationOptions? IntentConfiguration { get; set; }
}

/// <summary>
/// Configuration for sending payment intents on behalf of another Stripe account (Connect).
/// </summary>
public class PaymentSheetIntentConfigurationOptions
{
    /// <summary>
    /// The Stripe account ID to send the payment intent on behalf of (Connect account ID).
    /// Required for Connect scenarios.
    /// </summary>
    public required string OnBehalfOf { get; set; }
}

public class GooglePayOptions
{
    /// <summary>
    /// ISO country code used for Google Pay.
    /// Default: "us".
    /// </summary>
    public string CountryCode { get; set; } = "us";

    /// <summary>
    /// Whether to use the Google Pay test environment.
    /// Default: true.
    /// </summary>
    public bool UseTestEnvironment { get; set; } = true;
}

public class ApplePayOptions
{
    /// <summary>
    /// Apple Pay merchant identifier (e.g. "your.merchant.id").
    /// </summary>
    public required string MerchantId { get; set; }

    /// <summary>
    /// ISO country code used for Apple Pay.
    /// Default: "us".
    /// </summary>
    public string CountryCode { get; set; } = "us";
}