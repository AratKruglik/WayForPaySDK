# Epic-03: 3D Secure & Advanced Operations

## Огляд

Цей Epic охоплює операції 3D Secure (COMPLETE_3DS), верифікацію карт (VERIFY) та отримання списку транзакцій (TRANSACTION_LIST).

**Ціль:** Реалізувати підтримку 3D Secure автентифікації та додаткових операцій для управління платежами.

## Метадані

| Атрибут | Значення |
|---------|----------|
| **Epic ID** | Epic-03 |
| **User Stories** | US-047 — US-056 (10 stories) |
| **Приблизний обсяг** | ~18 Story Points |
| **Залежності** | Epic-01, Epic-02 |
| **Пріоритет** | High |

## Залежності

```
Epic-01 (Core Infrastructure)
    │
    └──► Epic-02 (Payment Operations)
             │
             └──► Epic-03 (3D Secure & Advanced) ◄── YOU ARE HERE
```

**Від Epic-02 потрібні:**
- IWayForPayClient interface
- WayForPayClient base implementation
- ChargeResponse з 3DS detection (Requires3Ds, D3AcsUrl)

---

## User Stories

### Секція 1: COMPLETE_3DS Operation

---

### US-047: Complete3DsRequest Model

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a Complete3DsRequest record (MerchantAccount, D3dsMd, D3dsPares)
So that I can complete 3DS authentication after user returns from bank ACS

**Acceptance Criteria:**

```gherkin
Scenario: Request requires all fields
  Given I create Complete3DsRequest
  When I don't provide MerchantAccount, D3dsMd, or D3dsPares
  Then compilation fails

Scenario: D3dsMd comes from original charge response
  Given I received ChargeResponse with Requires3Ds=true
  When I create Complete3DsRequest
  Then D3dsMd is Transaction.D3Md from original response

Scenario: D3dsPares comes from ACS callback
  Given user completed 3DS on bank page
  When bank redirects to my TermUrl
  Then D3dsPares is the PARes POST parameter
```

**Технічні нотатки:**
- Required: MerchantAccount, D3dsMd, D3dsPares
- transactionType: "COMPLETE_3DS"
- D3dsMd (MD) - merchant data, зберігається між redirect
- D3dsPares (PARes) - Payment Authentication Response від банку

**Залежності:** Epic-01

**Референси:**
- PRD: Section 3.6 (FR-05: COMPLETE_3DS operation)

---

### US-048: Complete3DsResponse Model

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a Complete3DsResponse record with full transaction details
So that I can process 3DS completion results and finalize the payment

**Acceptance Criteria:**

```gherkin
Scenario: Successful 3DS returns Approved
  Given user successfully authenticated
  When I receive Complete3DsResponse
  Then Transaction.TransactionStatus is "Approved" and IsSuccess is true

Scenario: Failed 3DS returns Declined
  Given user failed authentication or cancelled
  When I receive Complete3DsResponse
  Then Transaction.TransactionStatus is "Declined"

Scenario: Response contains full transaction details
  Given 3DS completion succeeded
  When I access Transaction
  Then I can read AuthCode, CardPan, RecToken etc.
```

**Технічні нотатки:**
- Аналогічно ChargeResponse
- Transaction: Transaction (full details)
- Reason: Reason
- MerchantSignature: string

**Залежності:** US-007, US-008

**Референси:**
- PRD: Section 3.6 (FR-05: COMPLETE_3DS output)

---

### US-049: Complete3DsAsync Method Implementation

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want a Complete3DsAsync method that completes 3DS flow
So that I can finish payments requiring 3D Secure authentication

**Acceptance Criteria:**

```gherkin
Scenario: Complete 3DS after successful authentication
  Given user authenticated on bank ACS page
  And bank redirected with PARes
  When I call Complete3DsAsync with MD and PARes
  Then payment is completed and response.IsSuccess is true

Scenario: Complete 3DS after failed authentication
  Given user cancelled on ACS page
  When I call Complete3DsAsync
  Then response shows declined status

Scenario: Invalid MD returns error
  Given MD doesn't match any pending 3DS transaction
  When I call Complete3DsAsync
  Then ApiException is thrown

Scenario: Expired 3DS session returns error
  Given too much time passed since initial charge
  When I call Complete3DsAsync
  Then ApiException with session expired error
```

**Технічні нотатки:**
- Endpoint: POST https://api.wayforpay.com/api
- transactionType: "COMPLETE_3DS"
- Signature fields: merchantAccount, d3ds_md
- Typical flow:
  1. ChargeAsync → Requires3Ds=true
  2. Redirect user to D3AcsUrl with MD, PaReq, TermUrl
  3. User authenticates on bank page
  4. Bank POSTs PARes to TermUrl
  5. Complete3DsAsync with MD and PARes

**Залежності:** US-029, US-047, US-048, US-044

**Референси:**
- PRD: Section 3.6 (FR-05: COMPLETE_3DS)
- PRD: Section 8.6 (3D Secure Flow example)

---

### US-050: 3DS Detection in ChargeResponse

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want ChargeResponse to clearly indicate when 3DS is required
So that I can redirect users to 3DS authentication automatically

**Acceptance Criteria:**

```gherkin
Scenario: Requires3Ds is true when 3DS needed
  Given card requires 3DS authentication
  When ChargeAsync returns
  Then response.Requires3Ds is true

Scenario: 3DS data available when required
  Given response.Requires3Ds is true
  When I access Transaction
  Then D3AcsUrl, D3Md, D3Pareq are all non-null

Scenario: Requires3Ds is false for successful payment
  Given payment completed without 3DS
  When ChargeAsync returns
  Then response.Requires3Ds is false

Scenario: Helper method for redirect URL
  Given response.Requires3Ds is true
  When I need redirect URL
  Then Transaction.D3AcsUrl is the URL to redirect to
```

**Технічні нотатки:**
- ChargeResponse property:
  ```csharp
  public bool Requires3Ds =>
      Reason.Is3DsRequired &&
      Transaction?.D3AcsUrl != null;
  ```
- 3DS fields in Transaction:
  - D3AcsUrl: URL банку для автентифікації
  - D3Md: MD параметр для ідентифікації
  - D3Pareq: PAReq для відправки на ACS

**Залежності:** US-031

**Референси:**
- PRD: Section 3.2 (3DS fields in response)

---

### Секція 2: VERIFY Operation

---

### US-051: VerifyRequest Model

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a VerifyRequest record for card verification
So that I can verify cards without charging (for tokenization)

**Acceptance Criteria:**

```gherkin
Scenario: VerifyRequest has minimal required fields
  Given I create VerifyRequest
  When I provide MerchantAccount, MerchantDomainName, OrderReference, Amount=1
  Then request is valid for verification

Scenario: Verification uses 1 UAH amount
  Given I want to verify a card
  When I create VerifyRequest
  Then Amount should typically be 1 (will be refunded)

Scenario: Client data is optional
  Given I only need to verify card
  When I create VerifyRequest without Client
  Then request is still valid
```

**Технічні нотатки:**
- Required: MerchantAccount, MerchantDomainName, OrderReference, OrderDate, Amount (1), Currency, Card
- Endpoint: https://secure.wayforpay.com/verify (not /api)
- Amount зазвичай 1 UAH, який списується і одразу повертається

**Залежності:** US-003, US-006

**Референси:**
- PRD: Section 3.10 (FR-09: VERIFY operation)

---

### US-052: VerifyResponse Model

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a VerifyResponse record (RecToken, CardPan, TransactionStatus)
So that I can get card token after successful verification

**Acceptance Criteria:**

```gherkin
Scenario: Successful verification returns token
  Given card is valid
  When verification completes
  Then RecToken is populated with token for recurring payments

Scenario: Response includes card info
  Given verification succeeded
  When I check response
  Then CardPan has masked card number, CardType has Visa/MC

Scenario: Failed verification has no token
  Given card is invalid
  When verification fails
  Then RecToken is null and TransactionStatus is "Declined"
```

**Технічні нотатки:**
- RecToken: string? (token for subsequent charges)
- CardPan: string? (masked: 411111****1111)
- CardType: string? (Visa, MasterCard)
- TransactionStatus: string

**Залежності:** US-004, US-008

**Референси:**
- PRD: Section 3.10 (FR-09: VERIFY output)

---

### US-053: VerifyAsync Method Implementation

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want a VerifyAsync method that verifies cards
So that I can obtain recTokens for recurring payments without full charge

**Acceptance Criteria:**

```gherkin
Scenario: Verify valid card returns token
  Given valid card details
  When I call VerifyAsync
  Then response contains RecToken for future charges

Scenario: Verify processes 1 UAH transaction
  Given I verify a card
  When verification completes
  Then 1 UAH was authorized and released (not actually charged)

Scenario: Invalid card returns declined
  Given invalid card number
  When I call VerifyAsync
  Then response shows Declined status and no token

Scenario: Expired card returns declined
  Given card with past expiry date
  When I call VerifyAsync
  Then response shows appropriate error
```

**Технічні нотатки:**
- Endpoint: POST https://secure.wayforpay.com/verify (different from main API!)
- transactionType: "VERIFY"
- Returns token that can be used in ChargeAsync with RecToken instead of Card
- Token stored server-side, PCI DSS compliant

**Залежності:** US-029, US-051, US-052, US-044

**Референси:**
- PRD: Section 3.10 (FR-09: VERIFY operation)

---

### Секція 3: TRANSACTION_LIST Operation

---

### US-054: TransactionListRequest Model

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a TransactionListRequest record (MerchantAccount, DateBegin, DateEnd)
So that I can query transactions for a specific time period

**Acceptance Criteria:**

```gherkin
Scenario: Request requires date range
  Given I create TransactionListRequest
  When I don't provide DateBegin or DateEnd
  Then compilation fails

Scenario: Dates are Unix timestamps
  Given I set DateBegin and DateEnd
  When request is serialized
  Then dates are Unix timestamps (seconds since epoch)

Scenario: Date range can span multiple days
  Given I want last 7 days transactions
  When I set DateBegin=7 days ago, DateEnd=now
  Then request is valid
```

**Технічні нотатки:**
- Required: MerchantAccount, DateBegin (Unix timestamp), DateEnd (Unix timestamp)
- transactionType: "TRANSACTION_LIST"
- DateBegin/DateEnd: long (Unix timestamp seconds)

**Залежності:** Epic-01

**Референси:**
- PRD: Section 3.7 (FR-06: TRANSACTION_LIST operation)

---

### US-055: TransactionListResponse Model

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a TransactionListResponse record with IReadOnlyList<Transaction>
So that I can process and display transaction history

**Acceptance Criteria:**

```gherkin
Scenario: Response contains transaction array
  Given transactions exist in date range
  When I receive response
  Then TransactionList contains Transaction objects

Scenario: Empty period returns empty list
  Given no transactions in date range
  When I receive response
  Then TransactionList is empty (not null)

Scenario: Each transaction has full details
  Given response has transactions
  When I access TransactionList[0]
  Then all Transaction fields are populated
```

**Технічні нотатки:**
- TransactionList: IReadOnlyList<Transaction>
- Reason: Reason
- Can return many transactions, consider pagination in future

**Залежності:** US-007, US-008

**Референси:**
- PRD: Section 3.7 (FR-06: TRANSACTION_LIST output)

---

### US-056: GetTransactionListAsync Method Implementation

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want a GetTransactionListAsync method that retrieves transactions
So that I can generate reports and reconcile payments

**Acceptance Criteria:**

```gherkin
Scenario: Get transactions for date range
  Given transactions exist between Jan 1 and Jan 7
  When I call GetTransactionListAsync for that range
  Then response contains all transactions in period

Scenario: Empty date range returns empty list
  Given no transactions in specified period
  When I call GetTransactionListAsync
  Then TransactionList is empty

Scenario: Transactions include all statuses
  Given various transactions (approved, declined, refunded)
  When I call GetTransactionListAsync
  Then all statuses are included in results

Scenario: Date boundaries are inclusive
  Given transaction at exactly DateBegin time
  When I call GetTransactionListAsync
  Then that transaction is included
```

**Технічні нотатки:**
- Endpoint: POST https://api.wayforpay.com/api
- transactionType: "TRANSACTION_LIST"
- Signature fields: merchantAccount, dateBegin, dateEnd
- Useful for daily reconciliation, reporting dashboards

**Залежності:** US-029, US-054, US-055, US-044

**Референси:**
- PRD: Section 3.7 (FR-06: TRANSACTION_LIST operation)

---

## Summary

| Секція | User Stories | Story Points |
|--------|--------------|--------------|
| COMPLETE_3DS Operation | US-047 — US-050 | 9 |
| VERIFY Operation | US-051 — US-053 | 7 |
| TRANSACTION_LIST Operation | US-054 — US-056 | 7 |
| **Total** | **10 User Stories** | **~23 SP** |

---

## Definition of Done (Epic Level)

- [ ] Всі 10 User Stories імплементовані
- [ ] 3DS flow повністю функціональний (Charge → Redirect → Complete)
- [ ] VERIFY повертає токени для recurring payments
- [ ] TRANSACTION_LIST працює з date ranges
- [ ] Unit tests для кожної операції
- [ ] Integration tests з мок-сервером
- [ ] XML документація для public API
- [ ] Приклади використання 3DS flow в README

---

## 3DS Flow Diagram

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│   Merchant  │     │  WayForPay  │     │    Bank     │     │    User     │
└──────┬──────┘     └──────┬──────┘     └──────┬──────┘     └──────┬──────┘
       │                   │                   │                   │
       │ ChargeAsync()     │                   │                   │
       │──────────────────►│                   │                   │
       │                   │                   │                   │
       │ Requires3Ds=true  │                   │                   │
       │ (AcsUrl, MD, PAReq)                   │                   │
       │◄──────────────────│                   │                   │
       │                   │                   │                   │
       │ Redirect to AcsUrl with MD, PAReq    │                   │
       │──────────────────────────────────────────────────────────►│
       │                   │                   │                   │
       │                   │                   │◄──────────────────│
       │                   │                   │   User enters    │
       │                   │                   │   OTP/password   │
       │                   │                   │──────────────────►│
       │                   │                   │                   │
       │◄──────────────────────────────────────│                   │
       │ POST to TermUrl   │                   │                   │
       │ with MD, PARes    │                   │                   │
       │                   │                   │                   │
       │ Complete3DsAsync()│                   │                   │
       │ (MD, PARes)       │                   │                   │
       │──────────────────►│                   │                   │
       │                   │                   │                   │
       │ Success/Failure   │                   │                   │
       │◄──────────────────│                   │                   │
       │                   │                   │                   │
```

---

## Test Scenarios (Sandbox)

| Карта | 3DS Поведінка | Використання |
|-------|---------------|--------------|
| 4111111111111111 | No 3DS | Direct approval |
| 4111111111111113 | 3DS Required | Full 3DS flow testing |
| 5555555555554444 | No 3DS (MC) | MasterCard without 3DS |
