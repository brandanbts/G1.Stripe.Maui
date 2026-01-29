# Building and Using Local Packages with IntentConfiguration Support

## Prerequisites
- macOS (for building iOS Swift proxy)
- Xcode with Swift 6.1.2 toolchain
- .NET SDK installed

## Step 1: Rebuild the Swift Proxy Framework

Since we modified Swift code (`PaymentSheetProxy.swift` and `FlowControllerProxy.swift`), you need to rebuild the xcframework:

```bash
cd src
make build-ios build-simulator create-xcframework
```

This will:
- Build the iOS framework
- Build the simulator framework  
- Create the xcframework in `src/xcframework/Stripe.Swift.Proxy.xcframework`

**Important:** After building, copy the xcframework to the bindings project:

```bash
# From src directory
cp -R xcframework/Stripe.Swift.Proxy.xcframework G1.Stripe.iOS.Bindings/
```

**Note:** The `generate-api-definitions` step uses Sharpie and is not necessary since `ProxyApiDefinition.g.cs` was manually updated with the new `TSPSIntentConfiguration` bindings.

## Step 2: Build the NuGet Packages

From the repository root:

```powershell
# Windows
.\build-packages.ps1

# Or manually
dotnet restore
dotnet build --configuration Release
dotnet pack src\G1.Stripe.Android.Bindings\G1.Stripe.Android.Bindings.csproj --output ./nupkgs --no-build --configuration Release
dotnet pack src\G1.Stripe.iOS.Bindings\G1.Stripe.iOS.Bindings.csproj --output ./nupkgs --no-build --configuration Release
dotnet pack src\G1.Stripe.Maui\G1.Stripe.Maui.csproj --output ./nupkgs --no-build --configuration Release
```

Packages will be created in the `./nupkgs` directory.

## Step 3: Use Local Packages in Your Project

### Option A: Add Local NuGet Source

Add the local `nupkgs` directory as a NuGet source:

```bash
# Add local source
dotnet nuget add source ./nupkgs --name local-g1-stripe

# Or in your project's NuGet.config
```

Then in your project's `.csproj` or via CLI:

```bash
dotnet add package G1.Stripe.Maui --source local-g1-stripe
dotnet add package G1.Stripe.iOS.Bindings --source local-g1-stripe
dotnet add package G1.Stripe.Android.Bindings --source local-g1-stripe
```

### Option B: Direct Package Reference

Reference the `.nupkg` files directly:

```xml
<ItemGroup>
  <PackageReference Include="G1.Stripe.Maui" Version="1.0.0-local" />
  <PackageReference Include="G1.Stripe.iOS.Bindings" Version="1.0.0-local" />
  <PackageReference Include="G1.Stripe.Android.Bindings" Version="1.0.0-local" />
</ItemGroup>
```

### Option C: Project References (for development)

If you're working in the same solution, use project references:

```xml
<ItemGroup>
  <ProjectReference Include="..\..\G1.Stripe.Maui\src\G1.Stripe.Maui\G1.Stripe.Maui.csproj" />
  <ProjectReference Include="..\..\G1.Stripe.Maui\src\G1.Stripe.iOS.Bindings\G1.Stripe.iOS.Bindings.csproj" />
</ItemGroup>
```

## Step 4: Verify the Changes

You can now use `IntentConfiguration` in your code:

```csharp
var options = new PaymentSheetOptions
{
    ClientSecret = paymentIntentClientSecret,
    MerchantDisplayName = "My Store, Inc.",
    IntentConfiguration = new PaymentSheetIntentConfigurationOptions
    {
        StripeAccount = "acct_1234567890" // Connect account ID
    }
};

var result = await paymentSheet.Open(options, cancellationToken);
```

## Troubleshooting

### If you get build errors about missing xcframework:
- Make sure you ran `make all` in the `src` directory
- Verify `src/G1.Stripe.iOS.Bindings/Stripe.Swift.Proxy.xcframework` exists

### If C# bindings are missing:
- The `ProxyApiDefinition.g.cs` file was manually updated with `TSPSIntentConfiguration`
- If you regenerate with Sharpie, you may need to merge the changes back

### If packages aren't found:
- Check that packages are in `./nupkgs` directory
- Verify the NuGet source is configured correctly
- Try clearing NuGet cache: `dotnet nuget locals all --clear`

