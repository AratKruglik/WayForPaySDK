# WayForPaySDK

[![NuGet](https://img.shields.io/nuget/v/WayForPaySDK.svg)](https://www.nuget.org/packages/WayForPaySDK/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0%20%7C%2010.0-512BD4)](https://dotnet.microsoft.com/)

A modern .NET SDK for [WayForPay](https://wayforpay.com) payment gateway integration. This library provides a type-safe, async-first API for all WayForPay operations.

## Features

**Complete API Coverage**
- Purchase operations (standard and with regular payments)
- Charge operations (with token and regular payments)
- Refund and Settlement operations
- Transaction status checks and verification
- Invoice creation
- 3D-Secure support

**Developer Experience**
- Async/await pattern throughout
- Strong typing with nullable reference types
- Comprehensive XML documentation
- Dependency injection support
- Multi-framework targeting (.NET 8.0, 9.0, 10.0)
- ASP.NET Core webhook middleware

**Production Ready**
- Secure MD5 signature generation
- Automatic request/response validation
- Structured error handling
- Comprehensive test coverage

## Installation

```bash
dotnet add package WayForPaySDK
```

Or via Package Manager Console:
```powershell
Install-Package WayForPaySDK
```

## Quick Start

### 1. Configuration

Add WayForPay settings to your `appsettings.json`:

```json
{
  "WayForPay": {
    "MerchantAccount": "your_merchant_account",
    "MerchantSecretKey": "your_secret_key",
    "MerchantDomainName": "example.com"
  }
}
```

### 2. Service Registration

Register WayForPaySDK in your DI container:

```csharp
// Program.cs or Startup.cs
builder.Services.AddWayForPay(configuration.GetSection("WayForPay"));
```

### 3. Basic Usage

```csharp
public class PaymentService
{
    private readonly IWayForPayClient _client;

    public PaymentService(IWayForPayClient client)
    {
        _client = client;
    }

    public async Task<PurchaseResponse> CreatePaymentAsync()
    {
        var request = new PurchaseRequest
        {
            OrderReference = "ORDER-12345",
            Amount = 100.50m,
            Currency = Currency.UAH,
            OrderDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ProductName = ["Test Product"],
            ProductCount = [1],
            ProductPrice = [100.50m]
        };

        return await _client.CreatePurchaseAsync(request);
    }
}
```

## Usage Examples

### Creating a Payment

```csharp
var purchase = await _client.CreatePurchaseAsync(new PurchaseRequest
{
    OrderReference = $"ORDER-{DateTime.UtcNow:yyyyMMddHHmmss}",
    Amount = 250.00m,
    Currency = Currency.UAH,
    OrderDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
    ProductName = ["Premium Subscription"],
    ProductCount = [1],
    ProductPrice = [250.00m],
    ClientFirstName = "John",
    ClientLastName = "Doe",
    ClientEmail = "john.doe@example.com",
    ClientPhone = "380501234567"
});
```

### Charging with Token

```csharp
var charge = await _client.ChargeWithTokenAsync(new ChargeRequest
{
    OrderReference = "ORDER-67890",
    Amount = 50.00m,
    Currency = Currency.UAH,
    OrderDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
    Token = "saved_card_token"
});
```

### Refunding a Transaction

```csharp
var refund = await _client.RefundAsync(new RefundRequest
{
    OrderReference = "ORDER-12345",
    Amount = 100.50m,
    Currency = Currency.UAH,
    Comment = "Customer requested refund"
});
```

### Checking Transaction Status

```csharp
var status = await _client.CheckStatusAsync(new StatusRequest
{
    OrderReference = "ORDER-12345"
});

if (status.TransactionStatus == TransactionStatus.Approved)
{
    // Payment successful
}
```

## Webhook Handling (ASP.NET Core)

```csharp
// Register webhook handler
builder.Services.AddWayForPayWebhooks();

// In your controller
[ApiController]
[Route("api/webhooks")]
public class WebhookController : ControllerBase
{
    private readonly IWayForPayWebhookHandler _handler;

    public WebhookController(IWayForPayWebhookHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("wayforpay")]
    public async Task<IActionResult> HandleWebhook([FromBody] WebhookRequest webhook)
    {
        if (!_handler.VerifySignature(webhook))
        {
            return Unauthorized();
        }

        // Process the webhook
        return Ok();
    }
}
```

## Documentation

For detailed documentation, see the [docs](./docs) folder:
- [Product Requirements Document](./docs/PRD.md)
- [Architecture Decision Records](./docs/adr)
- [API Reference](https://wiki.wayforpay.com/en/)

## Supported Operations

| Operation | Method | Description |
|-----------|--------|-------------|
| Purchase | `CreatePurchaseAsync` | Create a new payment |
| Charge | `ChargeAsync` | Charge a card |
| Charge with Token | `ChargeWithTokenAsync` | Charge using saved token |
| Refund | `RefundAsync` | Refund a transaction |
| Settlement | `SettleAsync` | Settle a transaction |
| Void | `VoidAsync` | Cancel a transaction |
| Check Status | `CheckStatusAsync` | Check transaction status |
| Verify | `VerifyAsync` | Verify merchant signature |
| Create Invoice | `CreateInvoiceAsync` | Create payment invoice |
| Complete 3DS | `Complete3DSAsync` | Complete 3D-Secure |

## Requirements

- .NET 8.0, 9.0, or 10.0
- Microsoft.Extensions.DependencyInjection 9.0+
- Microsoft.Extensions.Http 9.0+
- Microsoft.AspNetCore.App (for webhook support)

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

- [GitHub Issues](https://github.com/AratKruglik/WayForPaySDK/issues) - Report bugs or request features
- [WayForPay Documentation](https://wiki.wayforpay.com/en/) - Official API documentation

## Acknowledgments

- Built for the Ukrainian payment ecosystem
- Powered by [WayForPay](https://wayforpay.com)
