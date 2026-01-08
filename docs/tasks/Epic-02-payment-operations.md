# Epic-02: Payment Operations

## Огляд

Цей Epic охоплює основні Host-to-Host платіжні операції WayForPay API: CHARGE (пряме списання), REFUND (повернення), CHECK STATUS (перевірка статусу) та SETTLE (підтвердження авторизації).

**Ціль:** Реалізувати ядро платіжної функціональності SDK.

## Метадані

| Атрибут | Значення |
|---------|----------|
| **Epic ID** | Epic-02 |
| **User Stories** | US-029 — US-046 (18 stories) |
| **Приблизний обсяг** | ~35 Story Points |
| **Залежності** | Epic-01 (Core Infrastructure) |
| **Пріоритет** | Critical |

## Залежності

```
Epic-01 (Core Infrastructure)
    │
    └──► Epic-02 (Payment Operations) ◄── YOU ARE HERE
             │
             ├──► Epic-03 (3D Secure & Advanced)
             └──► Epic-05 (Webhook Integration)
```

**Від Epic-01 потрібні:**
- Domain models (Card, Product, Client, Transaction, Reason)
- ISignatureGenerator та HmacMd5SignatureGenerator
- WayForPayOptions та DI extensions
- Exception hierarchy
- JSON serialization context

---

## User Stories

### Секція 1: Client Interface

---

### US-029: IWayForPayClient Interface Definition

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want an IWayForPayClient interface with all operation methods
So that I can program against an abstraction and mock the client for testing

**Acceptance Criteria:**

```gherkin
Scenario: Interface can be resolved from DI
  Given I have registered WayForPay services
  When I resolve IWayForPayClient
  Then I get a configured instance

Scenario: Interface includes all Host-to-Host operations
  Given I have IWayForPayClient reference
  When I check available methods
  Then I see ChargeAsync, RefundAsync, CheckStatusAsync, SettleAsync, Complete3DsAsync, GetTransactionsAsync, CreateInvoiceAsync

Scenario: All methods support CancellationToken
  Given I call any async method
  When I pass CancellationToken
  Then the operation can be cancelled

Scenario: Interface can be mocked
  Given I write unit tests for my payment service
  When I create a mock of IWayForPayClient
  Then I can setup expected behavior and verify calls
```

**Технічні нотатки:**
```csharp
public interface IWayForPayClient
{
    Task<ChargeResponse> ChargeAsync(ChargeRequest request, CancellationToken ct = default);
    Task<RefundResponse> RefundAsync(RefundRequest request, CancellationToken ct = default);
    Task<CheckResponse> CheckStatusAsync(CheckRequest request, CancellationToken ct = default);
    Task<SettleResponse> SettleAsync(SettleRequest request, CancellationToken ct = default);
    Task<Complete3DsResponse> Complete3DsAsync(Complete3DsRequest request, CancellationToken ct = default);
    Task<TransactionListResponse> GetTransactionsAsync(TransactionListRequest request, CancellationToken ct = default);
    Task<InvoiceResponse> CreateInvoiceAsync(InvoiceRequest request, CancellationToken ct = default);
}
```

**Залежності:** Epic-01

**Референси:**
- PRD: Section 7.1 (IWayForPayClient Interface)

---

### Секція 2: CHARGE Operation

---

### US-030: ChargeRequest Model

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want a ChargeRequest record with all required and optional fields
So that I can create charge requests with full control over parameters

**Acceptance Criteria:**

```gherkin
Scenario: ChargeRequest requires core fields
  Given I create a ChargeRequest
  When I don't provide MerchantAccount, OrderReference, Amount, Currency, Products
  Then compilation fails with required member errors

Scenario: Either Card or RecToken must be provided
  Given I create a ChargeRequest
  When I don't provide Card or RecToken
  Then runtime validation fails

Scenario: Products is a collection
  Given I create a ChargeRequest
  When I provide Products as IReadOnlyList<Product>
  Then all products are included in the request

Scenario: Optional fields have default values
  Given I create a ChargeRequest without optional fields
  When I check MerchantTransactionType
  Then it defaults to TransactionType.Auto
```

**Технічні нотатки:**
- Required: MerchantAccount, MerchantDomainName, OrderReference, OrderDate, Amount, Currency, Products
- Payment method (one required): Card?, RecToken?
- Optional: Client?, ServiceUrl?, ReturnUrl?, HoldTimeout?, MerchantTransactionType, MerchantTransactionSecureType
- Default values: MerchantAuthType = "SimpleSignature", MerchantTransactionType = Auto, MerchantTransactionSecureType = Auto

**Залежності:** US-003, US-004, US-005, US-006

**Референси:**
- PRD: Section 3.2 (FR-01: CHARGE operation input parameters)
- PRD: Section 6.2 (ChargeRequest model)

---

### US-031: ChargeResponse Model

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want a ChargeResponse record with all response fields
So that I can process charge results including success, failure, and 3DS scenarios

**Acceptance Criteria:**

```gherkin
Scenario: ChargeResponse contains transaction details
  Given I receive a successful charge response
  When I access the response
  Then I can read Transaction with all payment details

Scenario: ChargeResponse has convenience properties
  Given I receive a charge response
  When I check IsSuccess
  Then it returns true only if Reason.IsSuccess

Scenario: 3DS detection via Requires3Ds property
  Given I receive a response requiring 3DS
  When I check Requires3Ds
  Then it returns true and Transaction has D3AcsUrl

Scenario: Response signature is available
  Given I receive a response
  When I access MerchantSignature
  Then I can verify response authenticity
```

**Технічні нотатки:**
- `public sealed record ChargeResponse : IWayForPayResponse`
- MerchantAccount: string
- MerchantSignature: string
- Transaction: Transaction
- Reason: Reason
- IsSuccess => Reason.IsSuccess
- Requires3Ds => Reason.Is3DsRequired && Transaction.D3AcsUrl != null

**Залежності:** US-007, US-008

**Референси:**
- PRD: Section 3.2 (FR-01: CHARGE output parameters)
- PRD: Section 6.3 (ChargeResponse model)

---

### US-032: ChargeAsync Method Implementation

**Статус:** Draft
**Story Points:** L (5)

**Опис:**
As a developer
I want a ChargeAsync method that sends charge requests and returns ChargeResponse
So that I can process direct card payments through WayForPay

**Acceptance Criteria:**

```gherkin
Scenario: Successful charge returns Approved status
  Given I have valid card data and sufficient funds
  When I call ChargeAsync
  Then response.IsSuccess is true and Transaction.TransactionStatus is "Approved"

Scenario: Failed charge returns error reason
  Given I have invalid card data
  When I call ChargeAsync
  Then response.IsSuccess is false and Reason contains error details

Scenario: Charge requiring 3DS returns redirect data
  Given I have card requiring 3DS
  When I call ChargeAsync
  Then response.Requires3Ds is true and Transaction contains AcsUrl, Md, Pareq

Scenario: Charge with RecToken succeeds
  Given I have valid recToken from previous payment
  When I call ChargeAsync with RecToken instead of Card
  Then payment is processed successfully

Scenario: Network error throws NetworkException
  Given WayForPay API is unavailable
  When I call ChargeAsync
  Then NetworkException is thrown with IsTransient=true

Scenario: Invalid signature throws SignatureException
  Given response has tampered signature
  When I call ChargeAsync
  Then SignatureException is thrown
```

**Технічні нотатки:**
- Endpoint: POST https://api.wayforpay.com/api
- Request body: JSON with transactionType: "CHARGE"
- Sign request with HMAC-MD5
- Validate response signature
- Parse response JSON to ChargeResponse
- Map error responses to ApiException

**Залежності:** US-029, US-030, US-031, US-033, US-034, US-044

**Референси:**
- PRD: Section 3.2 (FR-01: CHARGE operation)
- PRD: Section 8.2 (Usage example)

---

### US-033: Charge Request Signature Generation

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want charge requests to be automatically signed with the correct field order
So that WayForPay accepts the requests without signature errors

**Acceptance Criteria:**

```gherkin
Scenario: Signature includes all required fields in correct order
  Given I have a ChargeRequest
  When signature is generated
  Then fields are in order: merchantAccount, merchantDomainName, orderReference, orderDate, amount, currency, productName[], productCount[], productPrice[]

Scenario: Product arrays are expanded correctly
  Given I have 2 products
  When signature is generated
  Then productName, productCount, productPrice are repeated for each product

Scenario: Signature is added to request body
  Given I send a charge request
  When HTTP request is made
  Then JSON contains merchantSignature field

Scenario: Empty optional fields are excluded
  Given I have ChargeRequest without ServiceUrl
  When signature is generated
  Then ServiceUrl is not included in signature string
```

**Технічні нотатки:**
- Порядок полів для CHARGE signature (з документації WayForPay):
  1. merchantAccount
  2. merchantDomainName
  3. orderReference
  4. orderDate
  5. amount
  6. currency
  7. productName[] (кожен елемент)
  8. productCount[] (кожен елемент)
  9. productPrice[] (кожен елемент)
- Продукти розгортаються: ["Prod1", "Prod2"], [1, 2], [100, 200]

**Залежності:** US-010, US-011, US-030

**Референси:**
- PRD: Section 4.2 (HMAC-MD5 Signature)
- ADR: ADR-002-signature-generation.md

---

### US-034: Charge Response Signature Validation

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want charge responses to have signature validated automatically
So that I can trust the response authenticity and detect tampering

**Acceptance Criteria:**

```gherkin
Scenario: Valid signature passes silently
  Given WayForPay returns response with valid signature
  When ChargeAsync processes response
  Then no exception is thrown

Scenario: Invalid signature throws SignatureException
  Given response signature is tampered
  When ChargeAsync processes response
  Then SignatureException is thrown with expected and actual signatures

Scenario: Response signature uses correct field order
  Given I need to verify response signature
  When I check signature calculation
  Then fields are: merchantAccount, orderReference, amount, currency, authCode, cardPan, transactionStatus, reasonCode

Scenario: Validation happens before returning response
  Given response has invalid signature
  When ChargeAsync completes
  Then I never receive the ChargeResponse object (exception thrown first)
```

**Технічні нотатки:**
- Response signature field order:
  1. merchantAccount
  2. orderReference
  3. amount
  4. currency
  5. authCode
  6. cardPan
  7. transactionStatus
  8. reasonCode
- Використовувати timing-safe comparison (US-012)

**Залежності:** US-010, US-012, US-031

**Референси:**
- ADR: ADR-002-signature-generation.md

---

### Секція 3: REFUND Operation

---

### US-035: RefundRequest Model

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a RefundRequest record (MerchantAccount, OrderReference, Amount, Currency, Comment)
So that I can create refund requests for full or partial returns

**Acceptance Criteria:**

```gherkin
Scenario: RefundRequest requires core fields
  Given I create a RefundRequest
  When I don't provide MerchantAccount, OrderReference, Amount, Currency
  Then compilation fails

Scenario: Comment is optional
  Given I create a RefundRequest without Comment
  When request is serialized
  Then Comment field is omitted

Scenario: Partial refund with smaller amount
  Given original payment was 1000 UAH
  When I create RefundRequest with Amount = 500
  Then partial refund request is valid
```

**Технічні нотатки:**
- Required: MerchantAccount, OrderReference, Amount, Currency
- Optional: Comment (reason for refund)
- transactionType: "REFUND"

**Залежності:** Epic-01

**Референси:**
- PRD: Section 3.3 (FR-02: REFUND operation)

---

### US-036: RefundResponse Model

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a RefundResponse record (TransactionStatus, ReasonCode, Reason)
So that I can process refund operation results

**Acceptance Criteria:**

```gherkin
Scenario: RefundResponse indicates success
  Given refund was successful
  When I check response
  Then TransactionStatus is "Refunded" and ReasonCode is 1100

Scenario: RefundResponse indicates failure
  Given refund failed
  When I check response
  Then TransactionStatus is "Declined" and Reason contains error message

Scenario: RefundResponse has IsSuccess property
  Given I receive RefundResponse
  When I check IsSuccess
  Then it returns true only if ReasonCode is 1100
```

**Технічні нотатки:**
- MerchantAccount: string
- OrderReference: string
- TransactionStatus: string (Refunded, Declined)
- ReasonCode: int
- Reason: string

**Залежності:** US-008

**Референси:**
- PRD: Section 3.3 (FR-02: REFUND output parameters)

---

### US-037: RefundAsync Method Implementation

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want a RefundAsync method that sends refund requests
So that I can process full or partial refunds for completed payments

**Acceptance Criteria:**

```gherkin
Scenario: Full refund succeeds
  Given I have completed payment for 1000 UAH
  When I call RefundAsync with Amount = 1000
  Then refund is processed and response.IsSuccess is true

Scenario: Partial refund succeeds
  Given I have completed payment for 1000 UAH
  When I call RefundAsync with Amount = 300
  Then partial refund is processed successfully

Scenario: Refund for non-existent order fails
  Given OrderReference doesn't exist
  When I call RefundAsync
  Then ApiException is thrown with appropriate error code

Scenario: Refund exceeding original amount fails
  Given original payment was 1000 UAH
  When I call RefundAsync with Amount = 1500
  Then ApiException is thrown
```

**Технічні нотатки:**
- Endpoint: POST https://api.wayforpay.com/api
- transactionType: "REFUND"
- Signature fields: merchantAccount, orderReference, amount, currency

**Залежності:** US-029, US-035, US-036, US-044

**Референси:**
- PRD: Section 3.3 (FR-02: REFUND operation)
- PRD: Section 8.4 (Refund example)

---

### Секція 4: CHECK STATUS Operation

---

### US-038: CheckStatusRequest Model

**Статус:** Draft
**Story Points:** XS (1)

**Опис:**
As a developer
I want a CheckStatusRequest record (MerchantAccount, OrderReference)
So that I can create requests to check transaction status

**Acceptance Criteria:**

```gherkin
Scenario: CheckStatusRequest requires both fields
  Given I create a CheckStatusRequest
  When I don't provide MerchantAccount or OrderReference
  Then compilation fails

Scenario: Request is minimal
  Given I create CheckStatusRequest
  When I check required fields
  Then only MerchantAccount and OrderReference are required
```

**Технічні нотатки:**
- Required: MerchantAccount, OrderReference
- transactionType: "CHECK_STATUS"

**Залежності:** Epic-01

**Референси:**
- PRD: Section 3.4 (FR-03: CHECK operation)

---

### US-039: CheckStatusResponse Model

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a CheckStatusResponse record with full transaction details
So that I can get current order status and all payment information

**Acceptance Criteria:**

```gherkin
Scenario: Response contains full transaction
  Given order exists
  When I call CheckStatus
  Then response contains Transaction with all fields

Scenario: Response contains reason
  Given order exists
  When I check response
  Then Reason shows current status

Scenario: Not found order returns error
  Given order doesn't exist
  When I call CheckStatus
  Then ApiException with OrderNotFound error
```

**Технічні нотатки:**
- Повна інформація про транзакцію (аналогічно ChargeResponse)
- Transaction: Transaction (full details)
- Reason: Reason

**Залежності:** US-007, US-008

**Референси:**
- PRD: Section 3.4 (FR-03: CHECK output)

---

### US-040: CheckStatusAsync Method Implementation

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want a CheckStatusAsync method that queries transaction status
So that I can verify payment completion and get current transaction state

**Acceptance Criteria:**

```gherkin
Scenario: Check approved transaction
  Given payment was approved
  When I call CheckStatusAsync
  Then Transaction.TransactionStatus is "Approved"

Scenario: Check pending transaction
  Given payment is still processing
  When I call CheckStatusAsync
  Then Transaction.TransactionStatus is "Pending" or "InProcessing"

Scenario: Check refunded transaction
  Given payment was refunded
  When I call CheckStatusAsync
  Then Transaction.TransactionStatus is "Refunded"

Scenario: Check non-existent order
  Given order doesn't exist
  When I call CheckStatusAsync
  Then ApiException is thrown
```

**Технічні нотатки:**
- Endpoint: POST https://api.wayforpay.com/api
- transactionType: "CHECK_STATUS"
- Signature fields: merchantAccount, orderReference

**Залежності:** US-029, US-038, US-039, US-044

**Референси:**
- PRD: Section 3.4 (FR-03: CHECK operation)

---

### Секція 5: SETTLE Operation

---

### US-041: SettleRequest Model

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a SettleRequest record (MerchantAccount, OrderReference, Amount, Currency)
So that I can create requests to confirm AUTH transactions

**Acceptance Criteria:**

```gherkin
Scenario: SettleRequest requires all fields
  Given I create a SettleRequest
  When I don't provide any required field
  Then compilation fails

Scenario: Settle amount can be less than AUTH
  Given AUTH was for 1000 UAH
  When I create SettleRequest with Amount = 800
  Then request is valid (partial capture)

Scenario: Settle amount cannot exceed AUTH
  Given AUTH was for 1000 UAH
  When I create SettleRequest with Amount = 1200
  Then API will reject the request
```

**Технічні нотатки:**
- Required: MerchantAccount, OrderReference, Amount, Currency
- Amount: може бути <= AUTH суми
- transactionType: "SETTLE"

**Залежності:** Epic-01

**Референси:**
- PRD: Section 3.5 (FR-04: SETTLE operation)

---

### US-042: SettleResponse Model

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a SettleResponse record (TransactionStatus, ReasonCode, Reason)
So that I can process settle operation results

**Acceptance Criteria:**

```gherkin
Scenario: Successful settle returns Approved
  Given AUTH transaction exists
  When settle succeeds
  Then TransactionStatus is "Approved"

Scenario: Failed settle returns Declined
  Given settle fails
  When I check response
  Then TransactionStatus is "Declined" with error Reason
```

**Технічні нотатки:**
- TransactionStatus: string (Approved, Declined)
- ReasonCode: int
- Reason: string

**Залежності:** US-008

**Референси:**
- PRD: Section 3.5 (FR-04: SETTLE output)

---

### US-043: SettleAsync Method Implementation

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want a SettleAsync method that confirms AUTH transactions
So that I can complete two-step payments (authorize then capture)

**Acceptance Criteria:**

```gherkin
Scenario: Settle full AUTH amount
  Given AUTH for 1000 UAH exists
  When I call SettleAsync with Amount = 1000
  Then payment is captured and IsSuccess is true

Scenario: Partial capture
  Given AUTH for 1000 UAH exists
  When I call SettleAsync with Amount = 700
  Then 700 UAH is captured, 300 UAH is released

Scenario: Settle already captured transaction fails
  Given transaction is already settled
  When I call SettleAsync
  Then ApiException is thrown

Scenario: Settle expired AUTH fails
  Given AUTH has expired (holdTimeout passed)
  When I call SettleAsync
  Then ApiException is thrown
```

**Технічні нотатки:**
- Endpoint: POST https://api.wayforpay.com/api
- transactionType: "SETTLE"
- Signature fields: merchantAccount, orderReference, amount, currency
- Two-step flow: ChargeAsync with AsAuth() → SettleAsync

**Залежності:** US-029, US-041, US-042, US-044

**Референси:**
- PRD: Section 3.5 (FR-04: SETTLE operation)

---

### Секція 6: Client Implementation

---

### US-044: WayForPayClient Implementation

**Статус:** Draft
**Story Points:** L (5)

**Опис:**
As a developer
I want a WayForPayClient class implementing IWayForPayClient
So that I can perform all payment operations through a single configured client

**Acceptance Criteria:**

```gherkin
Scenario: Client is registered as typed HttpClient
  Given I call AddWayForPay
  When I resolve IWayForPayClient
  Then I get WayForPayClient with configured HttpClient

Scenario: Client uses options for merchant credentials
  Given WayForPayOptions has MerchantAccount and SecretKey
  When client makes request
  Then credentials from options are used

Scenario: Client is thread-safe
  Given multiple concurrent requests
  When I call multiple methods simultaneously
  Then all requests are processed correctly

Scenario: Client handles all transactionTypes
  Given I need different operations
  When I call ChargeAsync, RefundAsync, etc.
  Then each sends correct transactionType in request body
```

**Технічні нотатки:**
```csharp
public class WayForPayClient : IWayForPayClient
{
    private readonly HttpClient _httpClient;
    private readonly ISignatureGenerator _signatureGenerator;
    private readonly IOptions<WayForPayOptions> _options;

    // Constructor injection
    // Implementation of all interface methods
}
```
- Inject HttpClient, ISignatureGenerator, IOptions<WayForPayOptions>
- Base implementation pattern shared across methods
- JSON serialization using WayForPayJsonContext

**Залежності:** US-029, US-015, US-010, US-013, US-026

**Референси:**
- PRD: Section 5.2 (Component diagram)

---

### Секція 7: Error Handling

---

### US-045: API Error to Exception Mapping

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want API error responses mapped to ApiException with proper ReasonCode
So that I can handle errors consistently and programmatically

**Acceptance Criteria:**

```gherkin
Scenario: Error response throws ApiException
  Given API returns reasonCode != 1100
  When client processes response
  Then ApiException is thrown with ReasonCode and Reason

Scenario: ApiException contains order reference
  Given error is for specific order
  When I catch ApiException
  Then OrderReference property has the value

Scenario: Different error codes are distinguishable
  Given insufficient funds error (1104)
  When I catch ApiException
  Then ReasonCode is 1104 and I can handle specifically

Scenario: Error message is human-readable
  Given ApiException is thrown
  When I read exception.Message
  Then it contains both code and description
```

**Технічні нотатки:**
- Check reasonCode in response
- If reasonCode != 1100, throw ApiException
- Include: ReasonCode, Reason, OrderReference (if available)
- Set IsTransient based on code (e.g., rate limits are transient)

**Залежності:** US-021, US-025

**Референси:**
- PRD: Section 3.2 (Reason Codes table)
- ADR: ADR-004-error-handling.md

---

### US-046: Transient Error Detection for Retry

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want transient errors identified by IsTransient property
So that retry policies (Polly) work correctly

**Acceptance Criteria:**

```gherkin
Scenario: Timeout is transient
  Given HTTP request times out
  When NetworkException is thrown
  Then IsTransient is true

Scenario: 500 Server Error is transient
  Given API returns 500
  When NetworkException is thrown
  Then IsTransient is true

Scenario: Invalid card data is not transient
  Given card number is invalid
  When ApiException is thrown
  Then IsTransient is false (retry won't help)

Scenario: Rate limit is transient
  Given API returns rate limit error
  When ApiException is thrown
  Then IsTransient is true (retry after delay)

Scenario: Polly retry policy uses IsTransient
  Given I configure retry policy
  When I check ShouldRetry condition
  Then it uses exception.IsTransient property
```

**Технічні нотатки:**
- Transient HttpStatusCodes: 408, 429, 500, 502, 503, 504
- Transient exceptions: HttpRequestException, TaskCanceledException (timeout)
- Non-transient API errors: validation, invalid signature, insufficient funds
- Interface: `bool IsTransient { get; }` on exception classes

**Залежності:** US-021, US-024

**Референси:**
- ADR: ADR-004-error-handling.md
- ADR: ADR-001-http-client-strategy.md

---

## Summary

| Секція | User Stories | Story Points |
|--------|--------------|--------------|
| Client Interface | US-029 | 3 |
| CHARGE Operation | US-030 — US-034 | 17 |
| REFUND Operation | US-035 — US-037 | 7 |
| CHECK STATUS Operation | US-038 — US-040 | 6 |
| SETTLE Operation | US-041 — US-043 | 7 |
| Client Implementation | US-044 | 5 |
| Error Handling | US-045 — US-046 | 5 |
| **Total** | **18 User Stories** | **~50 SP** |

---

## Definition of Done (Epic Level)

- [ ] Всі 18 User Stories імплементовані
- [ ] IWayForPayClient та WayForPayClient повністю функціональні
- [ ] Unit tests для кожної операції
- [ ] Integration tests з мок-сервером
- [ ] Signature generation/validation працює коректно
- [ ] Error handling покриває всі сценарії
- [ ] XML документація для public API
- [ ] Код пройшов code review

---

## Test Scenarios (Sandbox)

| Карта | Результат | Використання |
|-------|-----------|--------------|
| 4111111111111111 | Approved | Happy path |
| 4111111111111112 | Declined (Insufficient funds) | Error handling |
| 4111111111111113 | 3DS Required | 3DS flow (Epic-03) |
| 5555555555554444 | Approved (MasterCard) | Card type variety |
