namespace WayForPaySDK.Requests;

public sealed class MerchantInfoRequest : MmsRequest
{
    public override string MmsOperation => "merchantInfo";

    public override IEnumerable<string> GetSignatureFields()
    {
        return new[] { MerchantAccount };
    }
}
