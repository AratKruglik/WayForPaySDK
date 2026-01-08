namespace WayForPaySDK.Forms;

/// <summary>
/// Data for generating an HTML payment form for WayForPay PURCHASE operation.
/// This form can be auto-submitted in the browser to initiate payment.
/// </summary>
public sealed class PurchaseFormData
{
    /// <summary>
    /// Gets or sets the WayForPay API endpoint URL.
    /// Default: https://secure.wayforpay.com/pay
    /// </summary>
    public string ActionUrl { get; set; } = "https://secure.wayforpay.com/pay";

    /// <summary>
    /// Gets or sets the form fields (key-value pairs).
    /// </summary>
    public required Dictionary<string, string> Fields { get; set; }

    /// <summary>
    /// Gets or sets the HTTP method for form submission.
    /// Default: POST
    /// </summary>
    public string Method { get; set; } = "POST";

    /// <summary>
    /// Gets or sets the form ID attribute.
    /// Default: wayforpay-form
    /// </summary>
    public string FormId { get; set; } = "wayforpay-form";

    /// <summary>
    /// Gets or sets whether to auto-submit the form on page load.
    /// Default: true
    /// </summary>
    public bool AutoSubmit { get; set; } = true;
}
