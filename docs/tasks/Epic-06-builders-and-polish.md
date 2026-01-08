# Epic-06: Builders & Polish

## Огляд

Цей Epic охоплює Fluent Builder API для запитів, інтеграцію з Polly для resilience patterns, та фіналізацію документації.

**Ціль:** Покращити Developer Experience та забезпечити production-ready SDK.

## Метадані

| Атрибут | Значення |
|---------|----------|
| **Epic ID** | Epic-06 |
| **User Stories** | US-080 — US-100 (21 stories) |
| **Приблизний обсяг** | ~32 Story Points |
| **Залежності** | Epic-01, Epic-02, Epic-04 |
| **Пріоритет** | High |

## Залежності

```
Epic-01 (Core Infrastructure)
    │
    ├──► Epic-02 (Payment Operations)
    │         │
    │         └──► Epic-06 (Builders & Polish) ◄── YOU ARE HERE
    │
    └──► Epic-04 (Invoice & Forms)
              │
              └──► Epic-06 (Builders & Polish)
```

**Від попередніх Epic-ів потрібні:**
- Всі Request models (ChargeRequest, RefundRequest, InvoiceRequest, PurchaseRequest)
- WayForPayOptions для default values
- IHttpClientBuilder для Polly integration

---

## User Stories

### Секція 1: ChargeRequestBuilder

---

### US-080: ChargeRequestBuilder Create Methods

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want ChargeRequestBuilder.Create() and Create(options) static methods
So that I can start building charge requests fluently

**Acceptance Criteria:**

```gherkin
Scenario: Create builder without options
  Given I want to build ChargeRequest
  When I call ChargeRequestBuilder.Create()
  Then I get new builder instance

Scenario: Create builder with options
  Given I have WayForPayOptions
  When I call ChargeRequestBuilder.Create(options)
  Then builder is pre-configured with MerchantAccount and MerchantDomainName

Scenario: Builder is reusable pattern
  Given I have builder reference
  When I call Build() multiple times with different configurations
  Then each Build() returns new ChargeRequest
```

**Технічні нотатки:**
```csharp
public class ChargeRequestBuilder
{
    public static ChargeRequestBuilder Create() => new();
    public static ChargeRequestBuilder Create(WayForPayOptions options) => new(options);
}
```
- Private constructor with optional options parameter
- Pre-fill MerchantAccount, MerchantDomainName from options

**Залежності:** US-030, US-013

**Референси:**
- PRD: Section 7.2 (Builder Pattern API)
- ADR: ADR-007-builder-pattern-api.md

---

### US-081: ChargeRequestBuilder Order Methods

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want WithOrderReference, WithAmount, WithOrderDate methods
So that I can set order details fluently

**Acceptance Criteria:**

```gherkin
Scenario: Set order reference
  Given I have builder
  When I call WithOrderReference("ORDER-123")
  Then OrderReference is set and builder is returned

Scenario: Set amount with default currency
  Given I have builder
  When I call WithAmount(100.00m)
  Then Amount is 100.00 and Currency defaults to "UAH"

Scenario: Set amount with currency
  Given I have builder
  When I call WithAmount(50.00m, "USD")
  Then Amount is 50.00 and Currency is "USD"

Scenario: OrderDate defaults to now
  Given I don't call WithOrderDate
  When I Build()
  Then OrderDate is current UTC time

Scenario: Custom order date
  Given I call WithOrderDate(specificDate)
  When I Build()
  Then OrderDate is the specified value
```

**Технічні нотатки:**
- `WithOrderReference(string orderRef)` - required
- `WithAmount(decimal amount, string currency = "UAH")`
- `WithOrderDate(DateTimeOffset date)` - optional, defaults to UtcNow
- All methods return `this` for chaining

**Залежності:** US-080

**Референси:**
- ADR: ADR-007-builder-pattern-api.md

---

### US-082: ChargeRequestBuilder Product Methods

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want WithProduct, WithProducts(params), WithProducts(IEnumerable) methods
So that I can add products fluently in various ways

**Acceptance Criteria:**

```gherkin
Scenario: Add single product
  Given I have builder
  When I call WithProduct(new Product { Name = "...", Price = 100, Count = 1 })
  Then product is added to list

Scenario: Add multiple products with params
  Given I have builder
  When I call WithProducts(product1, product2, product3)
  Then all products are added

Scenario: Add products from enumerable
  Given I have IEnumerable<Product>
  When I call WithProducts(products)
  Then all products are added

Scenario: Products accumulate
  Given I call WithProduct twice
  When I Build()
  Then both products are in request

Scenario: Inline product creation
  Given I want simpler syntax
  When I call WithProduct("Product Name", 99.99m, 2)
  Then Product is created and added
```

**Технічні нотатки:**
- `WithProduct(Product product)`
- `WithProduct(string name, decimal price, int count = 1)`
- `WithProducts(params Product[] products)`
- `WithProducts(IEnumerable<Product> products)`
- Internal list accumulates products

**Залежності:** US-080, US-006

**Референси:**
- ADR: ADR-007-builder-pattern-api.md

---

### US-083: ChargeRequestBuilder Payment Methods

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want WithCard and WithRecToken mutually exclusive methods
So that I can set payment method with automatic clearing of the alternative

**Acceptance Criteria:**

```gherkin
Scenario: Set card payment
  Given I have builder
  When I call WithCard(card)
  Then Card is set and RecToken is cleared

Scenario: Set token payment
  Given I have builder
  When I call WithRecToken(token)
  Then RecToken is set and Card is cleared

Scenario: Switching payment method
  Given I called WithCard(card1)
  When I call WithRecToken(token)
  Then only RecToken is set, Card is null

Scenario: Inline card creation
  Given I want simpler syntax
  When I call WithCard("4111111111111111", 12, 2025, "123", "JOHN DOE")
  Then Card is created and set
```

**Технічні нотатки:**
- `WithCard(Card card)` - clears RecToken
- `WithCard(string number, int expMonth, int expYear, string cvv, string holder)`
- `WithRecToken(string token)` - clears Card
- Mutually exclusive by design

**Залежності:** US-080, US-003, US-004

**Референси:**
- ADR: ADR-007-builder-pattern-api.md

---

### US-084: ChargeRequestBuilder Client Method

**Статус:** Draft
**Story Points:** XS (1)

**Опис:**
As a developer
I want WithClient method
So that I can set customer information fluently

**Acceptance Criteria:**

```gherkin
Scenario: Set client info
  Given I have builder and Client object
  When I call WithClient(client)
  Then Client is set on request

Scenario: Inline client creation
  Given I want simpler syntax
  When I call WithClient("John", "Doe", "john@example.com", "+380991234567")
  Then Client is created with those values
```

**Технічні нотатки:**
- `WithClient(Client client)`
- `WithClient(string firstName, string lastName, string email, string phone)`
- Optional method, Client can be null in request

**Залежності:** US-080, US-005

**Референси:**
- ADR: ADR-007-builder-pattern-api.md

---

### US-085: ChargeRequestBuilder Callback Methods

**Статус:** Draft
**Story Points:** XS (1)

**Опис:**
As a developer
I want WithServiceUrl and WithReturnUrl methods
So that I can set callback URLs fluently

**Acceptance Criteria:**

```gherkin
Scenario: Set service URL
  Given I have builder
  When I call WithServiceUrl("https://mysite.com/webhook")
  Then ServiceUrl is set

Scenario: Set return URL
  Given I have builder
  When I call WithReturnUrl("https://mysite.com/success")
  Then ReturnUrl is set

Scenario: Both URLs can be set
  Given I call both methods
  When I Build()
  Then both URLs are in request
```

**Технічні нотатки:**
- `WithServiceUrl(string url)` - for webhook callback
- `WithReturnUrl(string url)` - for redirect after payment
- Optional, can be null

**Залежності:** US-080

**Референси:**
- ADR: ADR-007-builder-pattern-api.md

---

### US-086: ChargeRequestBuilder Transaction Type Methods

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want AsSale, AsAuth, AsAuth(timeout) shortcut methods
So that I can set transaction type fluently

**Acceptance Criteria:**

```gherkin
Scenario: Configure as sale (default)
  Given I call AsSale()
  When I Build()
  Then MerchantTransactionType is Sale

Scenario: Configure as auth
  Given I call AsAuth()
  When I Build()
  Then MerchantTransactionType is Auth

Scenario: Configure auth with timeout
  Given I call AsAuth(3600) // 1 hour
  When I Build()
  Then MerchantTransactionType is Auth and HoldTimeout is 3600

Scenario: Default is Auto
  Given I don't call AsSale or AsAuth
  When I Build()
  Then MerchantTransactionType is Auto
```

**Технічні нотатки:**
- `AsSale()` - sets TransactionType.Sale
- `AsAuth()` - sets TransactionType.Auth
- `AsAuth(int holdTimeoutSeconds)` - sets Auth + HoldTimeout
- Default: TransactionType.Auto

**Залежності:** US-080, US-002

**Референси:**
- PRD: Section 3.2 (merchantTransactionType)
- ADR: ADR-007-builder-pattern-api.md

---

### US-087: ChargeRequestBuilder 3DS Methods

**Статус:** Draft
**Story Points:** XS (1)

**Опис:**
As a developer
I want With3DS and Without3DS shortcut methods
So that I can configure 3D Secure requirement fluently

**Acceptance Criteria:**

```gherkin
Scenario: Force 3DS
  Given I call With3DS()
  When I Build()
  Then MerchantTransactionSecureType is ThreeDs

Scenario: Disable 3DS
  Given I call Without3DS()
  When I Build()
  Then MerchantTransactionSecureType is NonThreeDs

Scenario: Default is Auto
  Given I don't call either method
  When I Build()
  Then MerchantTransactionSecureType is Auto
```

**Технічні нотатки:**
- `With3DS()` - sets SecureType.ThreeDs
- `Without3DS()` - sets SecureType.NonThreeDs
- Default: SecureType.Auto

**Залежності:** US-080, US-002

**Референси:**
- ADR: ADR-007-builder-pattern-api.md

---

### US-088: ChargeRequestBuilder Build with Validation

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want Build method that validates and returns ChargeRequest
So that invalid requests are caught early before API call

**Acceptance Criteria:**

```gherkin
Scenario: Build valid request
  Given all required fields are set
  When I call Build()
  Then ChargeRequest is returned

Scenario: Missing OrderReference throws
  Given OrderReference is not set
  When I call Build()
  Then ValidationException is thrown

Scenario: Missing products throws
  Given no products added
  When I call Build()
  Then ValidationException is thrown

Scenario: Missing payment method throws
  Given neither Card nor RecToken is set
  When I call Build()
  Then ValidationException is thrown

Scenario: Multiple validation errors collected
  Given multiple fields are missing
  When I call Build()
  Then ValidationException contains all errors
```

**Технічні нотатки:**
- `ChargeRequest Build()`
- Validate: OrderReference, Amount > 0, Currency, Products.Count > 0, Card XOR RecToken
- Throw ValidationException with all errors
- Use ValidationError list from US-023

**Залежності:** US-080, US-023

**Референси:**
- ADR: ADR-007-builder-pattern-api.md

---

### Секція 2: Other Builders

---

### US-089: RefundRequestBuilder Implementation

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want a RefundRequestBuilder with fluent methods
So that I can build refund requests easily

**Acceptance Criteria:**

```gherkin
Scenario: Build refund request
  Given I use RefundRequestBuilder
  When I set OrderReference, Amount, Currency
  Then valid RefundRequest is built

Scenario: Optional comment
  Given I call WithComment("Reason")
  When I Build()
  Then Comment is included

Scenario: Partial refund amount
  Given original was 1000
  When I set Amount = 300
  Then partial refund request is created
```

**Технічні нотатки:**
- `RefundRequestBuilder.Create(options?)`
- `ForOrder(string orderReference)`
- `WithAmount(decimal amount, string currency = "UAH")`
- `WithComment(string comment)`
- `Build()` with validation

**Залежності:** US-035

**Референси:**
- ADR: ADR-007-builder-pattern-api.md

---

### US-090: InvoiceRequestBuilder Implementation

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want an InvoiceRequestBuilder with fluent methods
So that I can build invoice requests easily

**Acceptance Criteria:**

```gherkin
Scenario: Build invoice request
  Given I use InvoiceRequestBuilder
  When I set required fields and products
  Then valid InvoiceRequest is built

Scenario: Set customer email
  Given I call ForCustomer("email@example.com")
  When I Build()
  Then ClientEmail is set

Scenario: Set payment options
  Given I call WithPaymentSystems(Card | GooglePay)
  When I Build()
  Then only those systems available

Scenario: Set expiration
  Given I call WithLifetime(TimeSpan.FromHours(24))
  When I Build()
  Then OrderLifetime is 86400 seconds
```

**Технічні нотатки:**
- Similar structure to ChargeRequestBuilder
- `ForCustomer(string email)` - required for invoice
- `WithPaymentSystems(PaymentSystem systems)`
- `WithLifetime(TimeSpan lifetime)`
- `WithLanguage(Language language)`

**Залежності:** US-057

**Референси:**
- ADR: ADR-007-builder-pattern-api.md

---

### US-091: PurchaseFormBuilder Implementation

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want a PurchaseFormBuilder with fluent methods and regular payment support
So that I can build purchase forms easily

**Acceptance Criteria:**

```gherkin
Scenario: Build purchase form
  Given I use PurchaseFormBuilder
  When I set required fields
  Then valid PurchaseRequest is built

Scenario: Set return URL
  Given I call WithReturnUrl("https://...")
  When I BuildForm()
  Then form redirects back after payment

Scenario: Configure regular payments
  Given I call WithRegularPayment(monthly, 100, nextDate)
  When I Build()
  Then RegularPaymentSettings is configured

Scenario: BuildForm returns HTML
  Given I call BuildForm()
  When I render result
  Then complete HTML form is ready
```

**Технічні нотатки:**
- `PurchaseFormBuilder.Create(options?)`
- Similar order/product methods
- `WithRegularPayment(RegularPaymentSettings settings)`
- `Build()` → PurchaseRequest
- `BuildForm()` → PurchaseFormData
- `BuildHtml()` → string HTML

**Залежності:** US-060, US-066, US-063

**Референси:**
- ADR: ADR-007-builder-pattern-api.md

---

### US-092: CheckRequestBuilder Implementation

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a CheckRequestBuilder with fluent methods
So that I can build status check requests easily

**Acceptance Criteria:**

```gherkin
Scenario: Build check request
  Given I use CheckRequestBuilder
  When I call ForOrder("ORDER-123")
  Then valid CheckStatusRequest is built

Scenario: Simple one-liner
  Given I have order reference
  When I call CheckRequestBuilder.Create(options).ForOrder("...").Build()
  Then request is created
```

**Технічні нотатки:**
- Simplest builder (minimal fields)
- `CheckRequestBuilder.Create(options?)`
- `ForOrder(string orderReference)`
- `Build()` → CheckStatusRequest

**Залежності:** US-038

**Референси:**
- ADR: ADR-007-builder-pattern-api.md

---

### Секція 3: Polly Integration

---

### US-093: Polly Retry Policy Configuration Helper

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want pre-configured Polly retry policies for transient errors
So that I can easily add resilience to API calls

**Acceptance Criteria:**

```gherkin
Scenario: Retry on transient HTTP errors
  Given I configure retry policy
  When 500 error occurs
  Then request is retried up to 3 times

Scenario: Exponential backoff
  Given retry policy is active
  When retries happen
  Then delay increases exponentially (1s, 2s, 4s)

Scenario: No retry on 4xx errors
  Given 400 Bad Request occurs
  When policy evaluates
  Then no retry (not transient)

Scenario: Retry respects IsTransient
  Given ApiException with IsTransient=true
  When policy evaluates
  Then retry is attempted
```

**Технічні нотатки:**
```csharp
public static class WayForPayResiliencePolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount = 3);
}
```
- Retry on: HttpRequestException, TaskCanceledException, 5xx responses
- Exponential backoff: 2^attempt seconds
- Jitter for thundering herd prevention

**Залежності:** US-019

**Референси:**
- ADR: ADR-001-http-client-strategy.md
- ADR: ADR-004-error-handling.md

---

### US-094: Polly Circuit Breaker Configuration Helper

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want pre-configured Polly circuit breaker policies
So that I can protect against cascading failures

**Acceptance Criteria:**

```gherkin
Scenario: Circuit opens after failures
  Given 5 consecutive failures
  When circuit breaker evaluates
  Then circuit opens, requests fail fast

Scenario: Circuit half-opens for test
  Given circuit was open for break duration
  When break duration passes
  Then circuit becomes half-open, allows test request

Scenario: Successful test closes circuit
  Given circuit is half-open
  When test request succeeds
  Then circuit closes, normal operation resumes

Scenario: Configuration is customizable
  Given I want different thresholds
  When I create policy with custom settings
  Then my values are used
```

**Технічні нотатки:**
```csharp
public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(
    int failureThreshold = 5,
    TimeSpan breakDuration = default); // default 30s
```
- Break on consecutive failures
- Half-open state for recovery testing
- Reset after success in half-open

**Залежності:** US-019

**Референси:**
- ADR: ADR-001-http-client-strategy.md

---

### US-095: AddWayForPayWithPolly Extension Method

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want an AddWayForPayWithPolly method that adds default resilience policies
So that resilience is easy to enable with one method call

**Acceptance Criteria:**

```gherkin
Scenario: Single method adds all resilience
  Given I call AddWayForPayWithPolly(options)
  When I make API calls
  Then retry and circuit breaker are both active

Scenario: Customization still possible
  Given I want custom policies
  When I chain additional configuration
  Then custom policies are added

Scenario: Default policies are sensible
  Given I use defaults
  When I check configuration
  Then 3 retries, 30s circuit break, exponential backoff
```

**Технічні нотатки:**
```csharp
public static IHttpClientBuilder AddWayForPayWithPolly(
    this IServiceCollection services,
    Action<WayForPayOptions> configure,
    int retryCount = 3,
    int circuitBreakerThreshold = 5)
{
    return services.AddWayForPay(configure)
        .AddPolicyHandler(GetRetryPolicy(retryCount))
        .AddPolicyHandler(GetCircuitBreakerPolicy(circuitBreakerThreshold));
}
```

**Залежності:** US-017, US-093, US-094

**Референси:**
- ADR: ADR-006-dependency-injection.md

---

### US-096: Logging DelegatingHandler

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want an optional logging handler for HTTP requests/responses
So that I can debug API calls during development

**Acceptance Criteria:**

```gherkin
Scenario: Log request details
  Given logging handler is configured
  When API call is made
  Then request URL, method, headers are logged

Scenario: Log response details
  Given logging handler is configured
  When response is received
  Then status code, timing are logged

Scenario: Mask sensitive data
  Given request contains card number
  When logged
  Then card number is masked (411111****1111)

Scenario: Opt-in only
  Given I don't configure logging
  When API calls are made
  Then no request/response logging occurs
```

**Технічні нотатки:**
```csharp
public class LoggingDelegatingHandler : DelegatingHandler
{
    private readonly ILogger<LoggingDelegatingHandler> _logger;
    // ...
}

// Registration
.AddHttpMessageHandler<LoggingDelegatingHandler>()
```
- Mask: card numbers, CVV, secret keys
- Log levels: Debug for requests, Info for responses
- Include timing (elapsed milliseconds)

**Залежності:** US-019

**Референси:**
- ADR: ADR-001-http-client-strategy.md

---

### Секція 4: Documentation

---

### US-097: XML Documentation for All Public APIs

**Статус:** Draft
**Story Points:** L (5)

**Опис:**
As a developer
I want XML documentation on all public types and members
So that IntelliSense shows helpful information

**Acceptance Criteria:**

```gherkin
Scenario: All public classes documented
  Given any public class
  When I hover in IDE
  Then I see summary and remarks

Scenario: All public methods documented
  Given any public method
  When I hover in IDE
  Then I see summary, param descriptions, returns, exceptions

Scenario: All public properties documented
  Given any public property
  When I hover in IDE
  Then I see summary

Scenario: Examples included where helpful
  Given complex API
  When I view documentation
  Then example code is shown

Scenario: No compiler warnings
  Given documentation is complete
  When I build with documentation warnings enabled
  Then no CS1591 warnings
```

**Технічні нотатки:**
- Enable `<GenerateDocumentationFile>true</GenerateDocumentationFile>`
- Enable `<NoWarn>CS1591</NoWarn>` only during development
- Cover: IWayForPayClient, IWebhookHandler, all Builders, all Request/Response types
- Include `<example>` sections for complex APIs

**Залежності:** Всі попередні US

**Референси:**
- PRD: Section 2.2 (TG-04: Documentation)

---

### US-098: README.md with Usage Examples

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want a comprehensive README with installation and usage examples
So that I can get started quickly with the SDK

**Acceptance Criteria:**

```gherkin
Scenario: Installation instructions
  Given I read README
  When I look for installation
  Then I see NuGet command and Package Manager instructions

Scenario: Quick start example
  Given I want to make first payment
  When I follow README
  Then I can complete a charge in under 5 minutes

Scenario: All operations covered
  Given I need any operation
  When I search README
  Then I find example for Charge, Refund, Invoice, Webhook, etc.

Scenario: Configuration examples
  Given I want to configure SDK
  When I read README
  Then I see DI registration and appsettings.json examples
```

**Технічні нотатки:**
- Sections: Installation, Quick Start, Configuration, Operations (Charge, Refund, etc.), Webhooks, Builders, Error Handling, Testing
- Code examples should be copy-pasteable
- Include badges (NuGet version, build status, coverage)

**Залежності:** Всі попередні Epic

**Референси:**
- PRD: Section 8 (Usage Examples)

---

### US-099: API Reference Documentation

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want generated API reference documentation
So that I can explore all available APIs

**Acceptance Criteria:**

```gherkin
Scenario: API docs generated from XML
  Given XML documentation is complete
  When I run doc generator
  Then HTML/MD documentation is created

Scenario: All types indexed
  Given I search documentation
  When I look for any type
  Then I find it with full details

Scenario: Navigation is intuitive
  Given I browse documentation
  When I want to find related types
  Then navigation links are available
```

**Технічні нотатки:**
- Tool options: DocFX, xmldoc2md, or GitHub Wiki
- Generated to `/docs/api/` or hosted separately
- Include in CI/CD pipeline
- Cross-reference with README examples

**Залежності:** US-097

**Референси:**
- PRD: Section 2.2 (Documentation metrics)

---

### US-100: Migration Guide from Direct API Integration

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer with existing WayForPay integration
I want a migration guide for moving from direct API calls to SDK
So that adoption is easier and I can migrate incrementally

**Acceptance Criteria:**

```gherkin
Scenario: Guide covers common patterns
  Given I have direct HttpClient calls
  When I read migration guide
  Then I see how to convert to SDK methods

Scenario: Before/after examples
  Given each operation
  When I view migration guide
  Then I see "Before (raw API)" and "After (SDK)" code

Scenario: Incremental migration path
  Given I can't migrate everything at once
  When I read guide
  Then I understand how to migrate operation by operation

Scenario: Breaking changes noted
  Given SDK has different patterns
  When I read guide
  Then all differences are documented
```

**Технічні нотатки:**
- Document: `/docs/MIGRATION.md`
- Sections: Why Migrate, Preparation, Operation-by-Operation Migration, Testing
- Include common gotchas (signature field order, date formats)
- Reference links to full documentation

**Залежності:** US-098

**Референси:**
- PRD (general migration concerns)

---

## Summary

| Секція | User Stories | Story Points |
|--------|--------------|--------------|
| ChargeRequestBuilder | US-080 — US-088 | 14 |
| Other Builders | US-089 — US-092 | 11 |
| Polly Integration | US-093 — US-096 | 11 |
| Documentation | US-097 — US-100 | 13 |
| **Total** | **21 User Stories** | **~49 SP** |

---

## Definition of Done (Epic Level)

- [ ] Всі 21 User Stories імплементовані
- [ ] ChargeRequestBuilder повністю функціональний
- [ ] Всі інші Builders імплементовані
- [ ] Polly integration працює з retry та circuit breaker
- [ ] XML documentation 100% покриття public API
- [ ] README.md з усіма прикладами
- [ ] API reference documentation згенеровано
- [ ] Migration guide готовий
- [ ] Unit tests для builders
- [ ] Code review пройдено

---

## Builder Usage Example

```csharp
// Complete builder example
var response = await wayForPayClient.ChargeAsync(
    ChargeRequestBuilder.Create(options)
        .WithOrderReference(Guid.NewGuid().ToString())
        .WithAmount(299.99m, "UAH")
        .WithProducts(
            new Product { Name = "Premium Plan", Price = 299.99m, Count = 1 })
        .WithCard("4111111111111111", 12, 2025, "123", "JOHN DOE")
        .WithClient("John", "Doe", "john@example.com", "+380991234567")
        .WithServiceUrl("https://mysite.com/webhook")
        .AsSale()
        .With3DS()
        .Build());

if (response.IsSuccess)
{
    Console.WriteLine($"Payment approved! Auth: {response.Transaction.AuthCode}");
}
else if (response.Requires3Ds)
{
    // Redirect to response.Transaction.D3AcsUrl
}
else
{
    Console.WriteLine($"Payment failed: {response.Reason.Message}");
}
```
