using G1.Stripe.Maui.Options;

namespace G1.Stripe.Maui;

/// <summary>
/// Android CustomerSheet implementation.
/// Full implementation requires bridging async Kotlin providers to C#. See docs/CUSTOMER_SHEET_IMPLEMENTATION_PLAN.md.
/// </summary>
public class AndroidCustomerSheet : ICustomerSheet
{
    public void Initialize(string publishableKey)
    {
        // PaymentConfiguration is shared with PaymentSheet
        // Will be initialized when PaymentSheet is used, or caller can init separately
    }

    public Task<CustomerSheetResult> PresentAsync(CustomerSheetOptions options, CancellationToken ct = default)
    {
        _ = options; // Not used yet
        throw new NotImplementedException(
            "CustomerSheet for Android is not yet implemented. The Stripe CustomerSheet requires async providers " +
            "(CustomerSessionClientSecret and SetupIntentClientSecret) that must be bridged from Kotlin to C#. " +
            "See docs/CUSTOMER_SHEET_IMPLEMENTATION_PLAN.md for the implementation plan. " +
            "For managing payment methods in checkout, use PaymentSheet with SetupIntentClientSecret.");
    }
}
