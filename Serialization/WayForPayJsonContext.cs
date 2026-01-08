using System.Text.Json.Serialization;
using WayForPaySDK.Domain;
using WayForPaySDK.Domain.Enums;

namespace WayForPaySDK.Serialization;

/// <summary>
/// JSON serialization context for WayForPay SDK types.
/// Uses source generation for AOT compatibility and improved performance.
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false)]
[JsonSerializable(typeof(Card))]
[JsonSerializable(typeof(CardToken))]
[JsonSerializable(typeof(Client))]
[JsonSerializable(typeof(Product))]
[JsonSerializable(typeof(Product[]))]
[JsonSerializable(typeof(Reason))]
[JsonSerializable(typeof(Regular))]
[JsonSerializable(typeof(Transaction))]
[JsonSerializable(typeof(TransactionStatus))]
[JsonSerializable(typeof(PaymentSystem))]
[JsonSerializable(typeof(Currency))]
[JsonSerializable(typeof(Language))]
[JsonSerializable(typeof(MerchantTransactionType))]
[JsonSerializable(typeof(RegularBehavior))]
[JsonSerializable(typeof(RegularMode))]
[JsonSerializable(typeof(RegularMode[]))]
[JsonSerializable(typeof(Dictionary<string, object>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
public partial class WayForPayJsonContext : JsonSerializerContext
{
}
