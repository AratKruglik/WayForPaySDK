using System.Text.Json.Serialization;

namespace WayForPaySDK.Requests;

public sealed class UpdatePartnerRequest : MmsRequest
{
    public override string MmsOperation => "updatePartner";

    [JsonPropertyName("partnerCode")]
    public required string PartnerCode { get; set; }

    [JsonPropertyName("site")]
    public string? Site { get; set; }

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("compensationCardNumber")]
    public string? CompensationCardNumber { get; set; }

    [JsonPropertyName("compensationCardExpYear")]
    public string? CompensationCardExpYear { get; set; }

    [JsonPropertyName("compensationCardExpMonth")]
    public string? CompensationCardExpMonth { get; set; }

    [JsonPropertyName("compensationCardCvv")]
    public string? CompensationCardCvv { get; set; }

    [JsonPropertyName("compensationCardHolder")]
    public string? CompensationCardHolder { get; set; }

    [JsonPropertyName("compensationCardToken")]
    public string? CompensationCardToken { get; set; }

    [JsonPropertyName("compensationAccount")]
    public string? CompensationAccount { get; set; }

    [JsonPropertyName("compensationAccountIban")]
    public string? CompensationAccountIban { get; set; }

    [JsonPropertyName("compensationAccountMfo")]
    public string? CompensationAccountMfo { get; set; }

    [JsonPropertyName("compensationAccountOkpo")]
    public string? CompensationAccountOkpo { get; set; }

    [JsonPropertyName("compensationAccountName")]
    public string? CompensationAccountName { get; set; }

    public override IEnumerable<string> GetSignatureFields()
    {
        return new[] { MerchantAccount, PartnerCode };
    }
}
