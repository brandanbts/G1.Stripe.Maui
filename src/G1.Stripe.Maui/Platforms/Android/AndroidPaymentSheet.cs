using AndroidX.Activity;
using Com.Stripe.Android;
using Com.Stripe.Android.Paymentsheet;
using G1.Stripe.Maui.Options;
using AndroidPaymentSheetResult = Com.Stripe.Android.Paymentsheet.PaymentSheetResult;
using SharedPSResult = G1.Stripe.Maui.PaymentSheetResult;

namespace G1.Stripe.Maui;

public class AndroidPaymentSheet : IPaymentSheet
{
    internal void CaptureActivity(ComponentActivity activity)
    {
        _capturedActivity = activity;
        _sheet = new PaymentSheet.Builder(new ResultCallback(this)).Build(activity);
    }

    private ComponentActivity? _capturedActivity;

    private PaymentSheet? _sheet;

    private TaskCompletionSource<SharedPSResult>? _tcs;
    
    private string? _publishableKey;

    public void Initialize(string publishableKey)
    {
        ArgumentNullException.ThrowIfNull(_capturedActivity);
        _publishableKey = publishableKey;
        PaymentConfiguration.Init(_capturedActivity, publishableKey);
    }

    public async Task<SharedPSResult> Open(PaymentSheetOptions options, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(_sheet);

        // If a Stripe Connect account is specified, reinitialize PaymentConfiguration with the account ID
        // In Stripe Android SDK, the account ID needs to be set when initializing PaymentConfiguration
        if (!string.IsNullOrWhiteSpace(options.StripeAccountId) && !string.IsNullOrWhiteSpace(_publishableKey))
        {
            // Reinitialize PaymentConfiguration with the Stripe account ID for Connect scenarios
            PaymentConfiguration.Init(_capturedActivity!, _publishableKey, options.StripeAccountId);
        }

        var configuration = options.BuildPlatform();

        _tcs = new TaskCompletionSource<SharedPSResult>();
        using (ct.Register(() => _tcs.TrySetCanceled(ct)))
        {
            _sheet.PresentWithPaymentIntent(options.ClientSecret, configuration);
            return await _tcs.Task.ConfigureAwait(false);
        }
    }

    private class ResultCallback(AndroidPaymentSheet sheet) : Java.Lang.Object, IPaymentSheetResultCallback
    {
        public void OnPaymentSheetResult(AndroidPaymentSheetResult paymentSheetResult)
        {
            switch (paymentSheetResult)
            {
                case AndroidPaymentSheetResult.Canceled c:
                    sheet._tcs?.SetResult(new SharedPSResult.Canceled());
                    break;

                case AndroidPaymentSheetResult.Failed f:
                    sheet._tcs?.SetResult(new SharedPSResult.Failed(f.Error));
                    break;

                case AndroidPaymentSheetResult.Completed completed:
                    sheet._tcs?.SetResult(new SharedPSResult.Completed());
                    break;

                default:
                    sheet._tcs?.SetException(new ImpossiblePaymentSheetException("Result didnt match one of excpected cases"));
                    break;
            }
        }
    }
}