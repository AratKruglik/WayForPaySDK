using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using WayForPaySDK.Crypto;
using WayForPaySDK.Domain;
using WayForPaySDK.Options;

namespace WayForPaySDK.Forms;

public sealed class PaymentFormBuilder
{
    private readonly WayForPayOptions _options;
    private readonly ISignatureGenerator _signatureGenerator;

    public PaymentFormBuilder(
        IOptions<WayForPayOptions> options,
        ISignatureGenerator signatureGenerator)
    {
        _options = options.Value;
        _signatureGenerator = signatureGenerator;
    }

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

        fields["productName[]"] = string.Join(";", productList.Select(p => p.Name));
        fields["productPrice[]"] = string.Join(";", productList.Select(p =>
            p.Price.ToString("0.##", CultureInfo.InvariantCulture)));
        fields["productCount[]"] = string.Join(";", productList.Select(p => p.Count));

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

        if (!string.IsNullOrEmpty(returnUrl))
            fields["returnUrl"] = returnUrl;
        if (!string.IsNullOrEmpty(serviceUrl))
            fields["serviceUrl"] = serviceUrl;
        if (!string.IsNullOrEmpty(language))
            fields["language"] = language;

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

    public string GenerateHtml(PurchaseFormData formData)
    {
        var sb = new StringBuilder();

        // HTML-encode all attributes to prevent XSS
        var encodedFormId = System.Net.WebUtility.HtmlEncode(formData.FormId);
        var encodedActionUrl = System.Net.WebUtility.HtmlEncode(formData.ActionUrl);
        var encodedMethod = System.Net.WebUtility.HtmlEncode(formData.Method);

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset=\"utf-8\">");
        sb.AppendLine("    <title>Redirecting to payment...</title>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine($"    <form id=\"{encodedFormId}\" action=\"{encodedActionUrl}\" method=\"{encodedMethod}\">");

        foreach (var field in formData.Fields)
        {
            // Encode both key and value to prevent XSS via field names
            var encodedKey = System.Net.WebUtility.HtmlEncode(field.Key);
            var encodedValue = System.Net.WebUtility.HtmlEncode(field.Value);
            sb.AppendLine($"        <input type=\"hidden\" name=\"{encodedKey}\" value=\"{encodedValue}\">");
        }

        sb.AppendLine("        <noscript>");
        sb.AppendLine("            <p>Please click the button below to proceed to payment:</p>");
        sb.AppendLine("            <button type=\"submit\">Proceed to Payment</button>");
        sb.AppendLine("        </noscript>");
        sb.AppendLine("    </form>");

        if (formData.AutoSubmit)
        {
            // Use JSON serialization to safely escape FormId for JavaScript context
            var jsFormId = JsonSerializer.Serialize(formData.FormId);
            sb.AppendLine("    <script>");
            sb.AppendLine($"        document.getElementById({jsFormId}).submit();");
            sb.AppendLine("    </script>");
        }

        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }

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
