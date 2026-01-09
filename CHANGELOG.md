# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-01-09

### Added
- Initial release of WayForPaySDK
- Complete WayForPay API integration with all major operations:
  - **Purchase operations**: Standard purchase and purchase with regular payments
  - **Charge operations**: Standard charge, charge with token, and charge with regular payments
  - **Refund operations**: Full and partial refunds with transaction history
  - **Settlement operations**: Transaction settlement with status tracking
  - **Void operations**: Transaction cancellation
  - **Status check**: Real-time transaction status verification
  - **Verification**: Merchant signature verification
  - **Invoice creation**: Digital invoice generation
  - **3D-Secure**: Complete 3D-Secure authentication flow
  - **Transaction list**: Retrieve transaction history
- Multi-framework support (.NET 8.0, 9.0, 10.0)
- Async/await pattern for all API operations
- Strong typing with nullable reference types enabled
- HMAC-MD5 signature generation and verification
- Dependency injection support with `AddWayForPay()` extension
- ASP.NET Core webhook handling middleware
- Comprehensive XML documentation for IntelliSense
- Custom exception hierarchy for error handling:
  - `WayForPayException` - Base exception
  - `WayForPayApiException` - API-specific errors
  - `WayForPaySignatureException` - Signature verification failures
  - `WayForPayValidationException` - Request validation errors
- JSON serialization with custom converters
- HTTP client configuration with retry policies
- Configuration options via `IConfiguration` and `IOptions<T>`
- Unit tests with xUnit, Moq, and WireMock.Net
- Test coverage for critical paths

### Features
- Type-safe request/response models
- Automatic timestamp generation
- Currency support (UAH, USD, EUR, etc.)
- Product array handling for multi-item purchases
- Client information management (name, email, phone)
- Regular payment (recurring) support
- Card tokenization for saved payments
- Webhook signature verification
- Extensible architecture for custom implementations

### Documentation
- Comprehensive README with quick start guide
- Product Requirements Document (PRD)
- Architecture Decision Records (ADRs)
- API usage examples
- Configuration guide

[1.0.0]: https://github.com/AratKruglik/WayForPaySDK/releases/tag/v1.0.0
