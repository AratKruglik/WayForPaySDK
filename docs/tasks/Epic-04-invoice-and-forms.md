# Epic-04: Invoice & Forms

## Огляд

Цей Epic охоплює операції виставлення рахунків (INVOICE), генерації платіжних форм для redirect flow (PURCHASE), підтримку платіжних систем та налаштування регулярних платежів.

**Ціль:** Реалізувати Redirect API для випадків, коли мерчант не хоче обробляти картові дані напряму.

## Метадані

| Атрибут | Значення |
|---------|----------|
| **Epic ID** | Epic-04 |
| **User Stories** | US-057 — US-066 (10 stories) |
| **Приблизний обсяг** | ~18 Story Points |
| **Залежності** | Epic-01, Epic-02 |
| **Пріоритет** | High |

## Залежності

```
Epic-01 (Core Infrastructure)
    │
    └──► Epic-02 (Payment Operations)
             │
             ├──► Epic-03 (3D Secure)
             │
             └──► Epic-04 (Invoice & Forms) ◄── YOU ARE HERE
```

**Від Epic-01 потрібні:**
- Domain models (Product, Client, RegularPaymentSettings)
- PaymentSystem enum
- Signature generation
- JSON serialization

---

## User Stories

### Секція 1: INVOICE Operation

---

### US-057: InvoiceRequest Model

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want an InvoiceRequest record with all invoice fields
So that I can create invoices to send payment links to customers

**Acceptance Criteria:**

```gherkin
Scenario: InvoiceRequest requires core fields
  Given I create an InvoiceRequest
  When I don't provide MerchantAccount, OrderReference, Amount, Currency, Products, ClientEmail
  Then compilation fails

Scenario: ClientEmail is required for invoice
  Given I create invoice
  When I don't provide ClientEmail
  Then compilation fails (email needed for sending)

Scenario: Optional fields customize invoice
  Given I create InvoiceRequest
  When I set Language, PaymentSystems, OrderTimeout, OrderLifetime
  Then invoice is customized accordingly

Scenario: PaymentSystems can limit options
  Given I set PaymentSystems = Card | GooglePay
  When customer opens invoice
  Then only Card and GooglePay options are shown
```

**Технічні нотатки:**
- Required: MerchantAccount, MerchantDomainName, OrderReference, OrderDate, Amount, Currency, Products, ClientEmail
- Optional: ClientPhone, OrderTimeout (seconds), OrderLifetime (seconds), PaymentSystems, Language
- transactionType: "CREATE_INVOICE"

**Залежності:** US-006, US-002

**Референси:**
- PRD: Section 3.8 (FR-07: INVOICE operation)

---

### US-058: InvoiceResponse Model

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want an InvoiceResponse record (InvoiceUrl, QrCode, ReasonCode, Reason)
So that I can send payment links to customers or display QR codes

**Acceptance Criteria:**

```gherkin
Scenario: Successful invoice returns URL
  Given invoice is created successfully
  When I receive response
  Then InvoiceUrl contains link customer can use to pay

Scenario: Response includes QR code
  Given invoice is created
  When I check QrCode property
  Then it contains base64 encoded QR code image

Scenario: Failed invoice returns error
  Given invalid invoice data
  When I receive response
  Then InvoiceUrl is null and Reason contains error
```

**Технічні нотатки:**
- InvoiceUrl: string? (URL для оплати)
- QrCode: string? (base64 PNG)
- ReasonCode: int
- Reason: string

**Залежності:** US-008

**Референси:**
- PRD: Section 3.8 (FR-07: INVOICE output)

---

### US-059: CreateInvoiceAsync Method Implementation

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want a CreateInvoiceAsync method that creates invoices
So that I can send payment links to customers via email or display QR codes

**Acceptance Criteria:**

```gherkin
Scenario: Create invoice returns payment URL
  Given valid invoice data
  When I call CreateInvoiceAsync
  Then response.InvoiceUrl contains payment link

Scenario: Invoice URL can be sent to customer
  Given I have InvoiceUrl from response
  When customer clicks the link
  Then they see WayForPay payment page with order details

Scenario: QR code is scannable
  Given I have QrCode from response
  When I display it as image
  Then customer can scan and pay with mobile

Scenario: Invoice expires after OrderLifetime
  Given I set OrderLifetime = 3600 (1 hour)
  When customer tries to pay after 1 hour
  Then invoice is expired

Scenario: Email notification sent to customer
  Given I provide ClientEmail
  When invoice is created
  Then WayForPay sends payment link to that email
```

**Технічні нотатки:**
- Endpoint: POST https://api.wayforpay.com/api
- transactionType: "CREATE_INVOICE"
- Signature fields: merchantAccount, merchantDomainName, orderReference, orderDate, amount, currency, productName[], productCount[], productPrice[]
- WayForPay sends email automatically if ClientEmail provided

**Залежності:** US-029, US-057, US-058, US-044

**Референси:**
- PRD: Section 3.8 (FR-07: INVOICE operation)

---

### Секція 2: PURCHASE Form (Redirect API)

---

### US-060: PurchaseRequest Model

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want a PurchaseRequest record for redirect flow
So that I can create purchase forms where customer enters card on WayForPay page

**Acceptance Criteria:**

```gherkin
Scenario: PurchaseRequest doesn't contain card data
  Given I create PurchaseRequest
  When I check available properties
  Then there's no Card property (user enters on WayForPay)

Scenario: ReturnUrl is important for redirect flow
  Given I set ReturnUrl
  When payment completes
  Then customer is redirected back to my site

Scenario: ServiceUrl receives callback
  Given I set ServiceUrl
  When payment completes
  Then WayForPay POSTs result to that URL

Scenario: Language customizes payment page
  Given I set Language = "UA"
  When customer sees payment page
  Then interface is in Ukrainian
```

**Технічні нотатки:**
- Similar to ChargeRequest but without Card data
- Required: MerchantAccount, MerchantDomainName, OrderReference, OrderDate, Amount, Currency, Products
- Optional: Client, ReturnUrl, ServiceUrl, Language, PaymentSystems, DefaultPaymentSystem
- Endpoint: https://secure.wayforpay.com/pay (not /api!)

**Залежності:** US-006, US-005, US-002

**Референси:**
- PRD: Section 3.9 (FR-08: PURCHASE operation)

---

### US-061: PurchaseFormData Model

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a PurchaseFormData record with all form fields and signature
So that I can render HTML forms that POST to WayForPay payment page

**Acceptance Criteria:**

```gherkin
Scenario: FormData contains all fields
  Given I create PurchaseFormData from request
  When I check fields
  Then it has merchantAccount, orderReference, amount, signature, etc.

Scenario: Signature is pre-calculated
  Given I have PurchaseFormData
  When I render HTML form
  Then merchantSignature hidden field is already set

Scenario: FormData has action URL
  Given I check FormData
  When I look for ActionUrl
  Then it's "https://secure.wayforpay.com/pay"
```

**Технічні нотатки:**
- ActionUrl: string (POST URL)
- Fields: Dictionary<string, string> (all form fields)
- Include merchantSignature in fields
- All values as strings (HTML form format)

**Залежності:** US-060

**Референси:**
- PRD: Section 3.9 (FR-08: PURCHASE form fields)

---

### US-062: CreatePurchaseForm Method

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want a CreatePurchaseForm method that generates signed form data
So that I can redirect users to WayForPay payment page

**Acceptance Criteria:**

```gherkin
Scenario: Generate form data from request
  Given valid PurchaseRequest
  When I call CreatePurchaseForm
  Then PurchaseFormData is returned with all fields

Scenario: Form data is properly signed
  Given I render form and user submits
  When WayForPay receives form
  Then signature validation passes

Scenario: Products are serialized as arrays
  Given I have 3 products
  When form data is generated
  Then productName[], productCount[], productPrice[] have indexed keys

Scenario: Empty optional fields omitted
  Given I don't set Language
  When form data is generated
  Then language field is not included
```

**Технічні нотатки:**
- Method: `PurchaseFormData CreatePurchaseForm(PurchaseRequest request)`
- Not async (no HTTP call, just data generation)
- Sign same as Charge: merchantAccount, merchantDomainName, orderReference, orderDate, amount, currency, productName[], productCount[], productPrice[]
- Product arrays: productName[0], productName[1], etc.

**Залежності:** US-060, US-061, US-010

**Референси:**
- PRD: Section 3.9 (FR-08: PURCHASE form generation)

---

### US-063: HTML Form Generation Helper

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want a helper method to generate complete HTML form
So that I can easily render payment forms in views

**Acceptance Criteria:**

```gherkin
Scenario: Generate complete HTML form
  Given I have PurchaseFormData
  When I call ToHtml()
  Then I get <form> with all hidden inputs and submit button

Scenario: Form has correct action and method
  Given HTML form is generated
  When I check form element
  Then action is ActionUrl and method is POST

Scenario: Auto-submit option available
  Given I want form to auto-submit
  When I call ToHtml(autoSubmit: true)
  Then HTML includes JavaScript to submit on load

Scenario: Custom submit button text
  Given I want Ukrainian button
  When I call ToHtml(submitText: "Оплатити")
  Then submit button has that text
```

**Технічні нотатки:**
- Extension method: `string ToHtml(this PurchaseFormData data, bool autoSubmit = false, string? submitText = null)`
- Default submit text: "Pay" or "Оплатити"
- Auto-submit: `<script>document.forms[0].submit();</script>`
- All fields as `<input type="hidden" name="..." value="...">`

**Залежності:** US-061

**Референси:**
- PRD: Section 8.8 (Form generation example)

---

### Секція 3: Payment Systems

---

### US-064: PaymentSystem Enum to String Conversion

**Статус:** Draft
**Story Points:** S (2)

**Опис:**
As a developer
I want PaymentSystem flags enum converted to WayForPay string format
So that payment systems are specified correctly in API requests

**Acceptance Criteria:**

```gherkin
Scenario: Single payment system converts to string
  Given PaymentSystem.Card
  When converted for API
  Then result is "card"

Scenario: Multiple systems joined with semicolon
  Given PaymentSystem.Card | PaymentSystem.GooglePay
  When converted for API
  Then result is "card;googlePay"

Scenario: All flag converts to all systems
  Given PaymentSystem.All
  When converted for API
  Then result includes all available systems

Scenario: Names match WayForPay API
  Given PaymentSystem.ApplePay
  When converted
  Then result is "applePay" (camelCase)
```

**Технічні нотатки:**
- WayForPay format: semicolon-separated, camelCase
- card, privat24, applePay, googlePay, masterPass, visaCheckout, payParts, payPartsMono, credit, qrCode
- Helper method: `string ToApiString(this PaymentSystem systems)`

**Залежності:** US-002

**Референси:**
- PRD: Section 3.11 (FR-11: Payment Systems)

---

### US-065: Language Enum Support

**Статус:** Draft
**Story Points:** XS (1)

**Опис:**
As a developer
I want Language enum (UA, RU, EN, AUTO) for invoice/purchase requests
So that I can specify payment page language

**Acceptance Criteria:**

```gherkin
Scenario: Language enum has all options
  Given I need to set language
  When I use Language enum
  Then I can choose UA, RU, EN, or AUTO

Scenario: AUTO detects from browser
  Given I set Language.AUTO
  When customer opens payment page
  Then language matches their browser settings

Scenario: Language serializes correctly
  Given Language.UA
  When serialized for API
  Then result is "UA"
```

**Технічні нотатки:**
- `public enum Language { AUTO, UA, RU, EN }`
- Serializes to uppercase string

**Залежності:** US-001

**Референси:**
- PRD: Section 3.8, 3.9 (language parameter)

---

### Секція 4: Regular Payments

---

### US-066: Regular Payment Parameters in PurchaseRequest

**Статус:** Draft
**Story Points:** M (3)

**Опис:**
As a developer
I want RegularPaymentSettings in PurchaseRequest
So that I can set up subscription payments through the redirect flow

**Acceptance Criteria:**

```gherkin
Scenario: Configure monthly subscription
  Given I set RegularPaymentSettings
  When customer pays through form
  Then recurring payment is set up

Scenario: Regular parameters appear in form
  Given I set regular payment options
  When form data is generated
  Then regularMode, regularAmount, dateNext fields are included

Scenario: Customer sees subscription info
  Given regular payment is configured
  When customer views payment page
  Then they see subscription terms

Scenario: Regular amount can differ from first payment
  Given first payment is 100 UAH (with discount)
  When I set RegularAmount = 150 UAH
  Then subsequent payments will be 150 UAH
```

**Технічні нотатки:**
- PurchaseRequest optional property: RegularPaymentSettings? RegularPayment
- Form fields: regularMode, regularAmount, dateNext, dateEnd, regularCount, regularOn
- regularMode: string[] (["monthly"] etc.)
- dateNext, dateEnd: Unix timestamp

**Залежності:** US-009, US-060

**Референси:**
- PRD: Section 3.12 (FR-12: Recurring Payments)
- PRD: Section 8.7 (Regular payments example)

---

## Summary

| Секція | User Stories | Story Points |
|--------|--------------|--------------|
| INVOICE Operation | US-057 — US-059 | 8 |
| PURCHASE Form | US-060 — US-063 | 10 |
| Payment Systems | US-064 — US-065 | 3 |
| Regular Payments | US-066 | 3 |
| **Total** | **10 User Stories** | **~24 SP** |

---

## Definition of Done (Epic Level)

- [ ] Всі 10 User Stories імплементовані
- [ ] CreateInvoiceAsync повертає InvoiceUrl та QrCode
- [ ] CreatePurchaseForm генерує валідні HTML форми
- [ ] PaymentSystems enum коректно серіалізується
- [ ] Regular payments можна налаштувати через form
- [ ] Unit tests для кожної операції
- [ ] Integration tests з мок-сервером
- [ ] XML документація для public API
- [ ] Приклади форм у README

---

## Flow Diagrams

### Invoice Flow
```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│   Merchant  │     │  WayForPay  │     │   Customer  │
└──────┬──────┘     └──────┬──────┘     └──────┬──────┘
       │                   │                   │
       │ CreateInvoiceAsync│                   │
       │──────────────────►│                   │
       │                   │                   │
       │ InvoiceUrl, QrCode│                   │
       │◄──────────────────│                   │
       │                   │                   │
       │ Send URL to customer (email/SMS)      │
       │──────────────────────────────────────►│
       │                   │                   │
       │                   │◄──────────────────│
       │                   │ Customer opens URL │
       │                   │ and enters card    │
       │                   │──────────────────►│
       │                   │                   │
       │ Webhook callback  │                   │
       │◄──────────────────│                   │
       │                   │                   │
```

### Purchase Form Flow
```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│   Merchant  │     │  WayForPay  │     │   Customer  │
└──────┬──────┘     └──────┬──────┘     └──────┬──────┘
       │                   │                   │
       │ Render HTML form  │                   │
       │──────────────────────────────────────►│
       │                   │                   │
       │                   │◄──────────────────│
       │                   │ Form POST         │
       │                   │                   │
       │                   │ Payment page      │
       │                   │──────────────────►│
       │                   │                   │
       │                   │◄──────────────────│
       │                   │ Card details      │
       │                   │                   │
       │ Redirect to       │                   │
       │◄──────────────────│                   │
       │ ReturnUrl         │                   │
       │                   │                   │
       │ Webhook callback  │                   │
       │◄──────────────────│                   │
       │                   │                   │
```
