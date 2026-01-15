using FluentAssertions;
using WayForPaySDK.Crypto;
using WayForPaySDK.Domain;
using WayForPaySDK.Forms;
using WayForPaySDK.Tests.Fixtures;

namespace WayForPaySDK.Tests.Unit;

public class PaymentFormBuilderSecurityTests
{
    private readonly PaymentFormBuilder _sut;

    public PaymentFormBuilderSecurityTests()
    {
        var options = TestOptions.CreateOptions();
        var signatureGenerator = new SignatureGenerator(options);
        _sut = new PaymentFormBuilder(options, signatureGenerator);
    }

    [Theory]
    [InlineData("<script>alert('xss')</script>", "&lt;script&gt;")]
    [InlineData("\"onmouseover=\"alert('xss')", "&quot;onmouseover=&quot;")]
    [InlineData("'><script>alert('xss')</script>", "&#39;&gt;&lt;script&gt;")]
    [InlineData("<img src=x onerror=alert('xss')>", "&lt;img")]
    public void GenerateHtml_WithMaliciousFieldValue_EncodesValue(string maliciousValue, string expectedEncoded)
    {
        var formData = new PurchaseFormData
        {
            AutoSubmit = false,
            Fields = new Dictionary<string, string>
            {
                ["productName"] = maliciousValue
            }
        };

        var html = _sut.GenerateHtml(formData);

        html.Should().NotContain($"value=\"{maliciousValue}\"");
        html.Should().Contain(expectedEncoded);
    }

    [Theory]
    [InlineData("<script>", "&lt;script&gt;")]
    [InlineData("\"onclick=\"", "&quot;onclick=&quot;")]
    [InlineData("'onload='", "&#39;onload=&#39;")]
    public void GenerateHtml_WithMaliciousFormId_EncodesFormId(string maliciousFormId, string expectedEncoded)
    {
        var formData = new PurchaseFormData
        {
            FormId = maliciousFormId,
            AutoSubmit = false,
            Fields = new Dictionary<string, string>()
        };

        var html = _sut.GenerateHtml(formData);

        html.Should().NotContain($"id=\"{maliciousFormId}\"");
        html.Should().Contain(expectedEncoded);
    }

    [Theory]
    [InlineData("\" onclick=\"alert('xss')", "&quot; onclick=&quot;")]
    [InlineData("https://evil.com\" onload=\"alert('xss')", "&quot; onload=&quot;")]
    public void GenerateHtml_WithMaliciousActionUrl_EncodesActionUrl(string maliciousUrl, string expectedEncoded)
    {
        var formData = new PurchaseFormData
        {
            ActionUrl = maliciousUrl,
            AutoSubmit = false,
            Fields = new Dictionary<string, string>()
        };

        var html = _sut.GenerateHtml(formData);

        html.Should().NotContain($"action=\"{maliciousUrl}\"");
        html.Should().Contain(expectedEncoded);
    }

    [Theory]
    [InlineData("\" onclick=\"alert('xss')\" data-x=\"")]
    [InlineData("<script>")]
    public void GenerateHtml_WithMaliciousFieldKey_EncodesKey(string maliciousKey)
    {
        var formData = new PurchaseFormData
        {
            Fields = new Dictionary<string, string>
            {
                [maliciousKey] = "safe_value"
            }
        };

        var html = _sut.GenerateHtml(formData);

        html.Should().NotContain($"name=\"{maliciousKey}\"");
    }

    [Fact]
    public void GenerateHtml_WithAutoSubmitAndMaliciousFormId_EscapesInJavaScript()
    {
        var formData = new PurchaseFormData
        {
            FormId = "\");alert('xss');//",
            AutoSubmit = true,
            Fields = new Dictionary<string, string>()
        };

        var html = _sut.GenerateHtml(formData);

        html.Should().NotContain("alert('xss')");
        html.Should().Contain("\\u0022");
    }

    [Fact]
    public void GenerateHtml_WithNormalData_ProducesValidHtml()
    {
        var formData = new PurchaseFormData
        {
            FormId = "wayforpay_form",
            ActionUrl = "https://secure.wayforpay.com/pay",
            Method = "POST",
            AutoSubmit = true,
            Fields = new Dictionary<string, string>
            {
                ["merchantAccount"] = "test_merchant",
                ["amount"] = "100.00",
                ["currency"] = "UAH"
            }
        };

        var html = _sut.GenerateHtml(formData);

        html.Should().Contain("<form id=\"wayforpay_form\"");
        html.Should().Contain("action=\"https://secure.wayforpay.com/pay\"");
        html.Should().Contain("method=\"POST\"");
        html.Should().Contain("name=\"merchantAccount\" value=\"test_merchant\"");
        html.Should().Contain("document.getElementById(\"wayforpay_form\").submit();");
    }

    [Fact]
    public void CreatePurchaseForm_WithValidData_GeneratesSignature()
    {
        var products = new List<Product>
        {
            new()
            {
                Name = "Test Product",
                Price = 100.00m,
                Count = 1
            }
        };

        var formData = _sut.CreatePurchaseForm(
            orderReference: "ORDER-001",
            amount: 100.00m,
            currency: "UAH",
            products: products);

        formData.Fields.Should().ContainKey("merchantSignature");
        formData.Fields["merchantSignature"].Should().HaveLength(32);
    }

    [Fact]
    public void GenerateHtml_WithSpecialCharactersInProductName_EncodesCorrectly()
    {
        var formData = new PurchaseFormData
        {
            Fields = new Dictionary<string, string>
            {
                ["productName[]"] = "Product <test> \"special\" & 'chars'"
            }
        };

        var html = _sut.GenerateHtml(formData);

        html.Should().Contain("&lt;test&gt;");
        html.Should().Contain("&quot;special&quot;");
        html.Should().Contain("&amp;");
        html.Should().Contain("&#39;chars&#39;");
    }
}
