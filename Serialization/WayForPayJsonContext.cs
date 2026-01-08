using System.Text.Json.Serialization;
using WayForPaySDK.Domain;
using WayForPaySDK.Domain.Enums;
using WayForPaySDK.Requests;
using WayForPaySDK.Responses;

namespace WayForPaySDK.Serialization;

/// <summary>
/// JSON serialization context for WayForPay SDK types.
/// Uses source generation for AOT compatibility and improved performance.
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false)]
// Domain models
[JsonSerializable(typeof(Card))]
[JsonSerializable(typeof(CardToken))]
[JsonSerializable(typeof(Client))]
[JsonSerializable(typeof(Product))]
[JsonSerializable(typeof(Product[]))]
[JsonSerializable(typeof(Reason))]
[JsonSerializable(typeof(Regular))]
[JsonSerializable(typeof(Transaction))]
// Enums
[JsonSerializable(typeof(TransactionStatus))]
[JsonSerializable(typeof(PaymentSystem))]
[JsonSerializable(typeof(Currency))]
[JsonSerializable(typeof(Language))]
[JsonSerializable(typeof(MerchantTransactionType))]
[JsonSerializable(typeof(RegularBehavior))]
[JsonSerializable(typeof(RegularMode))]
[JsonSerializable(typeof(RegularMode[]))]
// Requests
[JsonSerializable(typeof(ChargeRequest))]
[JsonSerializable(typeof(RefundRequest))]
[JsonSerializable(typeof(CheckStatusRequest))]
[JsonSerializable(typeof(SettleRequest))]
[JsonSerializable(typeof(VoidRequest))]
[JsonSerializable(typeof(PurchaseRequest))]
[JsonSerializable(typeof(InvoiceRequest))]
[JsonSerializable(typeof(Complete3DSRequest))]
[JsonSerializable(typeof(VerifyRequest))]
[JsonSerializable(typeof(TransactionListRequest))]
// Responses
[JsonSerializable(typeof(ChargeResponse))]
[JsonSerializable(typeof(RefundResponse))]
[JsonSerializable(typeof(CheckStatusResponse))]
[JsonSerializable(typeof(SettleResponse))]
[JsonSerializable(typeof(VoidResponse))]
[JsonSerializable(typeof(PurchaseResponse))]
[JsonSerializable(typeof(InvoiceResponse))]
[JsonSerializable(typeof(Complete3DSResponse))]
[JsonSerializable(typeof(VerifyResponse))]
[JsonSerializable(typeof(TransactionListResponse))]
// Collections
[JsonSerializable(typeof(Dictionary<string, object>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(decimal[]))]
[JsonSerializable(typeof(int[]))]
public partial class WayForPayJsonContext : JsonSerializerContext
{
}
