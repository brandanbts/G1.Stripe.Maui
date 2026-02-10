import Foundation
import UIKit
import StripePaymentSheet
import StripeCore

// MARK: - CustomerSheet Result
@objc public enum TSPSCustomerSheetResult: Int {
    case canceled = 0
    case selected
    case error
}

// MARK: - CustomerSheet Wrapper
/// Same pattern as PaymentSheet: pass configuration and secrets directly. No bridge protocol.
@objc(TSPSCustomerSheet)
public class TSPSCustomerSheet: NSObject {
    private var customerSheet: CustomerSheet?

    @objc public init(
        configuration: TSPSCustomerSheetConfiguration,
        customerId: String,
        customerSessionClientSecret: String,
        setupIntentClientSecret: String?
    ) {
        super.init()

        let stripeConfig = configuration.toStripeConfiguration()
        let intentConfig = CustomerSheet.IntentConfiguration(
            paymentMethodTypes: nil,
            onBehalfOf: configuration.onBehalfOf,
            setupIntentClientSecretProvider: {
                guard let secret = setupIntentClientSecret else {
                    throw NSError(domain: "G1.Stripe.Maui", code: -1, userInfo: [NSLocalizedDescriptionKey: "SetupIntentClientSecret is required to add payment methods. Set it on CustomerSheetOptions."])
                }
                return secret
            }
        )
        let customerSessionProvider: () async throws -> CustomerSessionClientSecret = {
            CustomerSessionClientSecret(customerId: customerId, clientSecret: customerSessionClientSecret)
        }

        self.customerSheet = CustomerSheet(
            configuration: stripeConfig,
            intentConfiguration: intentConfig,
            customerSessionClientSecretProvider: customerSessionProvider
        )
    }

    @objc public func present(from presentingViewController: UIViewController, completion: @escaping (TSPSCustomerSheetResult, TSPSCustomerSheetPaymentOption?, NSError?) -> Void) {
        guard let customerSheet = self.customerSheet else {
            completion(.error, nil, NSError(domain: "TSPSCustomerSheet", code: -1, userInfo: [NSLocalizedDescriptionKey: "CustomerSheet not initialized"]))
            return
        }

        customerSheet.present(from: presentingViewController) { result in
            switch result {
            case .canceled(let selection):
                let option = selection.flatMap { TSPSCustomerSheetPaymentOption.from($0) }
                completion(.canceled, option, nil)
            case .selected(let selection):
                let option = selection.flatMap { TSPSCustomerSheetPaymentOption.from($0) }
                completion(.selected, option, nil)
            case .error(let error):
                completion(.error, nil, error as NSError)
            }
        }
    }
}

// MARK: - Payment Option
@objc(TSPSCustomerSheetPaymentOption)
public class TSPSCustomerSheetPaymentOption: NSObject {
    @objc public let paymentMethodId: String?
    @objc public let label: String
    @objc public let isWallet: Bool

    @objc public init(paymentMethodId: String?, label: String, isWallet: Bool) {
        self.paymentMethodId = paymentMethodId
        self.label = label
        self.isWallet = isWallet
        super.init()
    }

    static func from(_ selection: CustomerSheet.PaymentOptionSelection) -> TSPSCustomerSheetPaymentOption {
        let displayData = selection.displayData()
        switch selection {
        case .paymentMethod(let paymentMethod, _):
            return TSPSCustomerSheetPaymentOption(
                paymentMethodId: paymentMethod.stripeId,
                label: displayData.label,
                isWallet: false
            )
        case .applePay:
            return TSPSCustomerSheetPaymentOption(
                paymentMethodId: nil,
                label: displayData.label,
                isWallet: true
            )
        }
    }
}

// MARK: - Configuration
@objc(TSPSCustomerSheetConfiguration)
public class TSPSCustomerSheetConfiguration: NSObject {
    @objc public var merchantDisplayName: String = ""
    @objc public var returnURL: String?
    @objc public var headerTextForSelectionScreen: String?
    @objc public var onBehalfOf: String?

    @objc public override init() {
        super.init()
    }

    internal func toStripeConfiguration() -> CustomerSheet.Configuration {
        var config = CustomerSheet.Configuration()
        config.merchantDisplayName = merchantDisplayName
        config.returnURL = returnURL
        config.headerTextForSelectionScreen = headerTextForSelectionScreen
        return config
    }
}
