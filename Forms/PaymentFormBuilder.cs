using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using WayForPaySDK.Crypto;
using WayForPaySDK.Domain;
using WayForPaySDK.Options;

namespace WayForPaySDK.Forms;

/// <summary>
/// Builder for creating HTML payment forms for WayForPay redirect flow.
/// </summary>
public sealed class PaymentFormBuilder
{
    private readonly WayForPayOptions _options;
    private readonly ISignatureGenerator _signatureGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentFormBuilder"/> class.
    /// </summary>
    public PaymentFormBuilder(
        IOptions<WayForPayOptions> options,
        ISignatureGenerator signatureGenerator)
    {
        _options = options.Value;
        _signatureGenerator = signatureGenerator;
    }

    /// <summary>
    /// Creates form data for a purchase payment.
    /// </summary>
    /// <param name="orderReference">Unique order identifier.</param>
    /// <param name="amount">Payment amount.</param>
    /// <param name="currency">Currency code (e.g., "UAH").</param>
    /// <param name="products">Products in the order.</param>
    /// <param name="client">Client information (optional).</param>
    /// <param name="returnUrl">URL to redirect after payment (optional).</param>
    /// <param name="serviceUrl">Callback URL for server notifications (optional).</param>
    /// <param name="language">Payment page language (optional).</param>
    /// <param name="regular">Recurring payment settings (optional).</param>
    /// <returns>Purchase form data ready for HTML generation.</returns>
    public PurchaseFormData CreatePurchaseForm(
        string orderReference,
        decimal amount,
        string currency,
        IEnumerable<Product> products,
        Client? client = null,
        string? returnUrl = null,
        string? serviceUrl = null,
        string? language = null,
        Regular? regular = null)
    {
        var productList = products.ToList();
        var orderDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var fields = new Dictionary<string, string>
        {
            ["merchantAccount"] = _options.MerchantAccount,
            ["merchantDomainName"] = _options.MerchantDomainName,
            ["orderReference"] = orderReference,
            ["orderDate"] = orderDate.ToString(),
            ["amount"] = amount.ToString("0.##", CultureInfo.InvariantCulture),
            ["currency"] = currency.ToUpperInvariant()
        };

        // Add product arrays
        fields["productName[]"] = string.Join(";", productList.Select(p => p.Name));
        fields["productPrice[]"] = string.Join(";", productList.Select(p =>
            p.Price.ToString("0.##", CultureInfo.InvariantCulture)));
        fields["productCount[]"] = string.Join(";", productList.Select(p => p.Count));

        // Add client information if provided
        if (client != null)
        {
            if (!string.IsNullOrEmpty(client.FirstName))
                fields["clientFirstName"] = client.FirstName;
            if (!string.IsNullOrEmpty(client.LastName))
                fields["clientLastName"] = client.LastName;
            if (!string.IsNullOrEmpty(client.Email))
                fields["clientEmail"] = client.Email;
            if (!string.IsNullOrEmpty(client.Phone))
                fields["clientPhone"] = client.Phone;
            if (!string.IsNullOrEmpty(client.Country))
                fields["clientCountry"] = client.Country;
        }

        // Add optional URLs
        if (!string.IsNullOrEmpty(returnUrl))
            fields["returnUrl"] = returnUrl;
        if (!string.IsNullOrEmpty(serviceUrl))
            fields["serviceUrl"] = serviceUrl;
        if (!string.IsNullOrEmpty(language))
            fields["language"] = language;

        // Add regular payment fields if provided
        if (regular != null)
        {
            if (regular.Amount.HasValue)
                fields["regularAmount"] = regular.Amount.Value.ToString("0.##", CultureInfo.InvariantCulture);

            if (regular.Modes.Count > 0)
                fields["regularMode[]"] = string.Join(";", regular.Modes.Select(m => m.ToString().ToLowerInvariant()));

            if (regular.DateNext.HasValue)
                fields["regularOn"] = regular.DateNext.Value.ToString("yyyy-MM-dd");

            if (regular.Count.HasValue)
                fields["regularCount"] = regular.Count.Value.ToString();

            if (regular.Behavior.HasValue)
                fields["regularBehavior"] = regular.Behavior.Value.ToString().ToLowerInvariant();
        }

        // Generate signature
        var signatureFields = new List<string>
        {
            _options.MerchantAccount,
            _options.MerchantDomainName,
            orderReference,
            orderDate.ToString(),
            amount.ToString("0.##", CultureInfo.InvariantCulture),
            currency.ToUpperInvariant()
        };
        signatureFields.AddRange(productList.Select(p => p.Name));
        signatureFields.AddRange(productList.Select(p => p.Count.ToString()));
        signatureFields.AddRange(productList.Select(p =>
            p.Price.ToString("0.##", CultureInfo.InvariantCulture)));

        fields["merchantSignature"] = _signatureGenerator.GenerateSignature(signatureFields);

        return new PurchaseFormData
        {
            Fields = fields
        };
    }

    /// <summary>
    /// Generates HTML form markup from form data.
    /// </summary>
    /// <param name="formData">The form data.</param>
    /// <returns>HTML string containing the form.</returns>
    public string GenerateHtml(PurchaseFormData formData)
    {
        var sb = new StringBuilder();

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset=\"utf-8\">");
        sb.AppendLine("    <title>Redirecting to payment...</title>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine($"    <form id=\"{formData.FormId}\" action=\"{formData.ActionUrl}\" method=\"{formData.Method}\">");

        foreach (var field in formData.Fields)
        {
            var encodedValue = System.Net.WebUtility.HtmlEncode(field.Value);
            sb.AppendLine($"        <input type=\"hidden\" name=\"{field.Key}\" value=\"{encodedValue}\">");
        }

        sb.AppendLine("        <noscript>");
        sb.AppendLine("            <p>Please click the button below to proceed to payment:</p>");
        sb.AppendLine("            <button type=\"submit\">Proceed to Payment</button>");
        sb.AppendLine("        </noscript>");
        sb.AppendLine("    </form>");

        if (formData.AutoSubmit)
        {
            sb.AppendLine("    <script>");
            sb.AppendLine($"        document.getElementById('{formData.FormId}').submit();");
            sb.AppendLine("    </script>");
        }

        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }

    /// <summary>
    /// Creates and generates HTML for a purchase form in one call.
    /// </summary>
    /// <param name="orderReference">Unique order identifier.</param>
    /// <param name="amount">Payment amount.</param>
    /// <param name="currency">Currency code (e.g., "UAH").</param>
    /// <param name="products">Products in the order.</param>
    /// <param name="client">Client information (optional).</param>
    /// <param name="returnUrl">URL to redirect after payment (optional).</param>
    /// <param name="serviceUrl">Callback URL for server notifications (optional).</param>
    /// <param name="language">Payment page language (optional).</param>
    /// <param name="regular">Recurring payment settings (optional).</param>
    /// <returns>Complete HTML string for the payment form.</returns>
    public string CreatePurchaseFormHtml(
        string orderReference,
        decimal amount,
        string currency,
        IEnumerable<Product> products,
        Client? client = null,
        string? returnUrl = null,
        string? serviceUrl = null,
        string? language = null,
        Regular? regular = null)
    {
        var formData = CreatePurchaseForm(
            orderReference,
            amount,
            currency,
            products,
            client,
            returnUrl,
            serviceUrl,
            language,
            regular);

        return GenerateHtml(formData);
    }
}
