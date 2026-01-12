using FluentAssertions;
using WayForPaySDK.Requests;

namespace WayForPaySDK.Tests.Unit;

public class RegularManagementTests
{
    [Fact]
    public void SuspendRegularRequest_HasCorrectRequestType()
    {
        var request = new SuspendRegularRequest
        {
            MerchantAccount = "merch",
            MerchantPassword = "pass",
            OrderReference = "ORDER1"
        };
        request.RequestType.Should().Be("SUSPEND");
    }

    [Fact]
    public void ResumeRegularRequest_HasCorrectRequestType()
    {
        var request = new ResumeRegularRequest
        {
            MerchantAccount = "merch",
            MerchantPassword = "pass",
            OrderReference = "ORDER1"
        };
        request.RequestType.Should().Be("RESUME");
    }

    [Fact]
    public void RemoveRegularRequest_HasCorrectRequestType()
    {
        var request = new RemoveRegularRequest
        {
            MerchantAccount = "merch",
            MerchantPassword = "pass",
            OrderReference = "ORDER1"
        };
        request.RequestType.Should().Be("REMOVE");
    }
}
