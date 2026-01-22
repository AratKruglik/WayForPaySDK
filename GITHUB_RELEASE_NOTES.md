# WayForPaySDK v1.2.0

This release significantly expands the SDK capabilities, bringing full support for marketplace split payments, QR code generation, and advanced recurring payment management.

## What's New

### 🚀 Split Payments (Marketplace Support)
You can now distribute a single transaction's funds across multiple sub-merchants. This is essential for marketplace platforms.
- Added `Splits` property to `PurchaseRequest` and `ChargeRequest`.
- Support for `flat`, `percentage`, and `remaining` split types.

### 📱 QR Code Payments
Generate payment QR codes directly via the API for use in mobile apps or physical locations.
- New method: `CreateQrAsync`.
- Returns a direct URL to the generated QR code image.

### 🔄 Advanced Subscription Management
Full control over recurring payments beyond just creating them.
- `SuspendRegularAsync`: Temporarily stop a subscription.
- `ResumeRegularAsync`: Restore a suspended subscription.
- `RemoveRegularAsync`: Permanently cancel a recurring payment.

### 🛠 Improvements & Fixes
- **JSON Source Generation**: Updated `WayForPayJsonContext` for better performance and Native AOT compatibility with new models.
- **Extended Documentation**: `README.md` now contains code examples for **every** supported operation.
- **Unit Tests**: Added comprehensive test coverage for all new request types and signature generation logic.

## Installation

```bash
dotnet add package WayForPaySDK --version 1.2.0
```

## Quick Example: QR Code

```csharp
var qrResponse = await _client.CreateQrAsync(
    orderReference: "QR-123",
    amount: 100.00m,
    currency: "UAH",
    products: [new Product("Product Name", 100.00m, 1)]
);
Console.WriteLine($"Scan to pay: {qrResponse.QrCodeUrl}");
```

See [README.md](https://github.com/AratKruglik/WayForPaySDK/blob/main/README.md) for full documentation and more examples.
