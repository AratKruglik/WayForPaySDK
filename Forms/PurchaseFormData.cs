namespace WayForPaySDK.Forms;

public sealed class PurchaseFormData
{
    public string ActionUrl { get; set; } = "https://secure.wayforpay.com/pay";
    public required Dictionary<string, string> Fields { get; set; }
    public string Method { get; set; } = "POST";
    public string FormId { get; set; } = "wayforpay-form";
    public bool AutoSubmit { get; set; } = true;
}
