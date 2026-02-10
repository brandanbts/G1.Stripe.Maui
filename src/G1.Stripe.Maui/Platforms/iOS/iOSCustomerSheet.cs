using Foundation;
using G1.Stripe.Maui.Options;
using ObjCRuntime;
using Stripe;

namespace G1.Stripe.Maui;

/// <summary>
/// iOS CustomerSheet implementation.
/// Uses Stripe's CustomerSheet (Payment Method Settings Sheet) for managing saved payment methods.
/// </summary>
public class iOSCustomerSheet : ICustomerSheet
{
    public void Initialize(string publishableKey)
    {
        StripeCore.STPAPIClient.SharedClient.PublishableKey = publishableKey;
    }

    public async Task<CustomerSheetResult> PresentAsync(CustomerSheetOptions options, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(options.CustomerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.CustomerSessionClientSecret);

        // If a Stripe Connect account is specified, send requests on behalf of that account.
        if (!string.IsNullOrWhiteSpace(options.StripeAccountId))
        {
            StripeCore.STPAPIClient.SharedClient.StripeAccount = options.StripeAccountId;
        }

        var configuration = BuildConfiguration(options);

        TSPSCustomerSheet customerSheet;
        try
        {
            customerSheet = new TSPSCustomerSheet(
                configuration,
                options.CustomerId,
                options.CustomerSessionClientSecret,
                options.SetupIntentClientSecret);
        }
        catch (ObjCException ex)
        {
            throw new InvalidOperationException(
                "Stripe CustomerSheet failed to initialize. Rebuild the Stripe.Swift.Proxy xcframework: " +
                "from src run 'make build-ios build-simulator create-xcframework', " +
                "then copy xcframework/Stripe.Swift.Proxy.xcframework to G1.Stripe.iOS.Bindings/. See BUILD_INSTRUCTIONS.md. " +
                "Original error: " + ex.Message,
                ex);
        }

        var tcs = new TaskCompletionSource<CustomerSheetResult>();
        using (ct.Register(() => tcs.TrySetCanceled(ct)))
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                customerSheet.PresentFrom(
                    Platform.GetCurrentUIViewController()!,
                    (result, paymentOption, error) =>
                    {
                        switch (result)
                        {
                            case TSPSCustomerSheetResult.Canceled:
                                tcs.SetResult(new CustomerSheetResult.Canceled(ToPaymentOption(paymentOption)));
                                break;
                            case TSPSCustomerSheetResult.Selected:
                                tcs.SetResult(new CustomerSheetResult.Selected(ToPaymentOption(paymentOption)));
                                break;
                            case TSPSCustomerSheetResult.Error:
                                tcs.SetResult(new CustomerSheetResult.Failed(
                                    error != null ? new NSErrorException(error) : new InvalidOperationException("CustomerSheet failed")));
                                break;
                            default:
                                tcs.SetException(new InvalidOperationException($"Unexpected CustomerSheet result: {result}"));
                                break;
                        }
                    });
            }).ConfigureAwait(false);

            return await tcs.Task.ConfigureAwait(false);
        }
    }

    private static TSPSCustomerSheetConfiguration BuildConfiguration(CustomerSheetOptions options)
    {
        var config = new TSPSCustomerSheetConfiguration
        {
            MerchantDisplayName = options.MerchantDisplayName,
            ReturnURL = options.ReturnURL,
            HeaderTextForSelectionScreen = options.HeaderTextForSelectionScreen,
            OnBehalfOf = options.OnBehalfOf
        };
        return config;
    }

    private static CustomerSheetPaymentOption? ToPaymentOption(TSPSCustomerSheetPaymentOption? option)
    {
        if (option == null) return null;
        if (option.IsWallet)
            return new CustomerSheetPaymentOption.Wallet(option.Label);
        return new CustomerSheetPaymentOption.PaymentMethod(option.PaymentMethodId ?? "", option.Label);
    }
}
