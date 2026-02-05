using Foundation;
using G1.Stripe.Maui.Options;
using ObjCRuntime;
using Stripe;

namespace G1.Stripe.Maui;

public class iOSPaymentSheet : IPaymentSheet
{
    public void Initialize(string publishableKey)
    {
        StripeCore.STPAPIClient.SharedClient.PublishableKey = publishableKey;
    }

    public async Task<PaymentSheetResult> Open(PaymentSheetOptions options, CancellationToken ct = default)
    {
        var (clientSecret, isSetupIntent) = GetIntentSecretAndMode(options);

        // If a Stripe Connect account is specified, send requests on behalf of that account
        if (!string.IsNullOrWhiteSpace(options.StripeAccountId))
        {
            StripeCore.STPAPIClient.SharedClient.StripeAccount = options.StripeAccountId;
        }

        var configuration = options.BuildPlatform();

        TSPSPaymentSheet ps;
        try
        {
            // Use separate entry points to avoid enum marshalling issues on device (32/64-bit).
            ps = isSetupIntent
                ? TSPSPaymentSheet.CreateWithSetupIntent(clientSecret, configuration)
                : new TSPSPaymentSheet(clientSecret, configuration);
        }
        catch (ObjCException ex)
        {
            throw new InvalidOperationException(
                "Stripe payment sheet failed (native iOS). If you are using PaymentIntentClientSecret or SetupIntentClientSecret, " +
                "rebuild the Stripe.Swift.Proxy xcframework: from src run 'make build-ios build-simulator create-xcframework', " +
                "then copy xcframework/Stripe.Swift.Proxy.xcframework to G1.Stripe.iOS.Bindings/. See BUILD_INSTRUCTIONS.md. " +
                "Original error: " + ex.Message,
                ex);
        }

        var tcs = new TaskCompletionSource<PaymentSheetResult>();
        using (ct.Register(() => tcs.TrySetCanceled(ct)))
        {
            await MainThread.InvokeOnMainThreadAsync(() => ps.PresentFrom(Platform.GetCurrentUIViewController()!, (res, error) => OnPaymentSheetResult(res, error, tcs))).ConfigureAwait(false);
            return await tcs.Task.ConfigureAwait(false);
        }
    }

    private void OnPaymentSheetResult(TSPSPaymentSheetResult paymentSheetResult, NSError? error, TaskCompletionSource<PaymentSheetResult> tcs)
    {

        switch (paymentSheetResult)
        {
            case TSPSPaymentSheetResult.Canceled:
                tcs.SetResult(new PaymentSheetResult.Canceled());
                break;

            case TSPSPaymentSheetResult.Failed:
                tcs.SetResult(new PaymentSheetResult.Failed(ToException(error)));
                break;

            case TSPSPaymentSheetResult.Completed:
                tcs.SetResult(new PaymentSheetResult.Completed());
                break;

            default:
                tcs.SetException(new ImpossiblePaymentSheetException("Result didnt match one of excpected cases"));
                break;

        }
    }

    private static Exception ToException(NSError? error)
    {
        return error is null 
            ? new ImpossiblePaymentSheetException("Internal error occured in stripe payment sheet") //should never be a case
            : new NSErrorException(error);
    }

    private static (string clientSecret, bool isSetupIntent) GetIntentSecretAndMode(PaymentSheetOptions options)
    {
        var hasSetup = !string.IsNullOrWhiteSpace(options.SetupIntentClientSecret);
        var paymentSecret = options.PaymentIntentClientSecret ?? options.ClientSecret;
        var hasPayment = !string.IsNullOrWhiteSpace(paymentSecret);

        if (hasSetup && hasPayment)
            throw new ArgumentException("Set either PaymentIntent client secret (ClientSecret or PaymentIntentClientSecret) or SetupIntentClientSecret, not both.", nameof(options));
        if (!hasSetup && !hasPayment)
            throw new ArgumentException("Set either PaymentIntent client secret (ClientSecret or PaymentIntentClientSecret) or SetupIntentClientSecret.", nameof(options));

        if (hasSetup)
            return (options.SetupIntentClientSecret!, true);
        return (paymentSecret!, false);
    }
}
