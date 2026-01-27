using System.Runtime.Versioning;
using Foundation;

namespace G1.Stripe.Maui.Demo
{
    [RequiresPreviewFeatures]
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}