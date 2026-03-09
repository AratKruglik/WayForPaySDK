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
- QR Code generation
- Split payments (Marketplace)
- MMS Marketplace management (partners, merchants, balances)
- Regular payment management (Suspend/Resume/Remove)
- P2P transfers (card-to-card and IBAN)

**Developer Experience**
- Async/await pattern throughout
- Strong typing with nullable reference types
- Domain models with built-in validation
- Dependency injection support
- Multi-framework targeting (.NET 8.0, 9.0, 10.0)
- ASP.NET Core webhook handling with extension methods
- HTML payment form generation

**Security Features**
- HMAC-MD5 signature verification (timing-safe via `CryptographicOperations.FixedTimeEquals`)
- HTTPS-only by default (`AllowInsecureHttp = false`)
- XSS protection in HTML form generation
- ReDoS protection in input validation
- Certificate pinning support

**Production Ready**
- Structured exception hierarchy
- Automatic request/response signature validation
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
// Via IConfigurationSection
builder.Services.AddWayForPay(builder.Configuration.GetSection("WayForPay"));

// Or via Action<WayForPayOptions> delegate
builder.Services.AddWayForPay(options =>
{
    options.MerchantAccount = "your_merchant_account";
    options.MerchantSecretKey = "your_secret_key";
    options.MerchantDomainName = "example.com";
    options.RequireResponseSignature = true;
    options.AllowInsecureHttp = false;
});
```

This registers `IWayForPayClient`, `IMmsClient`, `IWebhookHandler`, `ISignatureGenerator`, and configures `HttpClient` instances with the appropriate base address, timeout, and headers.

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
        var products = new[] { new Product { Name = "Test Product", Price = 100.50m, Count = 1 } };

        var response = await _client.CreatePurchaseAsync(
            orderReference: "ORDER-12345",
            amount: 100.50m,
            currency: "UAH",
            products: products);

        if (response.IsSuccess)
            Console.WriteLine($"Payment URL: {response.Url}");

        return response;
    }
}
```

## Configuration Reference

All `WayForPayOptions` properties:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `MerchantAccount` | `string` | *required* | Your WayForPay merchant account identifier |
| `MerchantSecretKey` | `string` | *required* | Secret key for HMAC signature generation |
| `MerchantDomainName` | `string` | *required* | Merchant domain name registered with WayForPay |
| `ApiBaseUrl` | `string` | `https://api.wayforpay.com/api` | API endpoint URL |
| `TimeoutSeconds` | `int` | `30` | HTTP request timeout (1-300 seconds) |
| `RequireResponseSignature` | `bool` | `true` | Verify HMAC signatures on API responses (fail-closed) |
| `AllowInsecureHttp` | `bool` | `false` | Allow HTTP URLs (blocks non-HTTPS when `false`) |
| `ServerCertificateCustomValidationCallback` | `Func<...>` | `null` | Custom TLS certificate validation for pinning |

### JSON Configuration

```json
{
  "WayForPay": {
    "MerchantAccount": "your_merchant_account",
    "MerchantSecretKey": "your_secret_key",
    "MerchantDomainName": "example.com",
    "ApiBaseUrl": "https://api.wayforpay.com/api",
    "TimeoutSeconds": 30,
    "RequireResponseSignature": true,
    "AllowInsecureHttp": false
  }
}
```

## Domain Models

The SDK provides strongly-typed domain models with built-in validation.

### Product

```csharp
var product = new Product { Name = "Premium Plan", Price = 299.99m, Count = 1 };

// Validation
product.ValidateAndThrow();       // throws ValidationException on failure
IReadOnlyList<string> errors = product.Validate();  // returns error list
bool valid = product.IsValid;     // quick check
```

### Card

```csharp
var card = new Card
{
    Number = "4111111111111111",
    ExpireMonth = 12,
    ExpireYear = 2027,
    Cvv = "123",
    Holder = "JOHN DOE"
};

try
{
    card.ValidateAndThrow();
}
catch (ValidationException ex)
{
    // ex.Errors contains: Luhn check failures, expiry validation, CVV format errors
    foreach (var error in ex.Errors)
        Console.WriteLine(error);
}
```

Validation includes: Luhn algorithm for card number, expiry date check, CVV format (3-4 digits), PAN length (13-19 digits).

### Client

All fields are optional. Validation only runs on provided (non-null) values.

```csharp
var client = new Client
{
    FirstName = "John",          // max 100 chars
    LastName = "Doe",            // max 100 chars
    Email = "john@example.com",  // regex validated
    Phone = "380501234567",      // 7-20 chars, regex validated
    Country = "UA",
    IpAddress = "192.168.1.1",   // IPAddress.TryParse validated
    Address = "123 Main St",
    City = "Kyiv",
    State = "Kyiv",
    ZipCode = "01001"
};

client.ValidateAndThrow();
```

### Regular (Subscriptions)

```csharp
var regular = new Regular
{
    Modes = [RegularMode.Monthly],
    Amount = 199.00m,
    Behavior = RegularBehavior.Preset,
    Count = 12,
    DateNext = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)),
    DateEnd = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1))
};
```

**`RegularMode` values:** `Client`, `None`, `Once`, `Daily`, `Weekly`, `Monthly`, `Quarterly`, `HalfYearly`, `Yearly`

**`RegularBehavior` values:** `None`, `Default`, `Preset`

### Split (Marketplace)

```csharp
var splits = new[]
{
    new Split { Id = "sub_merchant_1", Type = "flat", Value = "100" },
    new Split { Id = "sub_merchant_2", Type = "percentage", Value = "10" }
};
```

## API Operations

All operations are available through `IWayForPayClient`. Responses inherit from `ApiResponse` providing `IsSuccess`, `ReasonCode`, and `ReasonMessage`.

### Purchase (Redirect)

Creates a payment and returns a redirect URL for the customer.

```csharp
var products = new[] { new Product { Name = "Premium Subscription", Price = 250.00m, Count = 1 } };

var client = new Client
{
    FirstName = "John",
    LastName = "Doe",
    Email = "john.doe@example.com",
    Phone = "380501234567"
};

var response = await _client.CreatePurchaseAsync(
    orderReference: $"ORDER-{DateTime.UtcNow:yyyyMMddHHmmss}",
    amount: 250.00m,
    currency: "UAH",
    products: products,
    client: client,
    returnUrl: "https://example.com/payment/result",
    serviceUrl: "https://example.com/api/webhooks/wayforpay",
    language: "UA");

if (response.IsSuccess)
    Console.WriteLine($"Redirect customer to: {response.Url}");
```

### Purchase with Regular (Subscription via Redirect)

```csharp
var regular = new Regular
{
    Modes = [RegularMode.Monthly],
    Amount = 100.00m,
    Behavior = RegularBehavior.Preset,
    Count = 12
};

var response = await _client.CreatePurchaseWithRegularAsync(
    orderReference: "SUB-START-1",
    amount: 100.00m,
    currency: "UAH",
    products: [new Product { Name = "Monthly Plan", Price = 100.00m, Count = 1 }],
    regular: regular,
    returnUrl: "https://example.com/subscription/result");
```

### Direct Card Charge (S2S)

```csharp
var card = new Card
{
    Number = "4111111111111111",
    ExpireMonth = 12,
    ExpireYear = 2027,
    Cvv = "123",
    Holder = "JOHN DOE"
};

var response = await _client.ChargeAsync(
    orderReference: $"ORDER-CHARGE-{DateTime.UtcNow.Ticks}",
    amount: 100.00m,
    currency: "UAH",
    card: card,
    products: [new Product { Name = "Direct Charge", Price = 100.00m, Count = 1 }]);

if (response.Requires3DS())
{
    // Redirect to 3DS page — see 3D Secure section below
    var redirectUrl = response.Get3DSRedirectUrl();
}
else if (response.IsSuccess)
{
    Console.WriteLine($"Charged successfully. AuthCode: {response.AuthCode}");
}
```

### Charge with Token (recToken)

```csharp
var response = await _client.ChargeWithTokenAsync(
    orderReference: "ORDER-67890",
    amount: 50.00m,
    currency: "UAH",
    recToken: "saved_rec_token",
    products: [new Product { Name = "Recurring Charge", Price = 50.00m, Count = 1 }]);
```

### Charge with Regular (Subscription S2S)

```csharp
var regular = new Regular
{
    Modes = [RegularMode.Monthly],
    Amount = 100.00m,
    Behavior = RegularBehavior.None
};

var response = await _client.ChargeWithRegularAsync(
    orderReference: "SUB-PAYMENT-2",
    amount: 100.00m,
    currency: "UAH",
    card: card,
    products: [new Product { Name = "Subscription", Price = 100.00m, Count = 1 }],
    regular: regular);
```

### Check Status

```csharp
var response = await _client.CheckStatusAsync("ORDER-12345");

if (response.IsSuccess)
{
    Console.WriteLine($"Status: {response.TransactionStatus}");
    Console.WriteLine($"Card: {response.CardPan}");
}
```

### Refund

```csharp
var response = await _client.RefundAsync(
    orderReference: "ORDER-12345",
    amount: 100.50m,
    currency: "UAH",
    comment: "Customer requested refund");

if (response.IsSuccess)
    Console.WriteLine($"Refunded: {response.TransactionStatus}");
```

### Settle / Void (Two-Stage Payments)

```csharp
// Settle (Confirm/Capture authorized funds)
var settle = await _client.SettleAsync(
    orderReference: "ORDER-AUTH-1",
    amount: 100.00m,
    currency: "UAH");

// Void (Cancel authorization hold)
var voidResult = await _client.VoidAsync(
    orderReference: "ORDER-AUTH-1",
    amount: 100.00m,
    currency: "UAH",
    comment: "Cancelled by merchant");
```

### Invoice Creation

```csharp
var response = await _client.CreateInvoiceAsync(
    orderReference: $"INV-{DateTime.UtcNow.Ticks}",
    amount: 1500.00m,
    currency: "UAH",
    products: [new Product { Name = "Consulting Services", Price = 1500.00m, Count = 1 }],
    orderLifetime: 86400);

Console.WriteLine($"Invoice URL: {response.InvoiceUrl}");
```

### 3D Secure Flow

```csharp
// 1. Charge returns 3DS requirement
var charge = await _client.ChargeAsync(orderReference, amount, currency, card, products);

if (charge.Requires3DS())
{
    // 2. Redirect customer to 3DS page
    var acsUrl = charge.Get3DSRedirectUrl();
    var md = charge.Get3DSMd();
    var paReq = charge.Get3DSPaReq();
    // POST md + paReq to acsUrl in the customer's browser
}

// 3. Customer returns from 3DS — complete the flow
var result = await _client.Complete3DSAsync(
    d3Md: "md_value_from_bank",
    d3Pares: "pares_value_from_bank");

if (result.IsSuccess)
    Console.WriteLine($"3DS completed. RecToken: {result.RecToken}");
```

### Card Verification (recToken Generation)

```csharp
var card = new Card
{
    Number = "4111111111111111",
    ExpireMonth = 12,
    ExpireYear = 2027,
    Cvv = "123",
    Holder = "JOHN DOE"
};

var response = await _client.VerifyAsync(
    orderReference: $"VERIFY-{DateTime.UtcNow.Ticks}",
    card: card);

if (response.Requires3DS())
{
    var redirectUrl = response.Get3DSRedirectUrl();
    // Redirect customer for 3DS verification
}
else if (response.HasRecToken())
{
    // Card verified without 3DS, token available immediately
    Console.WriteLine($"RecToken: {response.RecToken}");
}
```

### QR Code Generation

```csharp
var response = await _client.CreateQrAsync(
    orderReference: $"QR-{DateTime.UtcNow.Ticks}",
    amount: 500.00m,
    currency: "UAH",
    products: [new Product { Name = "Coffee", Price = 500.00m, Count = 1 }]);

Console.WriteLine($"QR Code URL: {response.QrCodeUrl}");
```

### P2P Credit (Card-to-Card)

```csharp
var response = await _client.P2PCreditAsync(
    orderReference: $"P2P-{DateTime.UtcNow.Ticks}",
    amount: 1000.00m,
    currency: "UAH",
    cardBeneficiary: "4111111111111111");
```

### P2P Account (IBAN Transfer)

```csharp
var response = await _client.P2PAccountAsync(
    orderReference: $"IBAN-{DateTime.UtcNow.Ticks}",
    amount: 5000.00m,
    currency: "UAH",
    iban: "UA123456789012345678901234567",
    okpo: "12345678",
    accountName: "Beneficiary Name",
    description: "Payment for services");
```

### Transaction List

```csharp
var response = await _client.GetTransactionListAsync(
    dateBegin: DateTimeOffset.UtcNow.AddDays(-7),
    dateEnd: DateTimeOffset.UtcNow);

foreach (var tx in response.TransactionList)
{
    Console.WriteLine($"{tx.OrderReference}: {tx.Status} — {tx.Amount} {tx.Currency}");
    Console.WriteLine($"  Reason: [{tx.Reason.Code}] {tx.Reason.Message}");

    if (tx.IsStatusApproved) Console.WriteLine("  -> Approved");
    if (tx.IsStatusDeclined) Console.WriteLine("  -> Declined");
    if (tx.IsStatusRefunded) Console.WriteLine("  -> Refunded");
}
```

### Regular Management (Suspend/Resume/Remove)

```csharp
// Suspend a subscription
var suspend = await _client.SuspendRegularAsync("ORDER-SUB-123");
if (suspend.IsSuccess) Console.WriteLine("Subscription suspended");

// Resume a subscription
var resume = await _client.ResumeRegularAsync("ORDER-SUB-123");
if (resume.IsSuccess) Console.WriteLine("Subscription resumed");

// Cancel/Remove permanently
var remove = await _client.RemoveRegularAsync("ORDER-SUB-123");
if (remove.IsSuccess) Console.WriteLine("Subscription removed");
```

### MMS Marketplace API

MMS (Merchant Management System) operations are available through a separate `IMmsClient` interface. All 6 operations use the `/mms/{operation}.php` endpoint and HMAC-MD5 signatures.

#### Register a Partner

```csharp
public class MarketplaceService
{
    private readonly IMmsClient _mms;

    public MarketplaceService(IMmsClient mms)
    {
        _mms = mms;
    }

    public async Task<AddPartnerResponse> RegisterPartnerAsync()
    {
        var response = await _mms.AddPartnerAsync(
            partnerCode: "seller-001",
            site: "https://seller.example.com",
            phone: "+380501234567",
            email: "seller@example.com",
            description: "Electronics seller",
            compensationAccountIban: "UA213223130000026007233566001",
            compensationAccountOkpo: "12345678",
            compensationAccountName: "FOP Petrenko I.V.");

        if (response.IsSuccess)
            Console.WriteLine($"Partner registered: {response.PartnerCode}");

        return response;
    }
}
```

#### Get Partner Info

```csharp
var info = await _mms.GetPartnerInfoAsync("seller-001");

if (info.IsSuccess)
{
    Console.WriteLine($"Partner: {info.PartnerCode}");
    Console.WriteLine($"Status: {info.PartnerStatus}");
    Console.WriteLine($"Compensation: {info.Compensation}");
    Console.WriteLine($"Created: {info.CreateDate}");
}
```

#### Update Partner

```csharp
var result = await _mms.UpdatePartnerAsync(
    partnerCode: "seller-001",
    email: "new-email@example.com",
    compensationAccountIban: "UA999999999999999999999999999");

if (result.IsSuccess)
    Console.WriteLine($"Updated. New secret key: {result.SecretKey}");
```

#### Create a Merchant

```csharp
var response = await _mms.AddMerchantAsync(
    site: "https://shop.example.com",
    phone: "+380501234567",
    email: "shop@example.com",
    description: "Online electronics shop",
    compensationCardToken: "card_token_from_wayforpay");

if (response.IsSuccess)
{
    Console.WriteLine($"Merchant account: {response.MerchantAccount}");
    Console.WriteLine($"Secret key: {response.SecretKey}");
}
```

#### Get Merchant Info

```csharp
var info = await _mms.GetMerchantInfoAsync();

if (info.IsSuccess)
{
    Console.WriteLine($"Merchant: {info.MerchantAccount}");
    Console.WriteLine($"Site: {info.Site}");
    Console.WriteLine($"Status: {info.Status}");
}
```

#### Get Merchant Balance

```csharp
var balance = await _mms.GetMerchantBalanceAsync();

if (balance.IsSuccess)
    Console.WriteLine($"Balance: {balance.BalanceUah} UAH");

// With date filter
var balanceOnDate = await _mms.GetMerchantBalanceAsync(toDate: "01.03.2026");
```

## Payment Form Builder

`PaymentFormBuilder` generates HTML forms that auto-submit to WayForPay's secure payment page. It requires manual DI registration:

```csharp
builder.Services.AddSingleton<PaymentFormBuilder>();
```

### Generate HTML Form

```csharp
public class CheckoutController : Controller
{
    private readonly PaymentFormBuilder _formBuilder;

    public CheckoutController(PaymentFormBuilder formBuilder)
    {
        _formBuilder = formBuilder;
    }

    [HttpGet("checkout/{orderId}")]
    public IActionResult Checkout(string orderId)
    {
        var products = new[] { new Product { Name = "Order Payment", Price = 500.00m, Count = 1 } };

        var html = _formBuilder.CreatePurchaseFormHtml(
            orderReference: orderId,
            amount: 500.00m,
            currency: "UAH",
            products: products,
            returnUrl: "https://example.com/payment/result",
            serviceUrl: "https://example.com/api/webhooks/wayforpay");

        return Content(html, "text/html");
    }
}
```

### Two-Step Approach

```csharp
// 1. Create form data (for inspection or custom rendering)
PurchaseFormData formData = _formBuilder.CreatePurchaseForm(
    orderReference: "ORDER-1",
    amount: 100.00m,
    currency: "UAH",
    products: products);

// formData.Fields contains all key-value pairs including the signature
// formData.ActionUrl = "https://secure.wayforpay.com/pay"
// formData.AutoSubmit = true

// 2. Generate HTML from form data
string html = _formBuilder.GenerateHtml(formData);
```

### With Regular Payments

```csharp
var regular = new Regular
{
    Modes = [RegularMode.Monthly],
    Amount = 99.00m,
    Behavior = RegularBehavior.Preset,
    Count = 12
};

var html = _formBuilder.CreatePurchaseFormHtml(
    orderReference: "SUB-FORM-1",
    amount: 99.00m,
    currency: "UAH",
    products: products,
    regular: regular);
```

## Webhook Handling

`AddWayForPay()` automatically registers `IWebhookHandler` as a scoped service.

### Using Extension Methods (Recommended)

```csharp
[ApiController]
[Route("api/webhooks")]
public class WebhookController : ControllerBase
{
    private readonly IWebhookHandler _handler;

    public WebhookController(IWebhookHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("wayforpay")]
    public Task<IActionResult> HandleWebhook()
    {
        return _handler.HandleAsync(Request, async payload =>
        {
            if (payload.IsApproved)
                await ProcessApprovedPayment(payload.OrderReference, payload.Amount);
            else if (payload.IsDeclined)
                await HandleDeclinedPayment(payload.OrderReference);
        });
    }
}
```

### With Custom Status Response

```csharp
[HttpPost("wayforpay")]
public Task<IActionResult> HandleWebhook()
{
    return _handler.HandleAsync(Request, async payload =>
    {
        if (payload.IsApproved)
        {
            var processed = await TryProcessPayment(payload.OrderReference);
            return processed ? WebhookStatus.Accept : WebhookStatus.Decline;
        }

        return WebhookStatus.Accept;
    });
}
```

### With Logger

```csharp
[HttpPost("wayforpay")]
public Task<IActionResult> HandleWebhook([FromServices] ILogger<WebhookController> logger)
{
    return _handler.HandleAsync(Request, async payload =>
    {
        await ProcessPayment(payload);
    }, logger);
}
```

### Manual Approach

```csharp
[HttpPost("wayforpay")]
public async Task<IActionResult> HandleWebhook()
{
    var payload = await _handler.ParseAsync(Request);

    // Inspect payload
    Console.WriteLine($"Order: {payload.OrderReference}");
    Console.WriteLine($"Status: {payload.TransactionStatus}");
    Console.WriteLine($"Amount: {payload.Amount} {payload.Currency}");

    // Process based on status
    if (payload.IsApproved) { /* ... */ }
    if (payload.IsDeclined) { /* ... */ }
    if (payload.IsRefunded) { /* ... */ }
    if (payload.IsVoided) { /* ... */ }

    // Create and return response
    var response = _handler.CreateResponse(payload, WebhookStatus.Accept);
    return response.ToActionResult();
}
```

### WebhookPayload Properties

| Property | Description |
|----------|-------------|
| `IsSuccess` | `ReasonCode == 1100` |
| `IsApproved` | `TransactionStatus == "Approved"` |
| `IsDeclined` | `TransactionStatus == "Declined"` |
| `IsRefunded` | `TransactionStatus == "Refunded"` |
| `IsInProcessing` | `TransactionStatus == "InProcessing"` |
| `IsVoided` | `TransactionStatus == "Voided"` |
| `RecToken` | Recurring token for future charges |

**Idempotency:** WayForPay may send the same webhook multiple times. Always check if a payment has already been processed before applying business logic.

## Error Handling

The SDK provides a structured exception hierarchy rooted in `WayForPayException`:

```
WayForPayException
  ├── ApiException           — HTTP/API errors
  ├── SignatureException     — Signature mismatch
  ├── ValidationException    — Domain model validation
  ├── JsonParseException     — Deserialization failures
  └── InvalidFieldException  — Field-level errors
```

### Comprehensive Error Handling

```csharp
try
{
    var response = await _client.ChargeAsync(orderReference, amount, currency, card, products);

    if (!response.IsSuccess)
        Console.WriteLine($"API declined: [{response.ReasonCode}] {response.ReasonMessage}");
}
catch (ValidationException ex)
{
    // Domain model validation failed (e.g., invalid card number)
    foreach (var error in ex.Errors)
        Console.WriteLine($"Validation: {error}");
}
catch (SignatureException)
{
    // Response signature does not match (potential tampering)
    Console.WriteLine("Signature verification failed — possible tampering");
}
catch (ApiException ex)
{
    // HTTP or API-level error
    Console.WriteLine($"API error: [{ex.ReasonCode}] {ex.Reason}");
    if (ex.HttpStatusCode.HasValue)
        Console.WriteLine($"HTTP status: {ex.HttpStatusCode}");
}
catch (JsonParseException ex)
{
    // Failed to deserialize response
    Console.WriteLine($"Parse error: {ex.Message}");
    Console.WriteLine($"Raw content: {ex.RawContent}");
}
catch (InvalidFieldException ex)
{
    Console.WriteLine($"Invalid field '{ex.FieldName}': {ex.FieldValue}");
}
catch (WayForPayException ex)
{
    // Catch-all for any SDK exception
    Console.WriteLine($"WayForPay error: {ex.Message}");
}
```

## Security

### Response Signature Verification

Enabled by default (`RequireResponseSignature = true`). The SDK verifies HMAC-MD5 signatures on all API responses using timing-safe comparison. Disable only for testing:

```csharp
builder.Services.AddWayForPay(options =>
{
    // ...
    options.RequireResponseSignature = false; // NOT recommended for production
});
```

### HTTPS-Only Mode

By default, the SDK rejects non-HTTPS URLs. To allow HTTP (e.g., local development):

```csharp
builder.Services.AddWayForPay(options =>
{
    // ...
    options.AllowInsecureHttp = true; // Only for local development
});
```

### Certificate Pinning

```csharp
builder.Services.AddWayForPay(options =>
{
    // ...
    options.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
    {
        if (cert is null) return false;
        var expectedThumbprint = "EXPECTED_THUMBPRINT";
        return string.Equals(cert.GetCertHashString(), expectedThumbprint, StringComparison.OrdinalIgnoreCase);
    };
});
```

### Secret Key Management

Never store `MerchantSecretKey` in source code or `appsettings.json` in production. Use:

- **Azure Key Vault** / **AWS Secrets Manager** for cloud deployments
- **Environment variables** for containers: `WayForPay__MerchantSecretKey`
- **.NET User Secrets** for local development: `dotnet user-secrets set "WayForPay:MerchantSecretKey" "your_key"`

## 3D Secure Extension Methods

### ChargeResponseExtensions

| Method | Returns | Description |
|--------|---------|-------------|
| `Requires3DS()` | `bool` | `true` if `ThreeDsAcsUrl` is present |
| `Get3DSRedirectUrl()` | `string?` | ACS URL for 3DS redirect |
| `Get3DSMd()` | `string?` | Merchant Data for 3DS |
| `Get3DSPaReq()` | `string?` | Payment Authentication Request |
| `IsApprovedWithout3DS()` | `bool` | `IsSuccess && !Requires3DS()` |

### VerifyResponseExtensions

| Method | Returns | Description |
|--------|---------|-------------|
| `Requires3DS()` | `bool` | `true` if redirect URL is present |
| `Get3DSRedirectUrl()` | `string?` | Redirect URL for 3DS verification |
| `HasRecToken()` | `bool` | `IsSuccess` and `RecToken` is present |
| `IsApprovedWithout3DS()` | `bool` | `IsSuccess && !Requires3DS() && HasRecToken()` |

### Full 3DS Flow

```csharp
// 1. Attempt charge
var charge = await _client.ChargeAsync(orderRef, amount, currency, card, products);

if (charge.IsApprovedWithout3DS())
{
    // No 3DS required — payment complete
    return Ok(new { success = true, authCode = charge.AuthCode });
}

if (charge.Requires3DS())
{
    // Redirect to 3DS
    return Ok(new
    {
        requires3DS = true,
        acsUrl = charge.Get3DSRedirectUrl(),
        md = charge.Get3DSMd(),
        paReq = charge.Get3DSPaReq()
    });
}

// 2. After customer returns from 3DS
var result = await _client.Complete3DSAsync(md, pares);

if (result.IsSuccess)
    return Ok(new { success = true, recToken = result.RecToken });
```

## Enums Reference

### Currency
`UAH`, `USD`, `EUR`, `PLN`, `GBP`, `RUB`

### TransactionStatus
`Created`, `InProcessing`, `WaitingAuthComplete`, `Approved`, `Pending`, `Expired`, `Refunded`, `Voided`, `Declined`, `RefundInProcessing`

### PaymentSystem
`Card`, `Privat24`, `LpTerminal`, `Btc`, `BankCash`, `Credit`, `PayParts`, `QrCode`, `MasterPass`, `VisaCheckout`, `GooglePay`, `ApplePay`, `PayPartsMono`

### RegularMode
`Client`, `None`, `Once`, `Daily`, `Weekly`, `Monthly`, `Quarterly`, `HalfYearly`, `Yearly`

### RegularBehavior
`None`, `Default`, `Preset`

### MerchantTransactionType
`Sale`, `Auth`, `Refund`, `CheckStatus`, `Charge`, `Complete3ds`, `Pay`, `Privat`, `Settle`, `Void`, `Account`, `Credit`, `CreateQr`, `Suspend`, `Resume`, `Remove`

### Language
`UA`, `RU`, `EN`

## Reason Codes

The `ReasonCodes` static class provides constants and helper methods:

```csharp
// Check result category
bool ok = ReasonCodes.IsSuccess(response.ReasonCode);    // Ok, OkRegular, or Wait3DsData
bool needs3ds = ReasonCodes.IsWaiting3Ds(response.ReasonCode);
```

### Common Codes

| Code | Constant | Description |
|------|----------|-------------|
| 1100 | `Ok` | Transaction approved |
| 1101 | `DeclinedToCardIssuer` | Declined by issuing bank |
| 1102 | `BadCvv2` | Invalid CVV |
| 1103 | `ExpiredCard` | Card expired |
| 1104 | `InsufficientFunds` | Insufficient funds |
| 1105 | `InvalidCard` | Invalid card number |
| 1106 | `ExceedWithdrawalFrequency` | Withdrawal limit exceeded |
| 1108 | `ThreeDsAuthFail` | 3DS authentication failed |
| 1109 | `FormatError` | Request format error |
| 1112 | `DuplicateOrderId` | Duplicate order reference |
| 1113 | `InvalidSignature` | Signature mismatch |
| 1114 | `Fraud` | Suspected fraud |
| 1115 | `ParameterMissing` | Required parameter missing |
| 1116 | `TokenNotFound` | RecToken not found |
| 1121 | `AccountNotFound` | Account not found |
| 1127 | `OrderNotFound` | Order not found |
| 1130 | `InvalidAmount` | Invalid amount |
| 1131 | `TransactionInProcessing` | Transaction still processing |
| 4100 | `OkRegular` | Regular payment approved |
| 5100 | `Wait3DsData` | Awaiting 3DS authentication |

## Supported Operations

| Operation | Method | Description |
|-----------|--------|-------------|
| Purchase | `CreatePurchaseAsync` | Create a redirect payment |
| Purchase + Regular | `CreatePurchaseWithRegularAsync` | Create a subscription via redirect |
| Charge | `ChargeAsync` | Direct card charge (S2S) |
| Charge with Token | `ChargeWithTokenAsync` | Charge using saved recToken |
| Charge + Regular | `ChargeWithRegularAsync` | Subscription charge (S2S) |
| Refund | `RefundAsync` | Refund a transaction |
| Settlement | `SettleAsync` | Capture authorized funds |
| Void | `VoidAsync` | Cancel authorization hold |
| Check Status | `CheckStatusAsync` | Check transaction status |
| Verify | `VerifyAsync` | Verify card and get recToken |
| Create Invoice | `CreateInvoiceAsync` | Create a payment invoice |
| Complete 3DS | `Complete3DSAsync` | Complete 3D-Secure flow |
| Create QR | `CreateQrAsync` | Generate payment QR code |
| P2P Credit | `P2PCreditAsync` | Card-to-card transfer |
| P2P Account | `P2PAccountAsync` | IBAN transfer |
| Transaction List | `GetTransactionListAsync` | Get transaction history |
| Suspend Regular | `SuspendRegularAsync` | Pause recurring payment |
| Resume Regular | `ResumeRegularAsync` | Resume recurring payment |
| Remove Regular | `RemoveRegularAsync` | Cancel recurring payment |
| **MMS Marketplace** | **`IMmsClient`** | |
| Add Partner | `AddPartnerAsync` | Register a marketplace partner |
| Partner Info | `GetPartnerInfoAsync` | Get partner details and status |
| Update Partner | `UpdatePartnerAsync` | Update partner data |
| Add Merchant | `AddMerchantAsync` | Create a new merchant |
| Merchant Info | `GetMerchantInfoAsync` | Get merchant details and status |
| Merchant Balance | `GetMerchantBalanceAsync` | Get merchant UAH balance |

## Requirements

- .NET 8.0, 9.0, or 10.0
- Microsoft.Extensions.DependencyInjection 9.0+
- Microsoft.Extensions.Http 9.0+
- Microsoft.AspNetCore.App (for webhook support)

## Documentation

For detailed documentation, see the [docs](./docs) folder:
- [API Overview](./docs/api-overview.md)
- [Marketplace (MMS) API](./docs/marketplace/overview.md)
- [Product Requirements Document](./docs/PRD.md)
- [Architecture Decision Records](./docs/adr)
- [API Reference](https://wiki.wayforpay.com/en/)

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
