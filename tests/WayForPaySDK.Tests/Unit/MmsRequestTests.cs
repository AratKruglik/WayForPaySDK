using FluentAssertions;
using WayForPaySDK.Requests;

namespace WayForPaySDK.Tests.Unit;

public class MmsRequestTests
{
    [Fact]
    public void AddPartnerRequest_MmsOperation_ReturnsAddPartner()
    {
        var request = new AddPartnerRequest
        {
            MerchantAccount = "test",
            MerchantSignature = "",
            PartnerCode = "p1",
            Site = "https://site.com",
            Phone = "+380501234567",
            Email = "test@test.com"
        };

        request.MmsOperation.Should().Be("addPartner");
    }

    [Fact]
    public void AddPartnerRequest_GetSignatureFields_ReturnsCorrectFields()
    {
        var request = new AddPartnerRequest
        {
            MerchantAccount = "test_merchant",
            MerchantSignature = "",
            PartnerCode = "seller-001",
            Site = "https://site.com",
            Phone = "+380501234567",
            Email = "test@test.com"
        };

        var fields = request.GetSignatureFields().ToList();

        fields.Should().HaveCount(4);
        fields[0].Should().Be("test_merchant");
        fields[1].Should().Be("seller-001");
        fields[2].Should().Be("+380501234567");
        fields[3].Should().Be("test@test.com");
    }

    [Fact]
    public void PartnerInfoRequest_MmsOperation_ReturnsPartnerInfo()
    {
        var request = new PartnerInfoRequest
        {
            MerchantAccount = "test",
            MerchantSignature = "",
            PartnerCode = "p1"
        };

        request.MmsOperation.Should().Be("partnerInfo");
    }

    [Fact]
    public void PartnerInfoRequest_GetSignatureFields_ReturnsCorrectFields()
    {
        var request = new PartnerInfoRequest
        {
            MerchantAccount = "test_merchant",
            MerchantSignature = "",
            PartnerCode = "seller-001"
        };

        var fields = request.GetSignatureFields().ToList();

        fields.Should().HaveCount(2);
        fields[0].Should().Be("test_merchant");
        fields[1].Should().Be("seller-001");
    }

    [Fact]
    public void UpdatePartnerRequest_MmsOperation_ReturnsUpdatePartner()
    {
        var request = new UpdatePartnerRequest
        {
            MerchantAccount = "test",
            MerchantSignature = "",
            PartnerCode = "p1"
        };

        request.MmsOperation.Should().Be("updatePartner");
    }

    [Fact]
    public void UpdatePartnerRequest_GetSignatureFields_ReturnsCorrectFields()
    {
        var request = new UpdatePartnerRequest
        {
            MerchantAccount = "test_merchant",
            MerchantSignature = "",
            PartnerCode = "seller-001"
        };

        var fields = request.GetSignatureFields().ToList();

        fields.Should().HaveCount(2);
        fields[0].Should().Be("test_merchant");
        fields[1].Should().Be("seller-001");
    }

    [Fact]
    public void AddMerchantRequest_MmsOperation_ReturnsAddMerchant()
    {
        var request = new AddMerchantRequest
        {
            MerchantAccount = "test",
            MerchantSignature = "",
            Site = "https://shop.com",
            Phone = "+380501234567",
            Email = "shop@test.com"
        };

        request.MmsOperation.Should().Be("addMerchant");
    }

    [Fact]
    public void AddMerchantRequest_GetSignatureFields_ReturnsCorrectFields()
    {
        var request = new AddMerchantRequest
        {
            MerchantAccount = "test_merchant",
            MerchantSignature = "",
            Site = "https://shop.com",
            Phone = "+380501234567",
            Email = "shop@test.com"
        };

        var fields = request.GetSignatureFields().ToList();

        fields.Should().HaveCount(4);
        fields[0].Should().Be("test_merchant");
        fields[1].Should().Be("https://shop.com");
        fields[2].Should().Be("+380501234567");
        fields[3].Should().Be("shop@test.com");
    }

    [Fact]
    public void MerchantInfoRequest_MmsOperation_ReturnsMerchantInfo()
    {
        var request = new MerchantInfoRequest
        {
            MerchantAccount = "test",
            MerchantSignature = ""
        };

        request.MmsOperation.Should().Be("merchantInfo");
    }

    [Fact]
    public void MerchantInfoRequest_GetSignatureFields_ReturnsCorrectFields()
    {
        var request = new MerchantInfoRequest
        {
            MerchantAccount = "test_merchant",
            MerchantSignature = ""
        };

        var fields = request.GetSignatureFields().ToList();

        fields.Should().HaveCount(1);
        fields[0].Should().Be("test_merchant");
    }

    [Fact]
    public void MerchantBalanceRequest_MmsOperation_ReturnsMerchantBalance()
    {
        var request = new MerchantBalanceRequest
        {
            MerchantAccount = "test",
            MerchantSignature = ""
        };

        request.MmsOperation.Should().Be("merchantBalance");
    }

    [Fact]
    public void MerchantBalanceRequest_GetSignatureFields_ReturnsCorrectFields()
    {
        var request = new MerchantBalanceRequest
        {
            MerchantAccount = "test_merchant",
            MerchantSignature = ""
        };

        var fields = request.GetSignatureFields().ToList();

        fields.Should().HaveCount(1);
        fields[0].Should().Be("test_merchant");
    }
}
