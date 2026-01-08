# Epic-05: Webhook Integration

## Огляд

Цей Epic охоплює обробку webhook callbacks від WayForPay — парсинг, валідацію підписів, генерацію відповідей та інтеграцію з ASP.NET Core.

**Ціль:** Надати надійний механізм обробки асинхронних сповіщень про статуси платежів.

## Метадані

| Атрибут | Значення |
|---------|----------|
| **Epic ID** | Epic-05 |
| **User Stories** | US-067 — US-079 (13 stories) |
| **Приблизний обсяг** | ~22 Story Points |
| **Залежності** | Epic-01 |
| **Пріоритет** | Critical |

## Залежності

```
Epic-01 (Core Infrastructure)
    │
    ├──► Epic-02 (Payment Operations)
    │
    └──► Epic-05 (Webhook Integration) ◄── YOU ARE HERE
```

**Від Epic-01 потрібні:**
- ISignatureGenerator для валідації та генерації підписів
- Domain models (Transaction)
- JSON serialization context
- Exception classes (SignatureException)

---

## User Stories

### Секція 1: Core Interface

---

### US-067: IWebhookHandler Interface

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want an IWebhookHandler interface with ParseAsync, Parse, CreateResponse, SerializeResponse methods
So that webhook handling is abstracted and testable

**Acceptance Criteria:**

```gherkin
Scenario: Interface can be resolved from DI
  Given I have registered WayForPay services
  When I resolve IWebhookHandler
  Then I get configured instance

Scenario: Interface can be mocked for testing
  Given I write unit tests for my controller
  When I mock IWebhookHandler
  Then I can verify webhook handling behavior

Scenario: Interface provides both sync and async parsing
  Given I have webhook body as Stream or string
  When I call ParseAsync or Parse
  Then webhook is parsed appropriately
```

**Технічні нотатки:**
```csharp
public interface IWebhookHandler
{
    Task<WebhookPayload> ParseAsync(Stream body, CancellationToken ct = default);
    WebhookPayload Parse(string json);
    WebhookResponse CreateResponse(WebhookPayload payload, WebhookStatus status = WebhookStatus.Accept);
    string SerializeResponse(WebhookResponse response);
}
```

**Залежності:** Epic-01

**Референси:**
- PRD: Section 7.3 (Webhook Handler API)
- ADR: ADR-009-webhook-handler-design.md

---

### Секція 2: Webhook Models

---

### US-068: WebhookPayload Model

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want a WebhookPayload record with all callback fields
So that I can access all payment information from the webhook

**Acceptance Criteria:**

```gherkin
Scenario: Payload contains order information
  Given webhook is parsed
  When I access payload
  Then I can read MerchantAccount, OrderReference, Amount, Currency

Scenario: Payload contains transaction result
  Given webhook is parsed
  When I check TransactionStatus, ReasonCode
  Then I know if payment succeeded

Scenario: Payload contains card info
  Given payment was made with card
  When I check payload
  Then CardPan has masked card number

Scenario: Payload contains signature for validation
  Given webhook is received
  When I access MerchantSignature
  Then I can validate authenticity
```

**Технічні нотатки:**
- MerchantAccount: string
- OrderReference: string
- MerchantSignature: string
- Amount: decimal
- Currency: string
- AuthCode: string?
- CardPan: string?
- TransactionStatus: string
- ReasonCode: int
- Reason: string
- Fee: decimal?
- PaymentSystem: string?
- RecToken: string? (for recurring)
- Email: string?
- Phone: string?
- CreatedDate: DateTimeOffset?
- ProcessingDate: DateTimeOffset?

**Залежності:** Epic-01

**Референси:**
- PRD: Section 3.11 (FR-10: Webhook callback structure)

---

### US-069: WebhookResponse Model

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a WebhookResponse record (OrderReference, Status, Time, Signature)
So that I can respond to WayForPay correctly

**Acceptance Criteria:**

```gherkin
Scenario: Response requires all fields
  Given I create WebhookResponse
  When response is serialized
  Then it contains orderReference, status, time, signature

Scenario: Signature is auto-generated
  Given I use CreateResponse helper
  When response is created
  Then signature is already calculated

Scenario: Time is Unix timestamp
  Given response is created
  When serialized
  Then time is integer (Unix seconds)
```

**Технічні нотатки:**
- OrderReference: string
- Status: string ("accept" or "decline")
- Time: long (Unix timestamp)
- Signature: string (HMAC-MD5)

**Залежності:** Epic-01

**Референси:**
- PRD: Section 3.11 (Webhook response format)

---

### US-070: WebhookStatus Enum

**Статус:** Draft
**Story Points:** XS (1)

**Опис:**
As a developer
I want a WebhookStatus enum (Accept, Decline)
So that I can specify response status type-safely

**Acceptance Criteria:**

```gherkin
Scenario: Accept status confirms receipt
  Given I want to acknowledge webhook
  When I use WebhookStatus.Accept
  Then response status is "accept"

Scenario: Decline status rejects webhook
  Given I want to reject (e.g., order not found)
  When I use WebhookStatus.Decline
  Then response status is "decline"
```

**Технічні нотатки:**
- `public enum WebhookStatus { Accept, Decline }`
- Accept serializes to "accept" (lowercase)
- Decline serializes to "decline" (lowercase)

**Залежності:** US-001

**Референси:**
- PRD: Section 3.11 (Response status values)

---

### Секція 3: Parsing & Validation

---

### US-071: WebhookHandler ParseAsync (Stream)

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want a ParseAsync method that parses webhook from Stream
So that I can handle webhooks directly from HTTP request body

**Acceptance Criteria:**

```gherkin
Scenario: Parse valid webhook from stream
  Given HTTP request body as Stream
  When I call ParseAsync(stream)
  Then WebhookPayload is returned with all fields

Scenario: Invalid JSON throws exception
  Given malformed JSON in stream
  When I call ParseAsync
  Then JsonException or similar is thrown

Scenario: Stream is read asynchronously
  Given large request body
  When I call ParseAsync
  Then reading is non-blocking

Scenario: CancellationToken is respected
  Given long-running parse
  When cancellation is requested
  Then OperationCanceledException is thrown
```

**Технічні нотатки:**
- `Task<WebhookPayload> ParseAsync(Stream body, CancellationToken ct = default)`
- Use `JsonSerializer.DeserializeAsync<WebhookPayload>(body, ctx, ct)`
- After parsing, validate signature automatically
- Throw SignatureException if validation fails

**Залежності:** US-067, US-068, US-073

**Референси:**
- ADR: ADR-009-webhook-handler-design.md

---

### US-072: WebhookHandler Parse (string)

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a Parse method that parses webhook from JSON string
So that I can handle webhooks when body is already read

**Acceptance Criteria:**

```gherkin
Scenario: Parse valid JSON string
  Given webhook JSON as string
  When I call Parse(json)
  Then WebhookPayload is returned

Scenario: Null or empty string throws
  Given null or empty string
  When I call Parse
  Then ArgumentException is thrown

Scenario: Synchronous operation
  Given I already have string body
  When I call Parse
  Then it completes synchronously
```

**Технічні нотатки:**
- `WebhookPayload Parse(string json)`
- Use `JsonSerializer.Deserialize<WebhookPayload>(json, ctx)`
- Validate signature after parsing
- Throw SignatureException if validation fails

**Залежності:** US-067, US-068, US-073

**Референси:**
- ADR: ADR-009-webhook-handler-design.md

---

### US-073: Webhook Signature Validation

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want webhook payload signature validated automatically
So that I only process authentic callbacks from WayForPay

**Acceptance Criteria:**

```gherkin
Scenario: Valid signature passes silently
  Given webhook from WayForPay with correct signature
  When parsed
  Then no exception thrown, payload returned

Scenario: Invalid signature throws SignatureException
  Given tampered webhook or wrong secret
  When parsed
  Then SignatureException is thrown with details

Scenario: Missing signature throws
  Given webhook without merchantSignature field
  When parsed
  Then SignatureException or ValidationException thrown

Scenario: Signature uses correct field order
  Given I need to verify webhook signature
  When checking calculation
  Then fields are: merchantAccount, orderReference, amount, currency, authCode, cardPan, transactionStatus, reasonCode
```

**Технічні нотатки:**
- Webhook signature field order:
  1. merchantAccount
  2. orderReference
  3. amount
  4. currency
  5. authCode
  6. cardPan
  7. transactionStatus
  8. reasonCode
- Use timing-safe comparison (US-012)
- Secret key from WayForPayOptions

**Залежності:** US-010, US-012, US-068

**Референси:**
- ADR: ADR-002-signature-generation.md
- ADR: ADR-009-webhook-handler-design.md

---

### Секція 4: Response Generation

---

### US-074: WebhookHandler CreateResponse

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want a CreateResponse method that creates signed response
So that WayForPay accepts my acknowledgment

**Acceptance Criteria:**

```gherkin
Scenario: Create accept response
  Given I processed webhook successfully
  When I call CreateResponse(payload, Accept)
  Then response has status="accept" and valid signature

Scenario: Create decline response
  Given I want to reject (e.g., duplicate)
  When I call CreateResponse(payload, Decline)
  Then response has status="decline" and valid signature

Scenario: Response time is current UTC
  Given I create response
  When I check Time field
  Then it's current Unix timestamp

Scenario: Default status is Accept
  Given I call CreateResponse(payload)
  When status parameter omitted
  Then response status is "accept"
```

**Технічні нотатки:**
- `WebhookResponse CreateResponse(WebhookPayload payload, WebhookStatus status = Accept)`
- Response signature fields: orderReference, status, time
- Time: DateTimeOffset.UtcNow.ToUnixTimeSeconds()

**Залежності:** US-067, US-069, US-070, US-010

**Референси:**
- PRD: Section 3.11 (Response on callback)
- ADR: ADR-009-webhook-handler-design.md

---

### US-075: WebhookHandler SerializeResponse

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a SerializeResponse method that returns JSON string
So that I can return proper response body

**Acceptance Criteria:**

```gherkin
Scenario: Serialize response to JSON
  Given I have WebhookResponse
  When I call SerializeResponse
  Then valid JSON string is returned

Scenario: JSON has correct format
  Given serialized response
  When I parse JSON
  Then it has orderReference, status, time, signature fields

Scenario: JSON uses camelCase
  Given property OrderReference
  When serialized
  Then JSON key is "orderReference"
```

**Технічні нотатки:**
- `string SerializeResponse(WebhookResponse response)`
- Use source-generated JSON context
- Return format: `{"orderReference":"...","status":"accept","time":1234567890,"signature":"..."}`

**Залежності:** US-067, US-069, US-026

**Референси:**
- ADR: ADR-009-webhook-handler-design.md

---

### Секція 5: ASP.NET Core Extensions

---

### US-076: ASP.NET Core Extension: ParseAsync(HttpRequest)

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As an ASP.NET Core developer
I want an extension method to parse webhook from HttpRequest
So that I can easily handle webhooks in controllers

**Acceptance Criteria:**

```gherkin
Scenario: Parse from HttpRequest
  Given ASP.NET Core controller action
  When I call handler.ParseAsync(Request)
  Then webhook is parsed from request body

Scenario: Works with minimal API
  Given minimal API endpoint
  When I call handler.ParseAsync(context.Request)
  Then webhook is parsed correctly

Scenario: Request body seekability handled
  Given request body may not be seekable
  When parsing
  Then EnableBuffering is called if needed
```

**Технічні нотатки:**
- Extension method in separate assembly or #if conditional
- `Task<WebhookPayload> ParseAsync(this IWebhookHandler handler, HttpRequest request, CancellationToken ct = default)`
- May need `request.EnableBuffering()` for rereadability
- Package: WayForPaySDK.AspNetCore or same package with conditional reference

**Залежності:** US-067, US-071

**Референси:**
- ADR: ADR-009-webhook-handler-design.md (ASP.NET Core integration)

---

### US-077: ASP.NET Core Extension: ToActionResult

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As an ASP.NET Core developer
I want a ToActionResult extension for WebhookResponse
So that I can return proper IActionResult from controllers

**Acceptance Criteria:**

```gherkin
Scenario: Convert to ContentResult
  Given I have WebhookResponse
  When I call response.ToActionResult()
  Then ContentResult with JSON is returned

Scenario: Content-Type is correct
  Given I return ToActionResult()
  When checking response headers
  Then Content-Type is "application/json"

Scenario: Status code is 200
  Given successful webhook handling
  When I return ToActionResult()
  Then HTTP status is 200 OK
```

**Технічні нотатки:**
- `IActionResult ToActionResult(this WebhookResponse response)`
- Return ContentResult with Content = JSON, ContentType = "application/json"
- Alternative: JsonResult with camelCase settings

**Залежності:** US-069, US-075

**Референси:**
- PRD: Section 8.5 (Webhook handling example)

---

### US-078: ASP.NET Core Extension: HandleAsync

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As an ASP.NET Core developer
I want a HandleAsync extension that combines parse, process, and respond
So that webhook handling is simplified to minimal code

**Acceptance Criteria:**

```gherkin
Scenario: One-liner webhook handling
  Given I have handler delegate
  When I call HandleAsync(Request, async payload => { ... })
  Then parsing, processing, and response creation are automatic

Scenario: Exception in handler returns decline
  Given handler throws exception
  When HandleAsync runs
  Then decline response is returned

Scenario: Success returns accept
  Given handler completes without exception
  When HandleAsync runs
  Then accept response is returned

Scenario: Custom status from handler
  Given handler returns WebhookStatus
  When HandleAsync runs
  Then that status is used in response
```

**Технічні нотатки:**
```csharp
Task<IActionResult> HandleAsync(
    this IWebhookHandler handler,
    HttpRequest request,
    Func<WebhookPayload, Task<WebhookStatus>> processor,
    CancellationToken ct = default)
```
- Or simpler: `Func<WebhookPayload, Task>` returning Accept on success
- Wrap in try/catch, return Decline on exception

**Залежності:** US-076, US-074, US-077

**Референси:**
- ADR: ADR-009-webhook-handler-design.md

---

### Секція 6: Helper Properties

---

### US-079: WebhookPayload Helper Properties

**Статус:** Draft
**Story Points:** XS (1)

**Опис:**
As a developer
I want helper properties on WebhookPayload (IsSuccess, IsApproved)
So that I can quickly check payment status without comparing strings

**Acceptance Criteria:**

```gherkin
Scenario: IsSuccess returns true for code 1100
  Given ReasonCode is 1100
  When I check IsSuccess
  Then it returns true

Scenario: IsApproved checks transaction status
  Given TransactionStatus is "Approved"
  When I check IsApproved
  Then it returns true

Scenario: Both properties combined
  Given I want to check full success
  When I check IsSuccess && IsApproved
  Then I know payment is fully complete

Scenario: Failed payment properties
  Given TransactionStatus is "Declined"
  When I check IsApproved
  Then it returns false
```

**Технічні нотатки:**
```csharp
public bool IsSuccess => ReasonCode == ReasonCodes.Ok; // 1100
public bool IsApproved => TransactionStatus == "Approved";
public bool IsDeclined => TransactionStatus == "Declined";
public bool IsRefunded => TransactionStatus == "Refunded";
```

**Залежності:** US-068, US-025

**Референси:**
- PRD: Section 3.2 (Transaction statuses)

---

## Summary

| Секція | User Stories | Story Points |
|--------|--------------|--------------|
| Core Interface | US-067 | 2 |
| Webhook Models | US-068 — US-070 | 6 |
| Parsing & Validation | US-071 — US-073 | 8 |
| Response Generation | US-074 — US-075 | 5 |
| ASP.NET Core Extensions | US-076 — US-078 | 7 |
| Helper Properties | US-079 | 1 |
| **Total** | **13 User Stories** | **~29 SP** |

---

## Definition of Done (Epic Level)

- [ ] Всі 13 User Stories імплементовані
- [ ] Webhook parsing працює з Stream та string
- [ ] Signature validation надійно захищає від підробок
- [ ] Response generation правильно підписує відповіді
- [ ] ASP.NET Core extensions інтегровані
- [ ] Unit tests для всіх сценаріїв
- [ ] Integration tests з реальними webhook payloads
- [ ] XML документація для public API
- [ ] Приклади в README

---

## Webhook Flow Diagram

```
┌─────────────────┐     ┌─────────────┐     ┌──────────────┐
│    WayForPay    │     │   Merchant  │     │  Your Code   │
└────────┬────────┘     └──────┬──────┘     └──────┬───────┘
         │                     │                   │
         │ POST /webhook       │                   │
         │ {webhook JSON}      │                   │
         │────────────────────►│                   │
         │                     │                   │
         │                     │ ParseAsync()      │
         │                     │──────────────────►│
         │                     │                   │
         │                     │   Validate        │
         │                     │   Signature       │
         │                     │◄──────────────────│
         │                     │                   │
         │                     │ Process payment   │
         │                     │──────────────────►│
         │                     │                   │
         │                     │ Update order      │
         │                     │◄──────────────────│
         │                     │                   │
         │                     │ CreateResponse()  │
         │                     │──────────────────►│
         │                     │                   │
         │                     │ Signed response   │
         │                     │◄──────────────────│
         │                     │                   │
         │ {"status":"accept"} │                   │
         │◄────────────────────│                   │
         │                     │                   │
```

---

## Security Considerations

1. **Signature Validation**
   - Always validate webhook signature before processing
   - Use timing-safe comparison to prevent timing attacks
   - Log signature mismatches for security monitoring

2. **Idempotency**
   - WayForPay may send same webhook multiple times
   - Store processed OrderReferences to detect duplicates
   - Return "accept" for already processed webhooks

3. **Response Timeout**
   - WayForPay expects response within 30 seconds
   - If processing takes longer, respond first, process async
   - Consider queue-based architecture for heavy processing

4. **HTTPS Only**
   - Webhook endpoint must use HTTPS
   - Validate that request comes from WayForPay IPs (optional)
