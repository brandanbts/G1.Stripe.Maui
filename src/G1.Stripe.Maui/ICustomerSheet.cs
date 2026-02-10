using G1.Stripe.Maui.Options;

namespace G1.Stripe.Maui;

/// <summary>
/// CustomerSheet (Payment Method Settings Sheet) for managing saved payment methods.
/// Use in app settings—add, remove, and set default payment method.
/// See: https://docs.stripe.com/elements/customer-sheet
/// </summary>
public interface ICustomerSheet
{
    /// <summary>
    /// Initialize with your Stripe publishable key.
    /// </summary>
    void Initialize(string publishableKey);

    /// <summary>
    /// Present the CustomerSheet to manage payment methods.
    /// </summary>
    Task<CustomerSheetResult> PresentAsync(CustomerSheetOptions options, CancellationToken ct = default);
}
