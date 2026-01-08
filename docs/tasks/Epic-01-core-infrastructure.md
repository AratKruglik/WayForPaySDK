# Epic-01: Core Infrastructure

## Огляд

Цей Epic охоплює базову інфраструктуру WayForPaySDK: доменні моделі, генерацію підписів, HTTP клієнт, Dependency Injection інтеграцію, систему винятків та JSON серіалізацію.

**Ціль:** Створити фундамент SDK, на якому будуть побудовані всі платіжні операції.

## Метадані

| Атрибут | Значення |
|---------|----------|
| **Epic ID** | Epic-01 |
| **User Stories** | US-001 — US-028 (28 stories) |
| **Приблизний обсяг** | ~50 Story Points |
| **Залежності** | Немає (базовий Epic) |
| **Пріоритет** | Critical |

## Залежності

Цей Epic не має залежностей від інших Epic-ів. Всі інші Epic-и залежать від нього.

```
Epic-01 (Core Infrastructure)
    │
    ├──► Epic-02 (Payment Operations)
    ├──► Epic-03 (3D Secure & Advanced)
    ├──► Epic-04 (Invoice & Forms)
    ├──► Epic-05 (Webhook Integration)
    └──► Epic-06 (Builders & Polish)
```

---

## User Stories

### Секція 1: Project Setup

---

### US-001: Project Setup and Multi-Target Configuration

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a .NET developer
I want the SDK project to be properly configured with multi-target frameworks (net6.0, net8.0, net9.0, net10.0)
So that I can use the SDK across different .NET versions in my projects

**Acceptance Criteria:**

```gherkin
Scenario: Project targets multiple .NET versions
  Given I have the WayForPaySDK source code
  When I build the project
  Then it should produce assemblies for net6.0, net8.0, net9.0, and net10.0

Scenario: NuGet package contains all targets
  Given I have built the SDK in Release mode
  When I create a NuGet package
  Then the package should contain assemblies for all target frameworks

Scenario: Modern C# features are available
  Given the project uses LangVersion 12.0 or latest
  When I write code using records, required members, and file-scoped namespaces
  Then the code should compile successfully
```

**Технічні нотатки:**
- Використовувати `<TargetFrameworks>net6.0;net8.0;net9.0;net10.0</TargetFrameworks>`
- Увімкнути `<Nullable>enable</Nullable>` та `<ImplicitUsings>enable</ImplicitUsings>`
- Налаштувати `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` для Release
- Додати NuGet metadata (PackageId, Authors, Description, Tags, License)

**Залежності:** Немає

**Референси:**
- ADR: ADR-008-multi-target-framework.md

---

### Секція 2: Domain Enums and Constants

---

### US-002: Domain Enums Implementation

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want strongly-typed enums for TransactionType, TransactionStatus, SecureType, PaymentSystem, RegularMode
So that I can use type-safe values instead of magic strings in my code

**Acceptance Criteria:**

```gherkin
Scenario: TransactionType enum contains all values
  Given I need to specify transaction type
  When I use the TransactionType enum
  Then I can choose from Auto, Sale, Auth values

Scenario: TransactionStatus enum contains all values
  Given I receive a transaction response
  When I check TransactionStatus
  Then I can compare against Approved, Pending, InProcessing, WaitingAuthComplete, Declined, Refunded, Expired, Voided

Scenario: PaymentSystem is a flags enum
  Given I want to specify multiple payment systems
  When I combine PaymentSystem values with bitwise OR
  Then I get a valid combination like PaymentSystem.Card | PaymentSystem.GooglePay

Scenario: Enums serialize to WayForPay API format
  Given I have a TransactionType.Auth value
  When it is serialized to JSON
  Then it should produce "AUTH" string
```

**Технічні нотатки:**
- TransactionType: Auto, Sale, Auth
- SecureType: Auto, ThreeDs, NonThreeDs
- TransactionStatus: Approved, Pending, InProcessing, WaitingAuthComplete, Declined, Refunded, Expired, Voided
- PaymentSystem: [Flags] enum з Card, Privat24, ApplePay, GooglePay, MasterPass, VisaCheckout, PayParts, PayPartsMono, Credit, QrCode
- RegularMode: Once, Daily, Weekly, Monthly, Quarterly, Halfyearly, Yearly, Client
- Використовувати JsonStringEnumConverter для серіалізації

**Залежності:** US-001

**Референси:**
- PRD: Section 3.11 (Payment Systems), Section 3.12 (Recurring)
- ADR: ADR-003-domain-models-design.md

---

### US-025: ReasonCodes Constants Class

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a ReasonCodes static class with all WayForPay error codes as constants
So that I can check specific error conditions without using magic numbers

**Acceptance Criteria:**

```gherkin
Scenario: ReasonCodes contains success code
  Given I receive a response with reasonCode 1100
  When I compare with ReasonCodes.Ok
  Then they should be equal

Scenario: ReasonCodes contains common error codes
  Given I need to check for specific errors
  When I use ReasonCodes constants
  Then I can check for InvalidSignature (1102), InsufficientFunds (1104), ThreeDsRequired (1112)

Scenario: ReasonCodes has descriptive comments
  Given I use IntelliSense on ReasonCodes
  When I hover over a constant
  Then I see XML documentation describing the error
```

**Технічні нотатки:**
- Ok = 1100
- InvalidMerchantData = 1101
- InvalidSignature = 1102
- InsufficientFunds = 1104
- OrderAlreadyPaid = 1105
- InvalidCardData = 1108
- InvalidCvv = 1109
- CardExpired = 1110
- ThreeDsRequired = 1112
- TransactionDeclined = 1130
- MerchantBlocked = 1131
- InvalidAmount = 1132
- CurrencyNotAllowed = 1133

**Залежності:** US-001

**Референси:**
- PRD: Section 3.2 (Reason Codes table)

---

### Секція 3: Domain Models

---

### US-003: Card Domain Model

**Статус:** Draft
**Story Points:** XS (1)

**Опис:**
As a developer
I want a Card record with required properties (Number, ExpireMonth, ExpireYear, Cvv, Holder)
So that I can pass card data to payment operations in a type-safe manner

**Acceptance Criteria:**

```gherkin
Scenario: Card requires all properties
  Given I create a Card instance
  When I don't provide Number, ExpireMonth, ExpireYear, Cvv, or Holder
  Then compilation should fail with required member error

Scenario: Card is immutable
  Given I have a Card instance
  When I try to modify any property
  Then compilation should fail (init-only setters)

Scenario: Card uses record equality
  Given I have two Card instances with same values
  When I compare them with ==
  Then they should be equal
```

**Технічні нотатки:**
- Використовувати `public sealed record Card`
- Всі властивості з `required` модифікатором
- Number: string (16 цифр)
- ExpireMonth: int (1-12)
- ExpireYear: int (4 цифри)
- Cvv: string (3-4 цифри)
- Holder: string (латиницею)

**Залежності:** US-001

**Референси:**
- PRD: Section 6.1 (Card model)
- ADR: ADR-003-domain-models-design.md

---

### US-004: CardToken Domain Model

**Статус:** Draft
**Story Points:** XS (1)

**Опис:**
As a developer
I want a CardToken record for recurring payments (Token, CardPan, CardType)
So that I can use tokens for subsequent charges without storing card data

**Acceptance Criteria:**

```gherkin
Scenario: CardToken requires Token property
  Given I create a CardToken instance
  When I don't provide Token
  Then compilation should fail

Scenario: CardToken has optional card info
  Given I create a CardToken
  When I don't provide CardPan or CardType
  Then the instance is created with null values for optional properties

Scenario: CardToken from API response
  Given I receive a charge response with recToken
  When I extract CardToken
  Then it contains Token, CardPan mask, and CardType
```

**Технічні нотатки:**
- `public sealed record CardToken`
- Token: required string
- CardPan: string? (маска типу 411111****1111)
- CardType: string? (Visa, MasterCard, etc.)

**Залежності:** US-001

**Референси:**
- PRD: Section 6.1 (CardToken model)
- ADR: ADR-003-domain-models-design.md

---

### US-005: Client Domain Model

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a Client record with customer information (FirstName, LastName, Email, Phone, etc.)
So that I can pass client data to payment operations

**Acceptance Criteria:**

```gherkin
Scenario: Client requires core properties
  Given I create a Client instance
  When I provide FirstName, LastName, Email, Phone
  Then the instance is created successfully

Scenario: Client has optional address properties
  Given I create a Client
  When I don't provide Country, IpAddress, Address, City, State, ZipCode
  Then the instance is created with null values for optional properties

Scenario: Client AccountId is optional
  Given I want to link payment to internal customer
  When I set AccountId property
  Then it can be used for tracking in my system
```

**Технічні нотатки:**
- `public sealed record Client`
- Required: FirstName, LastName, Email, Phone
- Optional: AccountId, Country (ISO 3166-1 alpha-3), IpAddress, Address, City, State, ZipCode

**Залежності:** US-001

**Референси:**
- PRD: Section 6.1 (Client model)
- ADR: ADR-003-domain-models-design.md

---

### US-006: Product Domain Model

**Статус:** Draft
**Story Points:** XS (1)

**Опис:**
As a developer
I want a Product record (Name, Price, Count)
So that I can specify order items in payment requests

**Acceptance Criteria:**

```gherkin
Scenario: Product requires all properties
  Given I create a Product instance
  When I provide Name, Price, Count
  Then the instance is created successfully

Scenario: Product Price is decimal
  Given I create a Product with price 99.99
  When I serialize it to JSON
  Then the price is formatted correctly for WayForPay API

Scenario: Product Count is positive integer
  Given I create a Product
  When I set Count to 0 or negative
  Then validation should fail (at build time or runtime)
```

**Технічні нотатки:**
- `public sealed record Product`
- Name: required string
- Price: required decimal
- Count: required int

**Залежності:** US-001

**Референси:**
- PRD: Section 6.1 (Product model)
- ADR: ADR-003-domain-models-design.md

---

### US-007: Transaction Domain Model

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want a Transaction record with all response fields (OrderReference, Amount, Currency, Status, AuthCode, etc.)
So that I can process API responses and access all transaction details

**Acceptance Criteria:**

```gherkin
Scenario: Transaction contains order info
  Given I receive a charge response
  When I access Transaction
  Then I can read OrderReference, Amount, Currency, TransactionStatus

Scenario: Transaction contains bank info
  Given a successful payment
  When I access Transaction
  Then I can read AuthCode, CardPan, CardType, IssuerBankCountry, IssuerBankName

Scenario: Transaction contains 3DS info when required
  Given a payment requiring 3DS
  When I access Transaction
  Then I can read D3AcsUrl, D3Md, D3Pareq for redirect

Scenario: Transaction contains recToken for recurring
  Given a successful payment with card
  When I access Transaction.RecToken
  Then I can use it for subsequent charges
```

**Технічні нотатки:**
- `public sealed record Transaction`
- Core: OrderReference, Amount, Currency, TransactionStatus, MerchantTransactionType
- Timestamps: CreatedDate (DateTimeOffset), ProcessingDate (DateTimeOffset?)
- Result: ReasonCode, Reason
- Bank: AuthCode?, AuthTicket?
- Card: CardPan?, CardType?, IssuerBankCountry?, IssuerBankName?
- Recurring: RecToken? (CardToken)
- 3DS: D3AcsUrl?, D3Md?, D3Pareq?
- Client: Email?, Phone?
- Finance: PaymentSystem?, Fee?, BaseAmount?, BaseCurrency?

**Залежності:** US-001, US-002, US-004

**Референси:**
- PRD: Section 6.1 (Transaction model)
- ADR: ADR-003-domain-models-design.md

---

### US-008: Reason Domain Model

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a Reason record (Code, Message) with helper properties (IsSuccess, Is3DsRequired, IsPending)
So that I can easily check operation results without comparing magic numbers

**Acceptance Criteria:**

```gherkin
Scenario: Reason.IsSuccess returns true for code 1100
  Given I have a Reason with Code = 1100
  When I check IsSuccess
  Then it returns true

Scenario: Reason.Is3DsRequired returns true for code 1112
  Given I have a Reason with Code = 1112
  When I check Is3DsRequired
  Then it returns true

Scenario: Reason.IsPending returns true for processing codes
  Given I have a Reason with Code indicating in-processing
  When I check IsPending
  Then it returns true

Scenario: Reason has descriptive Message
  Given I receive an API response
  When I access Reason.Message
  Then it contains human-readable description
```

**Технічні нотатки:**
- `public sealed record Reason`
- Code: required int
- Message: required string
- IsSuccess => Code == ReasonCodes.Ok (1100)
- Is3DsRequired => Code == ReasonCodes.ThreeDsRequired (1112)
- IsPending => Code is in processing range

**Залежності:** US-001, US-025

**Референси:**
- PRD: Section 6.1 (Reason model)
- ADR: ADR-003-domain-models-design.md

---

### US-009: RegularPaymentSettings Domain Model

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a RegularPaymentSettings record (Modes, Amount, DateNext, DateEnd, Count)
So that I can configure recurring payments for subscriptions

**Acceptance Criteria:**

```gherkin
Scenario: RegularPaymentSettings requires core properties
  Given I create a RegularPaymentSettings instance
  When I provide Modes, Amount, DateNext
  Then the instance is created successfully

Scenario: DateEnd and Count are mutually exclusive alternatives
  Given I configure regular payment
  When I want to limit payments
  Then I can set either DateEnd OR Count (not both required)

Scenario: IsActive defaults to true
  Given I create RegularPaymentSettings without specifying IsActive
  When I check IsActive
  Then it returns true
```

**Технічні нотатки:**
- `public sealed record RegularPaymentSettings`
- Modes: required RegularMode[]
- Amount: required decimal
- DateNext: required DateTimeOffset
- DateEnd: DateTimeOffset? (альтернатива Count)
- Count: int? (альтернатива DateEnd)
- IsActive: bool = true

**Залежності:** US-001, US-002

**Референси:**
- PRD: Section 3.12 (Recurring Payments), Section 6.1 (RegularPaymentSettings)
- ADR: ADR-003-domain-models-design.md

---

### Секція 4: Signature Generation

---

### US-010: ISignatureGenerator Interface

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want an ISignatureGenerator interface with GenerateSignature and ValidateSignature methods
So that signature logic can be abstracted and tested independently

**Acceptance Criteria:**

```gherkin
Scenario: ISignatureGenerator can be mocked
  Given I write unit tests for WayForPayClient
  When I need to mock signature generation
  Then I can substitute ISignatureGenerator with a mock

Scenario: GenerateSignature accepts field array and secret
  Given I have an array of signature fields
  When I call GenerateSignature(fields, secret)
  Then it returns HMAC-MD5 signature string

Scenario: ValidateSignature compares signatures safely
  Given I have expected and actual signatures
  When I call ValidateSignature(expected, fields, secret)
  Then it returns true only if signatures match
```

**Технічні нотатки:**
- Interface: `ISignatureGenerator`
- Methods:
  - `string GenerateSignature(string[] fields, string secretKey)`
  - `bool ValidateSignature(string signature, string[] fields, string secretKey)`

**Залежності:** US-001

**Референси:**
- ADR: ADR-002-signature-generation.md

---

### US-011: HmacMd5SignatureGenerator Implementation

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want an HmacMd5SignatureGenerator class that generates HMAC-MD5 signatures
So that requests are properly signed according to WayForPay requirements

**Acceptance Criteria:**

```gherkin
Scenario: Signature generated with correct algorithm
  Given I have fields ["merchant", "ORDER123", "100", "UAH"]
  And secret key "my_secret"
  When I call GenerateSignature
  Then it produces correct HMAC-MD5 hash

Scenario: Fields joined with semicolon separator
  Given I have multiple fields
  When signature is calculated
  Then fields are joined with ";" before hashing

Scenario: Empty or null fields handled correctly
  Given some fields are empty strings
  When signature is calculated
  Then empty strings are included in concatenation

Scenario: Signature is lowercase hex string
  Given I generate a signature
  When I check the format
  Then it is 32-character lowercase hexadecimal string
```

**Технічні нотатки:**
- Реалізує `ISignatureGenerator`
- Алгоритм: HMAC-MD5
- Кодування: UTF-8
- Роздільник: `;` (semicolon)
- Формат виводу: lowercase hex
- Використовувати `System.Security.Cryptography.HMACMD5`
- Клас має бути thread-safe

**Залежності:** US-010

**Референси:**
- PRD: Section 4.2 (Security - HMAC-MD5)
- ADR: ADR-002-signature-generation.md

---

### US-012: Signature Validation with Timing-Safe Comparison

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want signature validation using CryptographicOperations.FixedTimeEquals
So that the SDK is protected against timing attacks

**Acceptance Criteria:**

```gherkin
Scenario: Valid signature returns true
  Given I have a valid signature from WayForPay
  When I call ValidateSignature
  Then it returns true

Scenario: Invalid signature returns false
  Given I have a tampered signature
  When I call ValidateSignature
  Then it returns false

Scenario: Comparison is timing-safe
  Given an attacker tries timing attack
  When comparing signatures
  Then response time is constant regardless of which byte differs

Scenario: Case-insensitive comparison
  Given signatures differ only in case
  When I compare "abc123" with "ABC123"
  Then validation should handle it correctly (normalize to lowercase)
```

**Технічні нотатки:**
- Використовувати `CryptographicOperations.FixedTimeEquals` для порівняння
- Конвертувати обидва підписи до lowercase перед порівнянням
- Конвертувати hex strings до byte arrays для FixedTimeEquals

**Залежності:** US-011

**Референси:**
- ADR: ADR-002-signature-generation.md (Security Considerations)

---

### Секція 5: Configuration

---

### US-013: WayForPayOptions Configuration Class

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a WayForPayOptions class (MerchantAccount, MerchantSecretKey, MerchantDomainName, Timeout, UseSandbox)
So that I can configure SDK through the options pattern

**Acceptance Criteria:**

```gherkin
Scenario: Options can be configured from appsettings.json
  Given I have WayForPay section in appsettings.json
  When I register SDK with AddWayForPay(configuration)
  Then options are bound from configuration

Scenario: Options can be configured inline
  Given I use AddWayForPay(options => {...})
  When I set properties inline
  Then SDK uses those values

Scenario: Timeout has sensible default
  Given I don't set Timeout explicitly
  When SDK makes HTTP requests
  Then default timeout of 30 seconds is used

Scenario: UseSandbox changes API endpoint
  Given I set UseSandbox = true
  When SDK makes requests
  Then it uses sandbox API endpoint
```

**Технічні нотатки:**
- `public class WayForPayOptions`
- MerchantAccount: string (required)
- MerchantSecretKey: string (required)
- MerchantDomainName: string (required)
- Timeout: TimeSpan = TimeSpan.FromSeconds(30)
- UseSandbox: bool = false
- Можливо: MerchantPassword для альтернативної автентифікації

**Залежності:** US-001

**Референси:**
- PRD: Section 8.1 (Basic Configuration)
- ADR: ADR-006-dependency-injection.md

---

### US-014: WayForPayOptions Validator

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want options validation at startup (IValidateOptions)
So that configuration errors are detected early before any API calls

**Acceptance Criteria:**

```gherkin
Scenario: Missing MerchantAccount throws at startup
  Given I don't configure MerchantAccount
  When application starts
  Then OptionsValidationException is thrown with clear message

Scenario: Missing MerchantSecretKey throws at startup
  Given I don't configure MerchantSecretKey
  When application starts
  Then OptionsValidationException is thrown

Scenario: Invalid Timeout throws at startup
  Given I set Timeout to zero or negative
  When application starts
  Then OptionsValidationException is thrown

Scenario: Valid configuration starts successfully
  Given all required options are configured
  When application starts
  Then no validation errors occur
```

**Технічні нотатки:**
- Реалізувати `IValidateOptions<WayForPayOptions>`
- Валідації:
  - MerchantAccount: not null or whitespace
  - MerchantSecretKey: not null or whitespace
  - MerchantDomainName: not null or whitespace
  - Timeout: > TimeSpan.Zero

**Залежності:** US-013

**Референси:**
- ADR: ADR-006-dependency-injection.md

---

### Секція 6: HTTP Client

---

### US-015: WayForPayHttpClient Typed Client

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want a typed HTTP client with proper configuration (BaseAddress, Timeout, Headers)
So that HTTP requests are properly formatted for WayForPay API

**Acceptance Criteria:**

```gherkin
Scenario: HTTP client uses correct base address
  Given UseSandbox is false
  When HTTP client is created
  Then BaseAddress is "https://api.wayforpay.com/"

Scenario: HTTP client sets Content-Type header
  Given I make a POST request
  When request is sent
  Then Content-Type is "application/json; charset=utf-8"

Scenario: HTTP client respects timeout configuration
  Given Timeout is set to 45 seconds
  When HTTP client is created
  Then request timeout is 45 seconds

Scenario: HTTP client is properly disposed
  Given I'm done with the client
  When it's disposed through DI
  Then underlying resources are released
```

**Технічні нотатки:**
- Використовувати typed client pattern з IHttpClientFactory
- BaseAddress: "https://api.wayforpay.com/" (production) або sandbox URL
- Default headers: Accept: application/json
- Timeout з WayForPayOptions

**Залежності:** US-013

**Референси:**
- PRD: Section 3.1 (API Endpoints)
- ADR: ADR-001-http-client-strategy.md

---

### US-016: HTTP Client Connection Pooling Configuration

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want SocketsHttpHandler with PooledConnectionLifetime configured
So that DNS refresh and connection management work correctly

**Acceptance Criteria:**

```gherkin
Scenario: Connection pool refreshes for DNS changes
  Given DNS record for api.wayforpay.com changes
  When PooledConnectionLifetime expires (2 minutes)
  Then new connections use updated DNS

Scenario: Connections are reused for performance
  Given I make multiple API calls
  When calls are within connection lifetime
  Then HTTP connections are reused

Scenario: Connection pool handles high load
  Given I make many concurrent requests
  When connection pool is configured
  Then requests don't exhaust socket resources
```

**Технічні нотатки:**
- Використовувати `SocketsHttpHandler` з `PooledConnectionLifetime = TimeSpan.FromMinutes(2)`
- Конфігурувати через `ConfigurePrimaryHttpMessageHandler`
- Це вирішує проблему DNS caching при load balancer змінах

**Залежності:** US-015

**Референси:**
- ADR: ADR-001-http-client-strategy.md

---

### Секція 7: Dependency Injection

---

### US-017: AddWayForPay Extension Method (Action overload)

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want an AddWayForPay(Action<WayForPayOptions>) extension method
So that I can register SDK services with inline configuration

**Acceptance Criteria:**

```gherkin
Scenario: Services are registered with inline config
  Given I have IServiceCollection
  When I call AddWayForPay(opt => { opt.MerchantAccount = "..."; })
  Then IWayForPayClient, ISignatureGenerator, IWebhookHandler are registered

Scenario: Configuration is applied
  Given I configure options inline
  When I resolve IWayForPayClient
  Then it uses my configuration values

Scenario: Method returns IHttpClientBuilder
  Given I call AddWayForPay
  When I chain additional configuration
  Then I can add Polly policies via IHttpClientBuilder
```

**Технічні нотатки:**
- Extension method на `IServiceCollection`
- Реєструє:
  - `IOptions<WayForPayOptions>` - Singleton
  - `ISignatureGenerator` → `HmacMd5SignatureGenerator` - Singleton
  - `IWayForPayClient` → `WayForPayClient` - Typed HttpClient
  - `IWebhookHandler` → `WebhookHandler` - Scoped
- Повертає `IHttpClientBuilder` для extensibility

**Залежності:** US-013, US-014, US-015, US-011

**Референси:**
- PRD: Section 7.4 (DI Extension Methods)
- ADR: ADR-006-dependency-injection.md

---

### US-018: AddWayForPay Extension Method (IConfiguration overload)

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want an AddWayForPay(IConfiguration) extension method
So that I can configure SDK from appsettings.json

**Acceptance Criteria:**

```gherkin
Scenario: Configuration bound from section
  Given I have appsettings.json with WayForPay section
  When I call AddWayForPay(configuration.GetSection("WayForPay"))
  Then options are bound from JSON

Scenario: All properties are mapped
  Given JSON has MerchantAccount, MerchantSecretKey, MerchantDomainName, Timeout
  When configuration is bound
  Then all properties have correct values

Scenario: Timeout parsed from string
  Given JSON has Timeout: "00:00:45"
  When configuration is bound
  Then Timeout is 45 seconds TimeSpan
```

**Технічні нотатки:**
- Overload: `AddWayForPay(this IServiceCollection services, IConfiguration configuration)`
- Використовувати `services.Configure<WayForPayOptions>(configuration)`
- Делегувати до Action overload для реєстрації сервісів

**Залежності:** US-017

**Референси:**
- PRD: Section 8.1 (Basic Configuration from JSON)
- ADR: ADR-006-dependency-injection.md

---

### US-019: IHttpClientBuilder Return for Extensibility

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want AddWayForPay to return IHttpClientBuilder
So that I can add Polly policies and custom handlers

**Acceptance Criteria:**

```gherkin
Scenario: Polly retry policy can be added
  Given I call AddWayForPay
  When I chain .AddTransientHttpErrorPolicy(...)
  Then retry policy is applied to HTTP calls

Scenario: Custom delegating handler can be added
  Given I have logging handler
  When I chain .AddHttpMessageHandler<MyHandler>()
  Then my handler intercepts all requests

Scenario: Circuit breaker can be configured
  Given I want circuit breaker protection
  When I chain .AddPolicyHandler(circuitBreakerPolicy)
  Then circuit breaker activates on failures
```

**Технічні нотатки:**
- `AddWayForPay` повертає `IHttpClientBuilder`
- Це дозволяє chaining як:
  ```csharp
  services.AddWayForPay(opt => {...})
          .AddTransientHttpErrorPolicy(p => p.RetryAsync(3));
  ```

**Залежності:** US-017

**Референси:**
- ADR: ADR-006-dependency-injection.md (Extensibility)

---

### Секція 8: Exception Hierarchy

---

### US-020: Exception Hierarchy (WayForPayException base)

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a WayForPayException base class with ErrorId and Timestamp
So that all SDK exceptions have consistent structure for logging and debugging

**Acceptance Criteria:**

```gherkin
Scenario: All SDK exceptions inherit from WayForPayException
  Given I catch WayForPayException
  When SDK throws any error
  Then it's caught by this handler

Scenario: ErrorId is unique per exception
  Given an exception is thrown
  When I log ErrorId
  Then I can correlate with support tickets

Scenario: Timestamp records when error occurred
  Given an exception is thrown
  When I check Timestamp
  Then it shows UTC time of the error
```

**Технічні нотатки:**
- `public abstract class WayForPayException : Exception`
- ErrorId: string (Guid.NewGuid().ToString("N"))
- Timestamp: DateTimeOffset.UtcNow
- Конструктори: (message), (message, innerException)

**Залежності:** US-001

**Референси:**
- ADR: ADR-004-error-handling.md

---

### US-021: ApiException Implementation

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want an ApiException with ReasonCode, Reason, OrderReference, IsTransient properties
So that I can handle API errors appropriately and implement retry logic

**Acceptance Criteria:**

```gherkin
Scenario: ApiException contains reason details
  Given WayForPay returns error response
  When ApiException is thrown
  Then I can access ReasonCode and Reason message

Scenario: IsTransient is true for retryable errors
  Given a network timeout occurs
  When I check IsTransient
  Then it returns true

Scenario: IsTransient is false for permanent errors
  Given invalid card number error occurs
  When I check IsTransient
  Then it returns false

Scenario: OrderReference available when applicable
  Given error is for specific order
  When I check OrderReference
  Then it contains the order ID
```

**Технічні нотатки:**
- `public class ApiException : WayForPayException`
- ReasonCode: int
- Reason: string
- OrderReference: string?
- IsTransient: bool (визначається по ReasonCode)
- Transient codes: timeout, rate limiting, server errors

**Залежності:** US-020, US-025

**Референси:**
- ADR: ADR-004-error-handling.md

---

### US-022: SignatureException Implementation

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a SignatureException with ExpectedSignature and ActualSignature properties
So that I can detect and log signature mismatches for debugging

**Acceptance Criteria:**

```gherkin
Scenario: SignatureException thrown on mismatch
  Given response has invalid signature
  When SDK validates signature
  Then SignatureException is thrown

Scenario: Exception contains both signatures
  Given SignatureException is caught
  When I log ExpectedSignature and ActualSignature
  Then I can debug the mismatch

Scenario: Signatures are partially masked in message
  Given exception message is generated
  When I read Message property
  Then signatures are shown with first/last 8 chars for security
```

**Технічні нотатки:**
- `public class SignatureException : WayForPayException`
- ExpectedSignature: string
- ActualSignature: string
- Message should include partial signature info for debugging
- Full signatures available in properties for detailed logging

**Залежності:** US-020

**Референси:**
- ADR: ADR-004-error-handling.md

---

### US-023: ValidationException Implementation

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a ValidationException with IReadOnlyList<ValidationError> property
So that I can display all validation errors to users at once

**Acceptance Criteria:**

```gherkin
Scenario: ValidationException contains all errors
  Given request has multiple invalid fields
  When validation fails
  Then exception contains all ValidationErrors

Scenario: ValidationError has field and message
  Given ValidationError for "Amount" field
  When I access error
  Then I get FieldName="Amount" and Message="Must be positive"

Scenario: Exception message summarizes errors
  Given exception with 3 validation errors
  When I read Message
  Then it lists all field names that failed
```

**Технічні нотатки:**
- `public class ValidationException : WayForPayException`
- Errors: IReadOnlyList<ValidationError>
- `public record ValidationError(string FieldName, string Message)`
- Message format: "Validation failed for fields: Amount, Currency, OrderReference"

**Залежності:** US-020

**Референси:**
- ADR: ADR-004-error-handling.md

---

### US-024: NetworkException Implementation

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a NetworkException with IsTransient, HttpStatusCode, RequestUrl, Elapsed properties
So that I can implement retry logic and diagnose network issues

**Acceptance Criteria:**

```gherkin
Scenario: NetworkException for timeout
  Given HTTP request times out
  When NetworkException is thrown
  Then IsTransient=true and HttpStatusCode=null

Scenario: NetworkException for 500 error
  Given server returns 500
  When NetworkException is thrown
  Then IsTransient=true and HttpStatusCode=500

Scenario: NetworkException for 404 error
  Given server returns 404
  When NetworkException is thrown
  Then IsTransient=false and HttpStatusCode=404

Scenario: Elapsed shows request duration
  Given request took 5 seconds before failing
  When I check Elapsed
  Then it shows TimeSpan of ~5 seconds
```

**Технічні нотатки:**
- `public class NetworkException : WayForPayException`
- IsTransient: bool
- HttpStatusCode: HttpStatusCode?
- RequestUrl: string
- Elapsed: TimeSpan
- Transient: 5xx, timeout, connection refused
- Non-transient: 4xx

**Залежності:** US-020

**Референси:**
- ADR: ADR-004-error-handling.md

---

### Секція 9: JSON Serialization

---

### US-026: JSON Serialization Context (Source Generated)

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want a WayForPayJsonContext with source-generated serialization
So that the SDK is AOT-compatible and has optimal performance

**Acceptance Criteria:**

```gherkin
Scenario: JSON context includes all request types
  Given I need to serialize ChargeRequest
  When I use WayForPayJsonContext.Default.ChargeRequest
  Then serialization works without reflection

Scenario: JSON context includes all response types
  Given I receive ChargeResponse JSON
  When I use WayForPayJsonContext.Default.ChargeResponse
  Then deserialization works without reflection

Scenario: AOT compilation succeeds
  Given I compile SDK with Native AOT
  When serialization is used
  Then no MissingMethodException or similar errors

Scenario: Property naming is camelCase
  Given I have MerchantAccount property
  When serialized to JSON
  Then key is "merchantAccount"
```

**Технічні нотатки:**
- `[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]`
- `[JsonSerializable(typeof(ChargeRequest))]`
- Include all Request and Response types
- Include domain models that appear in API payloads

**Залежності:** US-001, US-003, US-004, US-005, US-006, US-007, US-008, US-009

**Референси:**
- ADR: ADR-005-json-serialization.md

---

### US-027: UnixTimestampConverter for Dates

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a custom JSON converter for Unix timestamps
So that dates are serialized correctly for WayForPay API

**Acceptance Criteria:**

```gherkin
Scenario: DateTimeOffset serializes to Unix timestamp
  Given OrderDate is 2024-01-15 10:30:00 UTC
  When serialized to JSON
  Then output is integer 1705314600

Scenario: Unix timestamp deserializes to DateTimeOffset
  Given JSON has orderDate: 1705314600
  When deserialized
  Then DateTimeOffset is correct UTC time

Scenario: Null dates handled correctly
  Given ProcessingDate is null
  When serialized
  Then JSON has null or omits field
```

**Технічні нотатки:**
- `public class UnixTimestampConverter : JsonConverter<DateTimeOffset>`
- Also nullable version: `UnixTimestampConverter<DateTimeOffset?>`
- Read: `DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64())`
- Write: `writer.WriteNumberValue(value.ToUnixTimeSeconds())`

**Залежності:** US-026

**Референси:**
- PRD: Section 3.2 (orderDate is Unix timestamp)
- ADR: ADR-005-json-serialization.md

---

### US-028: DecimalWithoutTrailingZerosConverter

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a custom JSON converter for decimals without trailing zeros
So that amounts are formatted correctly for WayForPay API (100 instead of 100.00)

**Acceptance Criteria:**

```gherkin
Scenario: Whole number decimal has no decimal point
  Given Amount is 100.00m
  When serialized to JSON
  Then output is 100 (not 100.00)

Scenario: Decimal with significant digits preserved
  Given Amount is 99.99m
  When serialized to JSON
  Then output is 99.99

Scenario: Trailing zeros removed
  Given Amount is 50.50m
  When serialized to JSON
  Then output is 50.5 (not 50.50)
```

**Технічні нотатки:**
- `public class DecimalWithoutTrailingZerosConverter : JsonConverter<decimal>`
- Use `decimal.GetBits` or `ToString("G29")` to remove trailing zeros
- WayForPay API може бути чутливим до формату чисел

**Залежності:** US-026

**Референси:**
- ADR: ADR-005-json-serialization.md

---

## Summary

| Секція | User Stories | Story Points |
|--------|--------------|--------------|
| Project Setup | US-001 | 2 |
| Domain Enums & Constants | US-002, US-025 | 4 |
| Domain Models | US-003 — US-009 | 12 |
| Signature Generation | US-010 — US-012 | 7 |
| Configuration | US-013 — US-014 | 4 |
| HTTP Client | US-015 — US-016 | 5 |
| Dependency Injection | US-017 — US-019 | 7 |
| Exception Hierarchy | US-020 — US-024 | 10 |
| JSON Serialization | US-026 — US-028 | 7 |
| **Total** | **28 User Stories** | **~58 SP** |

---

## Definition of Done (Epic Level)

- [ ] Всі 28 User Stories імплементовані
- [ ] Unit tests з покриттям > 80%
- [ ] XML документація для всіх public API
- [ ] Код пройшов code review
- [ ] Немає compiler warnings
- [ ] Integration з DI працює коректно
