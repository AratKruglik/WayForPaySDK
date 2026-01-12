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
- **QR Code generation**
- **Split payments (Marketplace)**
- **Regular payment management (Suspend/Resume/Remove)**

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

### Creating a Payment (Redirect)

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

### Direct Card Charge (S2S)

```csharp
var charge = await _client.ChargeAsync(new ChargeRequest
{
    OrderReference = $"ORDER-CHARGE-{DateTime.UtcNow.Ticks}",
    Amount = 100.00m,
    Currency = Currency.UAH,
    OrderDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
    CardNumber = "4111111111111111",
    ExpMonth = "12",
    ExpYear = "2025",
    CardCvv = "123",
    CardHolder = "JOHN DOE",
    ProductName = ["Direct Charge"],
    ProductPrice = [100.00m],
    ProductCount = [1]
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

### Regular Payments (Subscriptions)

```csharp
// 1. Start a subscription via Purchase (Redirect)
var regularPurchase = await _client.CreatePurchaseWithRegularAsync(new PurchaseRequest
{
    OrderReference = "SUB-START-1",
    Amount = 100.00m,
    Currency = Currency.UAH,
    // ... products ...
    RegularBehavior = RegularBehavior.Preset,
    RegularMode = [RegularMode.Client], // Client can modify
    RegularAmount = 100.00m,
    RegularCount = 12, // 12 payments
    RegularOn = "month" // Monthly
});

// 2. Charge next payment (S2S)
var regularCharge = await _client.ChargeWithRegularAsync(new ChargeRequest
{
    OrderReference = "SUB-PAYMENT-2",
    Amount = 100.00m,
    // ... card details ...
    RegularBehavior = RegularBehavior.None // Continue existing behavior
});
```

### Managing Regular Payments

```csharp
// Suspend a subscription
await _client.SuspendRegularAsync("ORDER-SUB-123");

// Resume a subscription
await _client.ResumeRegularAsync("ORDER-SUB-123");

// Cancel/Remove a subscription permanently
await _client.RemoveRegularAsync("ORDER-SUB-123");
```

### Split Payments (Marketplace)

```csharp
var purchaseWithSplit = await _client.CreatePurchaseAsync(new PurchaseRequest
{
    OrderReference = "ORDER-SPLIT-1",
    Amount = 1000.00m,
    Currency = Currency.UAH,
    // ... other fields ...
    Splits =
    [
        // Transfer 100 UAH to sub-merchant 1
        new Split { Id = "merchant_id_1", Type = "flat", Value = "100" },
        // Transfer 10% to sub-merchant 2
        new Split { Id = "merchant_id_2", Type = "percentage", Value = "10" }
    ]
});
```

### Generating a Payment QR Code

```csharp
var qrResponse = await _client.CreateQrAsync(
    orderReference: $"QR-{DateTime.UtcNow.Ticks}",
    amount: 500.00m,
    currency: Currency.UAH,
    products: [new Product("Coffee", 500.00m, 1)]
);

// Use qrResponse.QrCodeUrl to display the QR code
Console.WriteLine($"QR Code URL: {qrResponse.QrCodeUrl}");
```

### Two-Stage Payments (Auth/Settle/Void)

```csharp
// 1. Authorize (Hold funds)
// Use MerchantTransactionType.Auth in your Purchase or Charge request

// 2. Settle (Confirm/Capture)
var settle = await _client.SettleAsync(new SettleRequest
{
    OrderReference = "ORDER-AUTH-1",
    Amount = 100.00m, // Can be partial amount
    Currency = Currency.UAH
});

// OR

// 3. Void (Cancel Hold)
var voidTx = await _client.VoidAsync(new VoidRequest
{
    OrderReference = "ORDER-AUTH-1",
    Amount = 100.00m,
    Currency = Currency.UAH
});
```

### Refunds

```csharp
var refund = await _client.RefundAsync(new RefundRequest
{
    OrderReference = "ORDER-12345",
    Amount = 100.50m,
    Currency = Currency.UAH,
    Comment = "Customer requested refund"
});
```

### P2P Transfers

```csharp
// Credit to Card
var p2pCredit = await _client.P2PCreditAsync(new P2PCreditRequest
{
    OrderReference = $"P2P-{DateTime.UtcNow.Ticks}",
    Amount = 1000.00m,
    Currency = Currency.UAH,
    CardBeneficiary = "4111111111111111" // Receiver card
});

// Transfer to IBAN
var p2pAccount = await _client.P2PAccountAsync(new P2PAccountRequest
{
    OrderReference = $"IBAN-{DateTime.UtcNow.Ticks}",
    Amount = 5000.00m,
    Currency = Currency.UAH,
    Iban = "UA12345678901234567890123456",
    Okpo = "12345678",
    AccountName = "Beneficiary Name",
    Description = "Payment for services"
});
```

### Transaction Info

```csharp
// Check single status
var status = await _client.CheckStatusAsync(new StatusRequest
{
    OrderReference = "ORDER-12345"
});

// Get list of transactions
var history = await _client.GetTransactionListAsync(new TransactionListRequest
{
    DateBegin = DateTimeOffset.UtcNow.AddDays(-7).ToUnixTimeSeconds(),
    DateEnd = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
});
```

### Invoice Creation

```csharp
var invoice = await _client.CreateInvoiceAsync(new InvoiceRequest
{
    OrderReference = $"INV-{DateTime.UtcNow.Ticks}",
    Amount = 1500.00m,
    Currency = Currency.UAH,
    ProductName = ["Consulting Services"],
    ProductPrice = [1500.00m],
    ProductCount = [1],
    OrderLifetime = 86400 // 24 hours
});

Console.WriteLine($"Invoice URL: {invoice.InvoiceUrl}");
```

### Complete 3D Secure

```csharp
// When you receive 3DS callback (md, pares)
var result = await _client.Complete3DSAsync(new Complete3DSRequest
{
    D3Md = "md_value_from_bank",
    D3Pares = "pares_value_from_bank"
});
```

### Verify Card

```csharp
var verification = await _client.VerifyAsync(new VerifyRequest
{
    OrderReference = $"VERIFY-{DateTime.UtcNow.Ticks}",
    CardNumber = "4111111111111111",
    ExpMonth = "12",
    ExpYear = "2025",
    CardCvv = "123",
    CardHolder = "JOHN DOE"
});
// Creates a temporary hold (e.g. 1 UAH) which is auto-reversed
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
| Create QR | `CreateQrAsync` | Generate payment QR code |
| Suspend Regular | `SuspendRegularAsync` | Suspend recurring payment |
| Resume Regular | `ResumeRegularAsync` | Resume recurring payment |
| Remove Regular | `RemoveRegularAsync` | Remove recurring payment |

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
