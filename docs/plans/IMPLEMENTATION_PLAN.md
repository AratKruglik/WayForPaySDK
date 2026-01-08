# Implementation Plan

## WayForPaySDK for .NET

**Версія документу:** 1.4
**Дата:** 08.01.2026
**Автор:** Business Analysis Team
**Статус:** Approved

---

## Зміст

1. [Executive Summary](#1-executive-summary)
2. [Фази реалізації](#2-фази-реалізації)
3. [Критичний шлях](#3-критичний-шлях)
4. [Детальний план по Sprint-ах](#4-детальний-план-по-sprint-ах)
5. [Паралельна робота](#5-паралельна-робота)
6. [Milestones та Definition of Done](#6-milestones-та-definition-of-done)
7. [Залежності від зовнішніх ресурсів](#7-залежності-від-зовнішніх-ресурсів)
8. [Ризики та мітигація](#8-ризики-та-мітигація)
9. [Команда та ролі](#9-команда-та-ролі)
10. [Метрики успіху](#10-метрики-успіху)

---

## 1. Executive Summary

### 1.1 Огляд проекту

WayForPaySDK - це .NET бібліотека для інтеграції з платіжною системою WayForPay. Проект охоплює:

| Метрика | Значення |
|---------|----------|
| Epic-ів | 6 |
| User Stories | 100 |
| Story Points | ~233 SP |
| Орієнтовна тривалість | 10-12 тижнів |
| Target Frameworks | .NET 8.0, 9.0, 10.0 |

### 1.3 Поточний статус реалізації

| Phase | Epic | Статус | Дата |
|-------|------|--------|------|
| Phase 1 | Epic-01: Core Infrastructure | ✅ ЗАВЕРШЕНО | 2026-01-08 |
| Phase 2 | Epic-02: Payment Operations | ✅ ЗАВЕРШЕНО | 2026-01-08 |
| Phase 3 | Epic-03: Extended Operations | ✅ ЗАВЕРШЕНО | 2026-01-08 |
| Phase 4 | Epic-04: Webhook Integration | ⏳ Очікує | - |
| Phase 5 | Epic-05: Testing & Quality | ⏳ Очікує | - |
| Phase 6 | Epic-06: Documentation & Samples | ⏳ Очікує | - |

> **Примітка:** .NET 6.0 видалено з Target Frameworks через несумісність з `required` keyword та JSON source generation для init-only properties.

### 1.2 Ключові дати

```
┌─────────────────────────────────────────────────────────────────────────┐
│ Week 1-2     │ Week 3-4   │ Week 5-6    │ Week 7-8   │ Week 9-10  │ W11 │
├─────────────────────────────────────────────────────────────────────────┤
│  PHASE 1     │  PHASE 2   │   PHASE 3   │  PHASE 4   │  PHASE 5   │ REL │
│  Foundation  │  Core Ops  │  Extended   │  Webhooks  │  Polish    │     │
│  (Epic-01)   │ (Epic-02)  │ (03+04)     │ (Epic-05)  │ (Epic-06)  │     │
│              │            │             │            │            │     │
│  58 SP       │  50 SP     │  47 SP      │  29 SP     │  49 SP     │     │
└─────────────────────────────────────────────────────────────────────────┘
```

### 1.3 Стратегія реалізації

1. **Foundation First** - Epic-01 є блокером для всіх інших Epic-ів
2. **Parallel Streams** - Epic-02, Epic-04, Epic-05 можуть виконуватись паралельно після Epic-01
3. **Sequential Dependencies** - Epic-03 залежить від Epic-02; Epic-06 від всіх попередніх
4. **Incremental Delivery** - кожна фаза завершується робочим SDK з обмеженим функціоналом

---

## 2. Фази реалізації

### Phase 1: Foundation (Тижні 1-2) ✅ ЗАВЕРШЕНО

**Epic-01: Core Infrastructure** — *Реалізовано 2026-01-08*

```
┌────────────────────────────────────────────────────────────────┐
│                   PHASE 1: FOUNDATION ✅ DONE                   │
│                         Epic-01 (58 SP)                         │
├────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Sprint 1.1 (Week 1)                                           │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ US-001: Project Setup               [S - 2 SP]          │   │
│  │ US-002: Domain Enums                [S - 2 SP]          │   │
│  │ US-025: ReasonCodes Constants       [S - 2 SP]          │   │
│  │ US-003: Card Model                  [XS - 1 SP]         │   │
│  │ US-004: CardToken Model             [XS - 1 SP]         │   │
│  │ US-005: Client Model                [S - 2 SP]          │   │
│  │ US-006: Product Model               [XS - 1 SP]         │   │
│  │ US-007: Transaction Model           [M - 3 SP]          │   │
│  │ US-008: Reason Model                [S - 2 SP]          │   │
│  │ US-009: RegularPaymentSettings      [S - 2 SP]          │   │
│  │ US-010: ISignatureGenerator         [S - 2 SP]          │   │
│  │ US-011: HmacMd5SignatureGenerator   [M - 3 SP]          │   │
│  │ US-012: Timing-Safe Validation      [S - 2 SP]          │   │
│  └─────────────────────────────────────────────────────────┘   │
│  Subtotal: 25 SP                                               │
│                                                                 │
│  Sprint 1.2 (Week 2)                                           │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ US-013: WayForPayOptions            [S - 2 SP]          │   │
│  │ US-014: Options Validator           [S - 2 SP]          │   │
│  │ US-015: Typed HTTP Client           [M - 3 SP]          │   │
│  │ US-016: Connection Pooling          [S - 2 SP]          │   │
│  │ US-017: AddWayForPay (Action)       [M - 3 SP]          │   │
│  │ US-018: AddWayForPay (IConfig)      [S - 2 SP]          │   │
│  │ US-019: IHttpClientBuilder Return   [S - 2 SP]          │   │
│  │ US-020: WayForPayException Base     [S - 2 SP]          │   │
│  │ US-021: ApiException                [S - 2 SP]          │   │
│  │ US-022: SignatureException          [S - 2 SP]          │   │
│  │ US-023: ValidationException         [S - 2 SP]          │   │
│  │ US-024: NetworkException            [S - 2 SP]          │   │
│  │ US-026: JSON Context                [M - 3 SP]          │   │
│  │ US-027: UnixTimestampConverter      [S - 2 SP]          │   │
│  │ US-028: DecimalConverter            [S - 2 SP]          │   │
│  └─────────────────────────────────────────────────────────┘   │
│  Subtotal: 33 SP                                               │
│                                                                 │
└────────────────────────────────────────────────────────────────┘
```

**Deliverables Phase 1:** ✅ Всі доставлені
- ✅ Проект з multi-target configuration (net8.0, net9.0, net10.0)
- ✅ Всі domain models (Card, CardToken, Client, Product, Transaction, Reason, Regular)
- ✅ Signature generation/validation (HMAC-MD5 з timing-safe порівнянням)
- ✅ DI infrastructure (AddWayForPay extension methods)
- ✅ Exception hierarchy (5 типів винятків)
- ✅ JSON serialization (source-generated context)

**Реалізовані файли Phase 1 (27 файлів):**
- `Domain/Enums/*.cs` (7 файлів) - TransactionStatus, PaymentSystem, Currency, Language, MerchantTransactionType, RegularBehavior, RegularMode
- `Domain/*.cs` (7 файлів) - Card, CardToken, Client, Product, Reason, Regular, Transaction
- `Constants/ReasonCodes.cs` - 50+ констант кодів причин
- `Exceptions/*.cs` (5 файлів) - WayForPayException, ApiException, SignatureException, InvalidFieldException, JsonParseException
- `Options/WayForPayOptions.cs` - конфігурація SDK
- `Crypto/*.cs` (2 файли) - ISignatureGenerator, SignatureGenerator
- `Serialization/WayForPayJsonContext.cs` - JSON source generator
- `Http/*.cs` (2 файли) - IWayForPayHttpClient, WayForPayHttpClient
- `Extensions/ServiceCollectionExtensions.cs` - AddWayForPay()

---

### Phase 2: Core Operations (Тижні 3-4) ✅ ЗАВЕРШЕНО

**Epic-02: Payment Operations** — *Завершено 2026-01-08*

```
┌────────────────────────────────────────────────────────────────┐
│                 PHASE 2: CORE OPERATIONS ✅ DONE                 │
│                         Epic-02 (50 SP)                         │
├────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Sprint 2.1 (Week 3)                                           │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ US-029: IWayForPayClient Interface  [M - 3 SP]          │   │
│  │ US-030: ChargeRequest Model         [M - 3 SP]          │   │
│  │ US-031: ChargeResponse Model        [M - 3 SP]          │   │
│  │ US-033: Charge Request Signature    [M - 3 SP]          │   │
│  │ US-034: Charge Response Validation  [M - 3 SP]          │   │
│  │ US-044: WayForPayClient Base        [L - 5 SP]          │   │
│  │ US-032: ChargeAsync Implementation  [L - 5 SP]          │   │
│  └─────────────────────────────────────────────────────────┘   │
│  Subtotal: 25 SP                                               │
│                                                                 │
│  Sprint 2.2 (Week 4)                                           │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ US-035: RefundRequest Model         [S - 2 SP]          │   │
│  │ US-036: RefundResponse Model        [S - 2 SP]          │   │
│  │ US-037: RefundAsync Implementation  [M - 3 SP]          │   │
│  │ US-038: CheckStatusRequest          [XS - 1 SP]         │   │
│  │ US-039: CheckStatusResponse         [S - 2 SP]          │   │
│  │ US-040: CheckStatusAsync            [M - 3 SP]          │   │
│  │ US-041: SettleRequest               [S - 2 SP]          │   │
│  │ US-042: SettleResponse              [S - 2 SP]          │   │
│  │ US-043: SettleAsync                 [M - 3 SP]          │   │
│  │ US-045: Error to Exception Mapping  [M - 3 SP]          │   │
│  │ US-046: Transient Error Detection   [S - 2 SP]          │   │
│  └─────────────────────────────────────────────────────────┘   │
│  Subtotal: 25 SP                                               │
│                                                                 │
└────────────────────────────────────────────────────────────────┘
```

**Deliverables Phase 2:** ✅ Всі доставлені
- ✅ Повністю функціональний IWayForPayClient
- ✅ CHARGE операція (з карткою та токеном)
- ✅ REFUND операція
- ✅ CHECK_STATUS операція
- ✅ SETTLE операція (підтвердження авторизації)
- ✅ VOID операція (скасування авторизації)
- ✅ CreatePurchase операція (redirect flow)
- ✅ CreateInvoice операція (виставлення рахунку)

**Реалізовані файли Phase 2 (16 файлів):**
- `Requests/ApiRequest.cs` - базовий клас запитів ✅
- `Responses/ApiResponse.cs` - базовий клас відповідей ✅
- `Requests/ChargeRequest.cs` - запит на списання ✅
- `Responses/ChargeResponse.cs` - відповідь на списання (з 3DS) ✅
- `Requests/RefundRequest.cs` - запит на повернення ✅
- `Responses/RefundResponse.cs` - відповідь на повернення ✅
- `Requests/CheckStatusRequest.cs` - запит на статус ✅
- `Responses/CheckStatusResponse.cs` - відповідь на статус ✅
- `Requests/SettleRequest.cs` - запит на підтвердження авторизації ✅
- `Responses/SettleResponse.cs` - відповідь на підтвердження ✅
- `Requests/VoidRequest.cs` - запит на скасування авторизації ✅
- `Responses/VoidResponse.cs` - відповідь на скасування ✅
- `Requests/PurchaseRequest.cs` - запит на redirect flow оплату ✅
- `Responses/PurchaseResponse.cs` - відповідь з URL для редиректу ✅
- `Requests/InvoiceRequest.cs` - запит на створення рахунку ✅
- `Responses/InvoiceResponse.cs` - відповідь з URL рахунку ✅
- `Services/IWayForPayClient.cs` - головний інтерфейс SDK (8 методів) ✅
- `Services/WayForPayClient.cs` - реалізація з верифікацією підпису ✅

**Примітка:** US-045 (Error Mapping) та US-046 (Transient Detection) перенесено до Phase 5 (Polly integration).

---

### Phase 3: Extended Operations (Тижні 5-6) ✅ ЗАВЕРШЕНО

**Epic-03: Extended Operations** — *Завершено 2026-01-08*

```
┌────────────────────────────────────────────────────────────────┐
│                 PHASE 3: EXTENDED OPERATIONS ✅ COMPLETED       │
│                         Epic-03 (47 SP)                         │
├────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Sprint 3.1 (Week 5) - 3D Secure & Forms                       │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ ✅ US-047: Complete3DsRequest       [S - 2 SP]          │   │
│  │ ✅ US-048: Complete3DsResponse      [S - 2 SP]          │   │
│  │ ✅ US-049: Complete3DsAsync         [M - 3 SP]          │   │
│  │ ✅ US-050: 3DS Detection Helper     [S - 2 SP]          │   │
│  │ ✅ US-061: PurchaseFormData Model   [S - 2 SP]          │   │
│  │ ✅ US-062: CreatePurchaseForm       [M - 3 SP]          │   │
│  │ ✅ US-063: HTML Form Generation     [S - 2 SP]          │   │
│  └─────────────────────────────────────────────────────────┘   │
│  Subtotal Sprint 3.1: 16 SP (все завершено)                    │
│                                                                 │
│  Sprint 3.2 (Week 6) - Advanced Operations & Regular Payments  │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ ✅ US-051: VerifyRequest            [S - 2 SP]          │   │
│  │ ✅ US-052: VerifyResponse           [S - 2 SP]          │   │
│  │ ✅ US-053: VerifyAsync              [M - 3 SP]          │   │
│  │ ✅ US-054: TransactionListRequest   [S - 2 SP]          │   │
│  │ ✅ US-055: TransactionListResponse  [S - 2 SP]          │   │
│  │ ✅ US-056: GetTransactionListAsync  [M - 3 SP]          │   │
│  │ ✅ US-066: Regular Payments         [M - 3 SP]          │   │
│  └─────────────────────────────────────────────────────────┘   │
│  Subtotal Sprint 3.2: 17 SP (все завершено)                    │
│                                                                 │
└────────────────────────────────────────────────────────────────┘
```

**Deliverables Phase 3:** ✅ Всі завершено
- ✅ 3D Secure flow (COMPLETE_3DS) - Complete3DSRequest, Complete3DSResponse, Complete3DSAsync
- ✅ 3DS Detection helpers - ChargeResponseExtensions, VerifyResponseExtensions
- ✅ VERIFY операція (card tokenization) - VerifyRequest, VerifyResponse, VerifyAsync
- ✅ TRANSACTION_LIST - TransactionListRequest, TransactionListResponse, GetTransactionListAsync
- ✅ PURCHASE form generation - PurchaseFormData, PaymentFormBuilder (з HTML generation)
- ✅ Regular payments support - ChargeWithRegularAsync, CreatePurchaseWithRegularAsync

**Реалізовані файли Phase 3 (14 файлів):**
- `Requests/Complete3DSRequest.cs` - завершення 3DS аутентифікації ✅
- `Responses/Complete3DSResponse.cs` - відповідь після 3DS ✅
- `Requests/VerifyRequest.cs` - верифікація картки без списання ✅
- `Responses/VerifyResponse.cs` - відповідь з recToken ✅
- `Requests/TransactionListRequest.cs` - запит на список транзакцій ✅
- `Responses/TransactionListResponse.cs` - список транзакцій ✅
- `Extensions/ChargeResponseExtensions.cs` - 3DS detection для ChargeResponse ✅
- `Extensions/VerifyResponseExtensions.cs` - 3DS detection для VerifyResponse ✅
- `Forms/PurchaseFormData.cs` - модель даних HTML форми ✅
- `Forms/PaymentFormBuilder.cs` - генератор HTML форм ✅
- `Requests/ChargeRequest.cs` - додано Regular payments fields ✅
- `Requests/PurchaseRequest.cs` - додано Regular payments fields ✅
- `Services/IWayForPayClient.cs` - додано 5 нових методів ✅
- `Services/WayForPayClient.cs` - імплементація 5 нових методів ✅

**Прогрес Phase 3:** 47 SP з 47 SP завершено (100%) ✅

---

### Phase 4: Webhook Integration (Тижні 7-8)

**Epic-05: Webhook Integration**

```
┌────────────────────────────────────────────────────────────────┐
│                    PHASE 4: WEBHOOK INTEGRATION                 │
│                         Epic-05 (29 SP)                         │
├────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Sprint 4.1 (Week 7)                                           │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ US-067: IWebhookHandler Interface   [S - 2 SP]          │   │
│  │ US-068: WebhookPayload Model        [M - 3 SP]          │   │
│  │ US-069: WebhookResponse Model       [S - 2 SP]          │   │
│  │ US-070: WebhookStatus Enum          [XS - 1 SP]         │   │
│  │ US-071: ParseAsync (Stream)         [M - 3 SP]          │   │
│  │ US-072: Parse (string)              [S - 2 SP]          │   │
│  │ US-073: Signature Validation        [M - 3 SP]          │   │
│  └─────────────────────────────────────────────────────────┘   │
│  Subtotal: 16 SP                                               │
│                                                                 │
│  Sprint 4.2 (Week 8)                                           │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ US-074: CreateResponse              [M - 3 SP]          │   │
│  │ US-075: SerializeResponse           [S - 2 SP]          │   │
│  │ US-076: ASP.NET ParseAsync          [S - 2 SP]          │   │
│  │ US-077: ToActionResult Extension    [S - 2 SP]          │   │
│  │ US-078: HandleAsync Extension       [M - 3 SP]          │   │
│  │ US-079: Helper Properties           [XS - 1 SP]         │   │
│  └─────────────────────────────────────────────────────────┘   │
│  Subtotal: 13 SP                                               │
│                                                                 │
└────────────────────────────────────────────────────────────────┘
```

**Deliverables Phase 4:**
- Complete webhook handling
- ASP.NET Core integration
- Signature validation
- Response generation

---

### Phase 5: Builders & Polish (Тижні 9-10)

**Epic-06: Builders & Polish**

```
┌────────────────────────────────────────────────────────────────┐
│                     PHASE 5: BUILDERS & POLISH                  │
│                         Epic-06 (49 SP)                         │
├────────────────────────────────────────────────────────────────┤
│                                                                 │
│  Sprint 5.1 (Week 9)                                           │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ US-080: ChargeRequestBuilder Create [S - 2 SP]          │   │
│  │ US-081: Order Methods               [S - 2 SP]          │   │
│  │ US-082: Product Methods             [S - 2 SP]          │   │
│  │ US-083: Payment Methods             [S - 2 SP]          │   │
│  │ US-084: Client Method               [XS - 1 SP]         │   │
│  │ US-085: Callback Methods            [XS - 1 SP]         │   │
│  │ US-086: Transaction Type Methods    [S - 2 SP]          │   │
│  │ US-087: 3DS Methods                 [XS - 1 SP]         │   │
│  │ US-088: Build with Validation       [M - 3 SP]          │   │
│  │ US-089: RefundRequestBuilder        [M - 3 SP]          │   │
│  │ US-090: InvoiceRequestBuilder       [M - 3 SP]          │   │
│  │ US-091: PurchaseFormBuilder         [M - 3 SP]          │   │
│  │ US-092: CheckRequestBuilder         [S - 2 SP]          │   │
│  └─────────────────────────────────────────────────────────┘   │
│  Subtotal: 27 SP                                               │
│                                                                 │
│  Sprint 5.2 (Week 10)                                          │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ US-093: Polly Retry Policy          [M - 3 SP]          │   │
│  │ US-094: Polly Circuit Breaker       [M - 3 SP]          │   │
│  │ US-095: AddWayForPayWithPolly       [S - 2 SP]          │   │
│  │ US-096: Logging Handler             [M - 3 SP]          │   │
│  │ US-097: XML Documentation           [L - 5 SP]          │   │
│  │ US-098: README.md                   [M - 3 SP]          │   │
│  │ US-099: API Reference               [M - 3 SP]          │   │
│  │ US-100: Migration Guide             [S - 2 SP]          │   │
│  └─────────────────────────────────────────────────────────┘   │
│  Subtotal: 24 SP (includes buffer)                             │
│                                                                 │
└────────────────────────────────────────────────────────────────┘
```

**Deliverables Phase 5:**
- All Fluent Builders
- Polly integration
- Logging support
- Complete documentation
- NuGet package ready

---

## 3. Критичний шлях

### 3.1 Діаграма критичного шляху

```
                                    CRITICAL PATH
═══════════════════════════════════════════════════════════════════════════

Week 1          Week 2          Week 3          Week 4          Week 5
   │               │               │               │               │
   ▼               ▼               ▼               ▼               ▼
┌───────┐      ┌───────┐      ┌───────┐      ┌───────┐      ┌───────┐
│US-001 │─────►│US-013 │─────►│US-029 │─────►│US-044 │─────►│US-047 │
│Setup  │      │Options│      │IClient│      │Client │      │3DS    │
└───────┘      └───────┘      └───────┘      └───────┘      └───────┘
   │               │               │               │               │
   ▼               ▼               ▼               ▼               ▼
┌───────┐      ┌───────┐      ┌───────┐      ┌───────┐      ┌───────┐
│US-003 │─────►│US-017 │─────►│US-030 │─────►│US-032 │─────►│US-049 │
│Models │      │DI Ext │      │Charge │      │Async  │      │Complete│
└───────┘      └───────┘      └───────┘      │Req    │      │3DS    │
   │               │                          └───────┘      └───────┘
   ▼               ▼                                              │
┌───────┐      ┌───────┐                                         │
│US-010 │─────►│US-026 │                                         │
│ISig   │      │JSON   │                                         │
└───────┘      └───────┘                                         │
   │                                                              │
   ▼                                                              ▼
┌───────┐                                                    ┌───────┐
│US-011 │──────────────────────────────────────────────────►│US-080 │
│HmacMD5│                                                    │Builder│
└───────┘                                                    └───────┘

Week 6          Week 7          Week 8          Week 9          Week 10
   │               │               │               │               │
   ▼               ▼               ▼               ▼               ▼
┌───────┐      ┌───────┐      ┌───────┐      ┌───────┐      ┌───────┐
│US-051 │─────►│US-067 │─────►│US-074 │─────►│US-088 │─────►│US-097 │
│Verify │      │Webhook│      │Create │      │Build  │      │Docs   │
└───────┘      │Handler│      │Response│      └───────┘      └───────┘
               └───────┘      └───────┘              │               │
                   │               │                 ▼               ▼
                   ▼               ▼            ┌───────┐      ┌───────┐
               ┌───────┐      ┌───────┐        │US-093 │─────►│US-098 │
               │US-073 │─────►│US-078 │        │Polly  │      │README │
               │Sig Val│      │Handle │        └───────┘      └───────┘
               └───────┘      └───────┘

═══════════════════════════════════════════════════════════════════════════
```

### 3.2 Блокуючі залежності

| Блокер | Блокує | Причина |
|--------|--------|---------|
| US-001 (Setup) | Все | Проект має бути налаштований |
| US-003-009 (Models) | US-030, US-068 | Request/Response потребують моделей |
| US-010-012 (Signature) | US-032, US-073 | Всі операції потребують підпису |
| US-017 (DI) | US-044 | Client реєструється через DI |
| US-026 (JSON) | US-032, US-071 | Серіалізація для HTTP |
| US-044 (Client) | US-032-043, US-049, US-053, US-056, US-059 | Base implementation |
| US-032 (Charge) | US-050 | 3DS detection в ChargeResponse |

### 3.3 Найкоротший шлях до MVP

**Мінімально життєздатний продукт (4 тижні):**

```
Week 1-2: Epic-01 (Foundation)
Week 3-4: Epic-02 (Core Operations - CHARGE, REFUND, CHECK)

MVP включає:
- ChargeAsync з картою та RecToken
- RefundAsync (повний та частковий)
- CheckStatusAsync
- Basic webhook handling (manual signature check)
```

---

## 4. Детальний план по Sprint-ах

### Sprint 1.1: Project Foundation (Week 1)

| US | Назва | SP | Залежності | Виконавець |
|----|-------|-----|------------|------------|
| US-001 | Project Setup | 2 | - | Dev 1 |
| US-002 | Domain Enums | 2 | US-001 | Dev 1 |
| US-025 | ReasonCodes | 2 | US-001 | Dev 1 |
| US-003 | Card Model | 1 | US-001 | Dev 2 |
| US-004 | CardToken Model | 1 | US-001 | Dev 2 |
| US-005 | Client Model | 2 | US-001 | Dev 2 |
| US-006 | Product Model | 1 | US-001 | Dev 2 |
| US-007 | Transaction Model | 3 | US-002, US-004 | Dev 1 |
| US-008 | Reason Model | 2 | US-025 | Dev 2 |
| US-009 | RegularPaymentSettings | 2 | US-002 | Dev 2 |
| US-010 | ISignatureGenerator | 2 | US-001 | Dev 1 |
| US-011 | HmacMd5Generator | 3 | US-010 | Dev 1 |
| US-012 | Timing-Safe Validation | 2 | US-011 | Dev 1 |

**Sprint Goal:** Базова інфраструктура проекту, всі domain models, signature generation.

**Sprint Velocity Target:** 25 SP

---

### Sprint 1.2: Infrastructure (Week 2)

| US | Назва | SP | Залежності | Виконавець |
|----|-------|-----|------------|------------|
| US-013 | WayForPayOptions | 2 | US-001 | Dev 1 |
| US-014 | Options Validator | 2 | US-013 | Dev 1 |
| US-015 | HTTP Client | 3 | US-013 | Dev 1 |
| US-016 | Connection Pooling | 2 | US-015 | Dev 1 |
| US-017 | AddWayForPay (Action) | 3 | US-013, US-011 | Dev 1 |
| US-018 | AddWayForPay (IConfig) | 2 | US-017 | Dev 1 |
| US-019 | IHttpClientBuilder | 2 | US-017 | Dev 1 |
| US-020 | WayForPayException | 2 | US-001 | Dev 2 |
| US-021 | ApiException | 2 | US-020, US-025 | Dev 2 |
| US-022 | SignatureException | 2 | US-020 | Dev 2 |
| US-023 | ValidationException | 2 | US-020 | Dev 2 |
| US-024 | NetworkException | 2 | US-020 | Dev 2 |
| US-026 | JSON Context | 3 | US-003-009 | Dev 2 |
| US-027 | UnixTimestampConverter | 2 | US-026 | Dev 2 |
| US-028 | DecimalConverter | 2 | US-026 | Dev 2 |

**Sprint Goal:** DI integration, HTTP client, exceptions, JSON serialization.

**Sprint Velocity Target:** 33 SP

**Milestone: M1 - Foundation Complete**

---

### Sprint 2.1: CHARGE Operation (Week 3)

| US | Назва | SP | Залежності | Виконавець |
|----|-------|-----|------------|------------|
| US-029 | IWayForPayClient | 3 | Epic-01 | Dev 1 |
| US-030 | ChargeRequest | 3 | US-003, US-006 | Dev 2 |
| US-031 | ChargeResponse | 3 | US-007, US-008 | Dev 2 |
| US-033 | Charge Signature | 3 | US-010, US-030 | Dev 1 |
| US-034 | Response Validation | 3 | US-010, US-031 | Dev 1 |
| US-044 | WayForPayClient | 5 | US-029, US-015, US-026 | Dev 1 |
| US-032 | ChargeAsync | 5 | US-044, US-033, US-034 | Dev 1 + Dev 2 |

**Sprint Goal:** Повністю функціональна CHARGE операція.

**Sprint Velocity Target:** 25 SP

---

### Sprint 2.2: Other Operations (Week 4)

| US | Назва | SP | Залежності | Виконавець |
|----|-------|-----|------------|------------|
| US-035 | RefundRequest | 2 | Epic-01 | Dev 2 |
| US-036 | RefundResponse | 2 | US-008 | Dev 2 |
| US-037 | RefundAsync | 3 | US-044, US-035, US-036 | Dev 2 |
| US-038 | CheckStatusRequest | 1 | Epic-01 | Dev 2 |
| US-039 | CheckStatusResponse | 2 | US-007, US-008 | Dev 2 |
| US-040 | CheckStatusAsync | 3 | US-044, US-038, US-039 | Dev 2 |
| US-041 | SettleRequest | 2 | Epic-01 | Dev 1 |
| US-042 | SettleResponse | 2 | US-008 | Dev 1 |
| US-043 | SettleAsync | 3 | US-044, US-041, US-042 | Dev 1 |
| US-045 | Error Mapping | 3 | US-021, US-025 | Dev 1 |
| US-046 | Transient Detection | 2 | US-021, US-024 | Dev 1 |

**Sprint Goal:** REFUND, CHECK_STATUS, SETTLE операції + error handling.

**Sprint Velocity Target:** 25 SP

**Milestone: M2 - Core Operations Complete**

---

### Sprint 3.1: 3DS + Invoice (Week 5) - PARALLEL

**Track A: 3D Secure**

| US | Назва | SP | Залежності | Виконавець |
|----|-------|-----|------------|------------|
| US-047 | Complete3DsRequest | 2 | Epic-01 | Dev 1 |
| US-048 | Complete3DsResponse | 2 | US-007, US-008 | Dev 1 |
| US-049 | Complete3DsAsync | 3 | US-044, US-047, US-048 | Dev 1 |
| US-050 | 3DS Detection | 2 | US-031 | Dev 1 |

**Track B: Invoice**

| US | Назва | SP | Залежності | Виконавець |
|----|-------|-----|------------|------------|
| US-057 | InvoiceRequest | 3 | US-006 | Dev 2 |
| US-058 | InvoiceResponse | 2 | US-008 | Dev 2 |
| US-059 | CreateInvoiceAsync | 3 | US-044, US-057, US-058 | Dev 2 |
| US-065 | Language Enum | 1 | US-001 | Dev 2 |

**Sprint Goal:** 3DS flow + Invoice creation.

**Sprint Velocity Target:** 18 SP (9 + 9)

---

### Sprint 3.2: Verify + Forms (Week 6) - PARALLEL

**Track A: Advanced Operations**

| US | Назва | SP | Залежності | Виконавець |
|----|-------|-----|------------|------------|
| US-051 | VerifyRequest | 2 | US-003, US-006 | Dev 1 |
| US-052 | VerifyResponse | 2 | US-004, US-008 | Dev 1 |
| US-053 | VerifyAsync | 3 | US-044, US-051, US-052 | Dev 1 |
| US-054 | TransactionListRequest | 2 | Epic-01 | Dev 1 |
| US-055 | TransactionListResponse | 2 | US-007 | Dev 1 |
| US-056 | GetTransactionListAsync | 3 | US-044, US-054, US-055 | Dev 1 |

**Track B: Forms**

| US | Назва | SP | Залежності | Виконавець |
|----|-------|-----|------------|------------|
| US-060 | PurchaseRequest | 3 | US-005, US-006 | Dev 2 |
| US-061 | PurchaseFormData | 2 | US-060 | Dev 2 |
| US-062 | CreatePurchaseForm | 3 | US-060, US-061, US-010 | Dev 2 |
| US-063 | HTML Generation | 2 | US-061 | Dev 2 |
| US-064 | PaymentSystem Conversion | 2 | US-002 | Dev 2 |
| US-066 | Regular Payments | 3 | US-009, US-060 | Dev 2 |

**Sprint Goal:** VERIFY, TRANSACTION_LIST, Purchase Forms, Regular Payments.

**Sprint Velocity Target:** 29 SP (14 + 15)

**Milestone: M3 - Extended Operations Complete**

---

### Sprint 4.1: Webhook Core (Week 7)

| US | Назва | SP | Залежності | Виконавець |
|----|-------|-----|------------|------------|
| US-067 | IWebhookHandler | 2 | Epic-01 | Dev 1 |
| US-068 | WebhookPayload | 3 | Epic-01 | Dev 2 |
| US-069 | WebhookResponse | 2 | Epic-01 | Dev 2 |
| US-070 | WebhookStatus Enum | 1 | US-001 | Dev 2 |
| US-071 | ParseAsync (Stream) | 3 | US-067, US-068, US-073 | Dev 1 |
| US-072 | Parse (string) | 2 | US-067, US-068, US-073 | Dev 1 |
| US-073 | Signature Validation | 3 | US-010, US-068 | Dev 1 |

**Sprint Goal:** Webhook parsing та validation.

**Sprint Velocity Target:** 16 SP

---

### Sprint 4.2: Webhook Extensions (Week 8)

| US | Назва | SP | Залежності | Виконавець |
|----|-------|-----|------------|------------|
| US-074 | CreateResponse | 3 | US-067, US-069, US-010 | Dev 1 |
| US-075 | SerializeResponse | 2 | US-067, US-069, US-026 | Dev 1 |
| US-076 | ASP.NET ParseAsync | 2 | US-067, US-071 | Dev 2 |
| US-077 | ToActionResult | 2 | US-069, US-075 | Dev 2 |
| US-078 | HandleAsync | 3 | US-076, US-074, US-077 | Dev 2 |
| US-079 | Helper Properties | 1 | US-068, US-025 | Dev 2 |

**Sprint Goal:** Complete webhook integration з ASP.NET Core.

**Sprint Velocity Target:** 13 SP

**Milestone: M4 - Webhook Integration Complete**

---

### Sprint 5.1: Builders (Week 9)

| US | Назва | SP | Залежності | Виконавець |
|----|-------|-----|------------|------------|
| US-080 | ChargeBuilder Create | 2 | US-030, US-013 | Dev 1 |
| US-081 | Order Methods | 2 | US-080 | Dev 1 |
| US-082 | Product Methods | 2 | US-080, US-006 | Dev 1 |
| US-083 | Payment Methods | 2 | US-080, US-003, US-004 | Dev 1 |
| US-084 | Client Method | 1 | US-080, US-005 | Dev 1 |
| US-085 | Callback Methods | 1 | US-080 | Dev 1 |
| US-086 | Transaction Type | 2 | US-080, US-002 | Dev 1 |
| US-087 | 3DS Methods | 1 | US-080, US-002 | Dev 1 |
| US-088 | Build Validation | 3 | US-080, US-023 | Dev 1 |
| US-089 | RefundBuilder | 3 | US-035 | Dev 2 |
| US-090 | InvoiceBuilder | 3 | US-057 | Dev 2 |
| US-091 | PurchaseFormBuilder | 3 | US-060, US-066, US-063 | Dev 2 |
| US-092 | CheckBuilder | 2 | US-038 | Dev 2 |

**Sprint Goal:** All Fluent Builders.

**Sprint Velocity Target:** 27 SP

---

### Sprint 5.2: Polish & Documentation (Week 10)

| US | Назва | SP | Залежності | Виконавець |
|----|-------|-----|------------|------------|
| US-093 | Polly Retry | 3 | US-019 | Dev 1 |
| US-094 | Polly Circuit Breaker | 3 | US-019 | Dev 1 |
| US-095 | AddWayForPayWithPolly | 2 | US-017, US-093, US-094 | Dev 1 |
| US-096 | Logging Handler | 3 | US-019 | Dev 1 |
| US-097 | XML Documentation | 5 | All | Dev 1 + Dev 2 |
| US-098 | README.md | 3 | All | Dev 2 |
| US-099 | API Reference | 3 | US-097 | Dev 2 |
| US-100 | Migration Guide | 2 | US-098 | Dev 2 |

**Sprint Goal:** Production-ready SDK з повною документацією.

**Sprint Velocity Target:** 24 SP

**Milestone: M5 - SDK Complete**

---

## 5. Паралельна робота

### 5.1 Можливості паралелізації

```
                          PARALLELIZATION OPPORTUNITIES
═══════════════════════════════════════════════════════════════════════════

Phase 1 (Week 1-2):
├─ Dev 1: US-001 → US-002 → US-007 → US-010 → US-011 → US-012 → US-013...
└─ Dev 2: ────────→ US-003 → US-004 → US-005 → US-006 → US-008 → US-009...

Phase 2 (Week 3-4):
├─ Dev 1: US-029 → US-044 → US-033 → US-034 → US-032 → US-041 → US-043...
└─ Dev 2: US-030 → US-031 → ─────────────────────→ US-035 → US-037 → US-040

Phase 3 (Week 5-6):  ** FULL PARALLEL TRACKS **
├─ Dev 1 (Track A): US-047 → US-048 → US-049 → US-050 → US-051 → US-053...
└─ Dev 2 (Track B): US-057 → US-058 → US-059 → US-060 → US-062 → US-063...

Phase 4 (Week 7-8):
├─ Dev 1: US-067 → US-073 → US-071 → US-072 → US-074 → US-075
└─ Dev 2: US-068 → US-069 → US-070 → ─────→ US-076 → US-077 → US-078

Phase 5 (Week 9-10):
├─ Dev 1: US-080 → US-081...US-088 → US-093 → US-094 → US-095 → US-096
└─ Dev 2: US-089 → US-090 → US-091 → US-092 → US-097 → US-098 → US-099

═══════════════════════════════════════════════════════════════════════════
```

### 5.2 Рекомендації щодо паралельної роботи

**DO:**
- Epic-02 (Payment Ops) можна починати одразу після Epic-01 Sprint 1.1
- Epic-04 (Invoice) та Epic-05 (Webhooks) можуть виконуватись паралельно
- Domain models (US-003 - US-009) можуть розроблятись паралельно
- Builders (US-080 - US-092) можуть розроблятись паралельно по різних типах

**DON'T:**
- Не починати Epic-03 (3DS) до завершення CHARGE operation (US-032)
- Не починати Builders до завершення відповідних Request models
- Не починати Polly integration до завершення HTTP Client (US-015-019)

### 5.3 Синхронізаційні точки

| Тиждень | Синхронізація | Учасники |
|---------|---------------|----------|
| End Week 2 | Foundation Review | All Devs |
| End Week 4 | Core Operations Demo | All Devs + QA |
| End Week 6 | Extended Ops Integration | All Devs |
| End Week 8 | Webhook + ASP.NET Test | All Devs + QA |
| End Week 10 | Final Review + Release Prep | All Team |

---

## 6. Milestones та Definition of Done

### 6.1 Milestone Overview

```
┌─────────────────────────────────────────────────────────────────────────┐
│                            MILESTONES                                    │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│  M1: Foundation Complete ────────────────────── Week 2                  │
│  ├─ Project setup done                                                  │
│  ├─ All domain models implemented                                       │
│  ├─ Signature generation working                                        │
│  ├─ DI registration working                                             │
│  ├─ Unit tests passing                                                  │
│  └─ Code coverage > 80%                                                 │
│                                                                          │
│  M2: Core Operations Complete ───────────────── Week 4                  │
│  ├─ CHARGE operation end-to-end                                         │
│  ├─ REFUND operation working                                            │
│  ├─ CHECK_STATUS operation working                                      │
│  ├─ SETTLE operation working                                            │
│  ├─ Error handling complete                                             │
│  ├─ Integration tests with mock server                                  │
│  └─ ** ALPHA RELEASE CANDIDATE **                                       │
│                                                                          │
│  M3: Extended Operations Complete ───────────── Week 6                  │
│  ├─ 3DS flow complete                                                   │
│  ├─ VERIFY (tokenization) working                                       │
│  ├─ TRANSACTION_LIST working                                            │
│  ├─ INVOICE creation working                                            │
│  ├─ Purchase forms generating                                           │
│  ├─ Regular payments configured                                         │
│  └─ Sandbox testing passed                                              │
│                                                                          │
│  M4: Webhook Integration Complete ───────────── Week 8                  │
│  ├─ Webhook parsing working                                             │
│  ├─ Signature validation secure                                         │
│  ├─ ASP.NET Core extensions done                                        │
│  ├─ Response generation correct                                         │
│  └─ ** BETA RELEASE CANDIDATE **                                        │
│                                                                          │
│  M5: SDK Complete ───────────────────────────── Week 10                 │
│  ├─ All Builders implemented                                            │
│  ├─ Polly integration done                                              │
│  ├─ Full XML documentation                                              │
│  ├─ README with examples                                                │
│  ├─ API reference generated                                             │
│  ├─ Migration guide ready                                               │
│  ├─ NuGet package prepared                                              │
│  └─ ** RELEASE CANDIDATE **                                             │
│                                                                          │
│  M6: Release ────────────────────────────────── Week 11                 │
│  ├─ Final testing complete                                              │
│  ├─ NuGet package published                                             │
│  ├─ GitHub release created                                              │
│  └─ Announcement posted                                                 │
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘
```

### 6.2 Definition of Done - Per Phase

#### Phase 1: Foundation

| Критерій | Перевірка |
|----------|-----------|
| Code Complete | Всі 28 US імплементовано |
| Unit Tests | Coverage > 80% для domain models та signature |
| Build | Multi-target build passing (net6.0, net8.0, net9.0, net10.0) |
| No Warnings | Zero compiler warnings in Release |
| Code Review | PR approved by at least 1 reviewer |
| Documentation | XML docs для всіх public types |

#### Phase 2: Core Operations

| Критерій | Перевірка |
|----------|-----------|
| Code Complete | Всі 18 US імплементовано |
| Unit Tests | Coverage > 80% для client та operations |
| Integration Tests | Mock server tests passing |
| Sandbox Test | CHARGE working in WayForPay sandbox |
| Error Handling | All error codes mapped to exceptions |
| Code Review | PR approved |

#### Phase 3: Extended Operations

| Критерій | Перевірка |
|----------|-----------|
| Code Complete | Всі 20 US (Epic-03 + Epic-04) імплементовано |
| 3DS Flow | Complete 3DS tested end-to-end |
| Tokenization | VERIFY returns valid recToken |
| Forms | HTML forms submitting correctly |
| Regular Payments | Subscription setup working |
| Code Review | PR approved |

#### Phase 4: Webhook Integration

| Критерій | Перевірка |
|----------|-----------|
| Code Complete | Всі 13 US імплементовано |
| Security | Signature validation timing-safe |
| ASP.NET | Extensions working in sample app |
| Response | Correct JSON format accepted by WayForPay |
| Idempotency | Duplicate webhooks handled |
| Code Review | PR approved |

#### Phase 5: Builders & Polish

| Критерій | Перевірка |
|----------|-----------|
| Code Complete | Всі 21 US імплементовано |
| Fluent API | All builders chainable and validated |
| Polly | Retry and circuit breaker working |
| Documentation | 100% public API coverage |
| README | Installation + all operations documented |
| NuGet | Package builds successfully |
| Final Review | Full team approval |

---

## 7. Залежності від зовнішніх ресурсів

### 7.1 WayForPay Sandbox Access

| Ресурс | Статус | Власник | Дедлайн |
|--------|--------|---------|---------|
| Sandbox Merchant Account | Required | Tech Lead | Week 1 Day 1 |
| Sandbox Secret Key | Required | Tech Lead | Week 1 Day 1 |
| Test Card Numbers | Available | WayForPay Docs | - |
| API Documentation Access | Available | Public | - |

**Тестові картки (з документації WayForPay):**

| Номер картки | Результат | Використання |
|--------------|-----------|--------------|
| 4111111111111111 | Approved | Happy path |
| 4111111111111112 | Declined (Insufficient funds) | Error handling |
| 4111111111111113 | 3DS Required | 3DS flow testing |
| 5555555555554444 | Approved (MasterCard) | Card type variety |

### 7.2 Development Environment

| Компонент | Версія | Статус |
|-----------|--------|--------|
| .NET SDK | 10.0 (latest) | Required |
| .NET SDK | 8.0, 6.0 (for multi-target) | Required |
| Visual Studio / Rider | Latest | Required |
| Git | Latest | Required |
| NuGet CLI | Latest | Required |

### 7.3 CI/CD Infrastructure

| Компонент | Призначення | Статус |
|-----------|-------------|--------|
| GitHub Actions | CI builds | To be configured Week 1 |
| NuGet.org Account | Package publishing | Required before M5 |
| Code Coverage Tool | Coverlet | Configured in project |
| Mock Server | WireMock.Net | Dev dependency |

### 7.4 Third-Party Dependencies

| Package | Version | Purpose | License |
|---------|---------|---------|---------|
| Microsoft.Extensions.Http | 6.0+ | IHttpClientFactory | MIT |
| Microsoft.Extensions.Options | 6.0+ | Options pattern | MIT |
| System.Text.Json | 6.0+ | JSON serialization | MIT |
| Polly | 8.0+ | Resilience (optional) | BSD-3 |
| xUnit | 2.5+ | Testing | Apache-2.0 |
| FluentAssertions | 6.12+ | Test assertions | Apache-2.0 |
| Moq | 4.20+ | Mocking | BSD-3 |
| WireMock.Net | 1.5+ | HTTP mocking | Apache-2.0 |

---

## 8. Ризики та мітигація

### 8.1 Матриця ризиків

```
                    IMPACT
            Low         Medium        High
        ┌───────────┬───────────┬───────────┐
  High  │           │    R3     │  R1, R2   │
        │           │           │           │
PROB    ├───────────┼───────────┼───────────┤
        │    R6     │    R4     │    R5     │
 Medium │           │           │           │
        ├───────────┼───────────┼───────────┤
        │           │    R7     │    R8     │
  Low   │           │           │           │
        └───────────┴───────────┴───────────┘
```

### 8.2 Детальний опис ризиків

#### R1: WayForPay API Changes

| Атрибут | Значення |
|---------|----------|
| **Ймовірність** | High |
| **Вплив** | High |
| **Опис** | WayForPay може змінити API без попередження |
| **Тригер** | Зміна форматів запитів/відповідей або signature алгоритму |
| **Мітигація** | 1. Abstraction layers для API communication 2. Comprehensive integration tests 3. Versioned request/response models 4. Monitor WayForPay changelog |
| **Contingency** | Hotfix release протягом 24 годин |
| **Власник** | Tech Lead |

#### R2: Signature Algorithm Complexity

| Атрибут | Значення |
|---------|----------|
| **Ймовірність** | High |
| **Вплив** | High |
| **Опис** | Неправильна реалізація signature призведе до відхилення всіх запитів |
| **Тригер** | Невірний порядок полів, encoding issues |
| **Мітигація** | 1. Reference PHP SDK implementation 2. Unit tests з відомими test vectors 3. Integration tests з sandbox |
| **Contingency** | Debug logging для signature fields |
| **Власник** | Senior Dev |

#### R3: 3DS Flow Complexity

| Атрибут | Значення |
|---------|----------|
| **Ймовірність** | High |
| **Вплив** | Medium |
| **Опис** | 3DS redirect flow складний для тестування та документування |
| **Тригер** | Browser redirects, session state, timeouts |
| **Мітигація** | 1. Detailed sequence diagrams 2. Sample ASP.NET application 3. Step-by-step documentation |
| **Contingency** | Окремий Epic для 3DS у наступній версії |
| **Власник** | Dev Team |

#### R4: Multi-Target Compatibility

| Атрибут | Значення |
|---------|----------|
| **Ймовірність** | Medium |
| **Вплив** | Medium |
| **Опис** | API differences між .NET 6/8/9/10 можуть спричинити проблеми |
| **Тригер** | Breaking changes в newer frameworks |
| **Мітигація** | 1. #if conditional compilation 2. CI builds for all targets 3. Abstraction for platform-specific code |
| **Contingency** | Drop support for problematic framework |
| **Власник** | Tech Lead |

#### R5: Sandbox Availability

| Атрибут | Значення |
|---------|----------|
| **Ймовірність** | Medium |
| **Вплив** | High |
| **Опис** | WayForPay sandbox може бути недоступний або нестабільний |
| **Тригер** | Maintenance, rate limiting, account issues |
| **Мітигація** | 1. WireMock.Net для local testing 2. Recorded responses для offline testing 3. Multiple sandbox accounts |
| **Contingency** | Proceed with mock testing, verify later |
| **Власник** | QA Lead |

#### R6: Scope Creep

| Атрибут | Значення |
|---------|----------|
| **Ймовірність** | Medium |
| **Вплив** | Low |
| **Опис** | Нові вимоги можуть з'являтись під час розробки |
| **Тригер** | Stakeholder requests, discovered requirements |
| **Мітигація** | 1. Strict change control 2. Backlog for v2 3. Clear MVP definition |
| **Contingency** | Defer to next release |
| **Власник** | Product Owner |

#### R7: Documentation Quality

| Атрибут | Значення |
|---------|----------|
| **Ймовірність** | Low |
| **Вплив** | Medium |
| **Опис** | Недостатня документація ускладнить adoption |
| **Тригер** | Time pressure, incomplete examples |
| **Мітигація** | 1. Documentation as part of DoD 2. Review by non-authors 3. Sample applications |
| **Contingency** | Community contributions post-release |
| **Власник** | Tech Writer |

#### R8: Security Vulnerabilities

| Атрибут | Значення |
|---------|----------|
| **Ймовірність** | Low |
| **Вплив** | High |
| **Опис** | Security issues в crypto або data handling |
| **Тригер** | Timing attacks, data leaks, improper validation |
| **Мітигація** | 1. Security review 2. No card data storage 3. Timing-safe comparisons 4. Input validation |
| **Contingency** | Immediate security patch |
| **Власник** | Security Lead |

### 8.3 Risk Response Actions

| Тиждень | Risk Review | Actions |
|---------|-------------|---------|
| Week 1 | R1, R2 | Verify sandbox access, test signature with known vectors |
| Week 3 | R4 | Run CI on all target frameworks |
| Week 5 | R3 | Complete 3DS flow testing |
| Week 7 | R5 | Verify all operations in sandbox |
| Week 9 | R7, R8 | Documentation review, security audit |

---

## 9. Команда та ролі

### 9.1 Рекомендований склад команди

| Роль | Кількість | Відповідальність |
|------|-----------|------------------|
| Tech Lead | 1 | Architecture decisions, code review, release management |
| Senior Developer | 1-2 | Core implementation, complex features |
| Developer | 1-2 | Feature implementation, testing |
| QA Engineer | 1 | Test strategy, integration testing, sandbox testing |
| Technical Writer | 0.5 | Documentation, README, examples |

### 9.2 RACI Matrix

| Activity | Tech Lead | Sr Dev | Dev | QA | Writer |
|----------|-----------|--------|-----|-----|--------|
| Architecture Design | A/R | C | I | I | I |
| Core Implementation | A | R | R | I | I |
| Unit Testing | A | R | R | C | I |
| Integration Testing | C | R | R | A/R | I |
| Code Review | A/R | R | C | I | I |
| Documentation | A | C | C | C | R |
| Release Management | A/R | C | I | C | I |
| Security Review | A/R | R | C | C | I |

**Legend:** R = Responsible, A = Accountable, C = Consulted, I = Informed

### 9.3 Комунікації

| Тип | Частота | Учасники | Формат |
|-----|---------|----------|--------|
| Daily Standup | Daily | Dev Team | 15 min sync |
| Sprint Planning | Bi-weekly | All | 2 hours |
| Sprint Review | Bi-weekly | All + Stakeholders | 1 hour demo |
| Sprint Retro | Bi-weekly | Dev Team | 1 hour |
| Technical Sync | Weekly | Tech Lead + Sr Devs | 30 min |
| Stakeholder Update | Weekly | Tech Lead + PO | 30 min |

---

## 10. Метрики успіху

### 10.1 Delivery Metrics

| Метрика | Target | Measurement |
|---------|--------|-------------|
| Sprint Velocity | 25-30 SP | Story points completed per sprint |
| Scope Completion | 100% | All 100 US delivered |
| On-Time Delivery | Week 11 | Release date met |
| Bug Escape Rate | < 5% | Bugs found after release |

### 10.2 Quality Metrics

| Метрика | Target | Measurement |
|---------|--------|-------------|
| Unit Test Coverage | > 80% | Lines covered / total lines |
| Integration Test Pass Rate | 100% | Passing / total tests |
| Code Review Coverage | 100% | PRs reviewed / total PRs |
| Documentation Coverage | 100% | Documented public APIs / total |

### 10.3 Technical Metrics

| Метрика | Target | Measurement |
|---------|--------|-------------|
| Build Time | < 2 min | CI build duration |
| Package Size | < 500 KB | NuGet package size |
| SDK Overhead | < 50ms | Time added to API calls |
| Memory Footprint | < 10 MB | Base memory consumption |

### 10.4 Adoption Metrics (Post-Release)

| Метрика | 1 Month | 3 Months | 6 Months |
|---------|---------|----------|----------|
| NuGet Downloads | 100+ | 500+ | 2000+ |
| GitHub Stars | 20+ | 50+ | 100+ |
| Open Issues | < 10 | < 20 | < 30 |
| Closed Issues | > 80% | > 80% | > 80% |

---

## Appendices

### Appendix A: User Story Index

| US | Epic | Назва | SP | Статус |
|----|------|-------|-----|--------|
| US-001 | Epic-01 | Project Setup | 2 | ✅ |
| US-002 | Epic-01 | Domain Enums | 2 | ✅ |
| US-003 | Epic-01 | Card Model | 1 | ✅ |
| US-004 | Epic-01 | CardToken Model | 1 | ✅ |
| US-005 | Epic-01 | Client Model | 2 | ✅ |
| US-006 | Epic-01 | Product Model | 1 | ✅ |
| US-007 | Epic-01 | Transaction Model | 3 | ✅ |
| US-008 | Epic-01 | Reason Model | 2 | ✅ |
| US-009 | Epic-01 | RegularPaymentSettings | 2 | ✅ |
| US-010 | Epic-01 | ISignatureGenerator | 2 | ✅ |
| US-011 | Epic-01 | HmacMd5SignatureGenerator | 3 | ✅ |
| US-012 | Epic-01 | Timing-Safe Validation | 2 | ✅ |
| US-013 | Epic-01 | WayForPayOptions | 2 | ✅ |
| US-014 | Epic-01 | Options Validator | 2 | ✅ |
| US-015 | Epic-01 | HTTP Client | 3 | ✅ |
| US-016 | Epic-01 | Connection Pooling | 2 | ✅ |
| US-017 | Epic-01 | AddWayForPay (Action) | 3 | ✅ |
| US-018 | Epic-01 | AddWayForPay (IConfig) | 2 | ✅ |
| US-019 | Epic-01 | IHttpClientBuilder | 2 | ✅ |
| US-020 | Epic-01 | WayForPayException | 2 | ✅ |
| US-021 | Epic-01 | ApiException | 2 | ✅ |
| US-022 | Epic-01 | SignatureException | 2 | ✅ |
| US-023 | Epic-01 | ValidationException | 2 | ✅ |
| US-024 | Epic-01 | NetworkException | 2 | ✅ |
| US-025 | Epic-01 | ReasonCodes | 2 | ✅ |
| US-026 | Epic-01 | JSON Context | 3 | ✅ |
| US-027 | Epic-01 | UnixTimestampConverter | 2 | ✅ |
| US-028 | Epic-01 | DecimalConverter | 2 | ✅ |
| US-029 | Epic-02 | IWayForPayClient | 3 | ✅ |
| US-030 | Epic-02 | ChargeRequest | 3 | ✅ |
| US-031 | Epic-02 | ChargeResponse | 3 | ✅ |
| US-032 | Epic-02 | ChargeAsync | 5 | ✅ |
| US-033 | Epic-02 | Charge Signature | 3 | ✅ |
| US-034 | Epic-02 | Response Validation | 3 | ✅ |
| US-035 | Epic-02 | RefundRequest | 2 | ✅ |
| US-036 | Epic-02 | RefundResponse | 2 | ✅ |
| US-037 | Epic-02 | RefundAsync | 3 | ✅ |
| US-038 | Epic-02 | CheckStatusRequest | 1 | ✅ |
| US-039 | Epic-02 | CheckStatusResponse | 2 | ✅ |
| US-040 | Epic-02 | CheckStatusAsync | 3 | ✅ |
| US-041 | Epic-02 | SettleRequest | 2 | ✅ |
| US-042 | Epic-02 | SettleResponse | 2 | ✅ |
| US-043 | Epic-02 | SettleAsync | 3 | ✅ |
| US-044 | Epic-02 | WayForPayClient | 5 | ✅ |
| US-045 | Epic-02 | Error Mapping | 3 | ⏳ |
| US-046 | Epic-02 | Transient Detection | 2 | ⏳ |
| US-047 | Epic-03 | Complete3DsRequest | 2 | ✅ |
| US-048 | Epic-03 | Complete3DsResponse | 2 | ✅ |
| US-049 | Epic-03 | Complete3DsAsync | 3 | ✅ |
| US-050 | Epic-03 | 3DS Detection | 2 | ⏳ |
| US-051 | Epic-03 | VerifyRequest | 2 | ✅ |
| US-052 | Epic-03 | VerifyResponse | 2 | ✅ |
| US-053 | Epic-03 | VerifyAsync | 3 | ✅ |
| US-054 | Epic-03 | TransactionListRequest | 2 | ✅ |
| US-055 | Epic-03 | TransactionListResponse | 2 | ✅ |
| US-056 | Epic-03 | GetTransactionListAsync | 3 | ✅ |
| US-057 | Epic-04 | InvoiceRequest | 3 | ✅ |
| US-058 | Epic-04 | InvoiceResponse | 2 | ✅ |
| US-059 | Epic-04 | CreateInvoiceAsync | 3 | ✅ |
| US-060 | Epic-04 | PurchaseRequest | 3 | ✅ |
| US-061 | Epic-04 | PurchaseFormData | 2 | ⏳ |
| US-062 | Epic-04 | CreatePurchaseForm | 3 | ⏳ |
| US-063 | Epic-04 | HTML Generation | 2 | ⏳ |
| US-064 | Epic-04 | PaymentSystem Conversion | 2 | ⏳ |
| US-065 | Epic-04 | Language Enum | 1 | ✅ |
| US-066 | Epic-04 | Regular Payments | 3 | ⏳ |
| US-067 | Epic-05 | IWebhookHandler | 2 | ⏳ |
| US-068 | Epic-05 | WebhookPayload | 3 | ⏳ |
| US-069 | Epic-05 | WebhookResponse | 2 | ⏳ |
| US-070 | Epic-05 | WebhookStatus Enum | 1 | ⏳ |
| US-071 | Epic-05 | ParseAsync (Stream) | 3 | ⏳ |
| US-072 | Epic-05 | Parse (string) | 2 | ⏳ |
| US-073 | Epic-05 | Signature Validation | 3 | ⏳ |
| US-074 | Epic-05 | CreateResponse | 3 | ⏳ |
| US-075 | Epic-05 | SerializeResponse | 2 | ⏳ |
| US-076 | Epic-05 | ASP.NET ParseAsync | 2 | ⏳ |
| US-077 | Epic-05 | ToActionResult | 2 | ⏳ |
| US-078 | Epic-05 | HandleAsync | 3 | ⏳ |
| US-079 | Epic-05 | Helper Properties | 1 | ⏳ |
| US-080 | Epic-06 | ChargeBuilder Create | 2 | ⏳ |
| US-081 | Epic-06 | Order Methods | 2 | ⏳ |
| US-082 | Epic-06 | Product Methods | 2 | ⏳ |
| US-083 | Epic-06 | Payment Methods | 2 | ⏳ |
| US-084 | Epic-06 | Client Method | 1 | ⏳ |
| US-085 | Epic-06 | Callback Methods | 1 | ⏳ |
| US-086 | Epic-06 | Transaction Type | 2 | ⏳ |
| US-087 | Epic-06 | 3DS Methods | 1 | ⏳ |
| US-088 | Epic-06 | Build Validation | 3 | ⏳ |
| US-089 | Epic-06 | RefundBuilder | 3 | ⏳ |
| US-090 | Epic-06 | InvoiceBuilder | 3 | ⏳ |
| US-091 | Epic-06 | PurchaseFormBuilder | 3 | ⏳ |
| US-092 | Epic-06 | CheckBuilder | 2 | ⏳ |
| US-093 | Epic-06 | Polly Retry | 3 | ⏳ |
| US-094 | Epic-06 | Polly Circuit Breaker | 3 | ⏳ |
| US-095 | Epic-06 | AddWayForPayWithPolly | 2 | ⏳ |
| US-096 | Epic-06 | Logging Handler | 3 | ⏳ |
| US-097 | Epic-06 | XML Documentation | 5 | ⏳ |
| US-098 | Epic-06 | README.md | 3 | ⏳ |
| US-099 | Epic-06 | API Reference | 3 | ⏳ |
| US-100 | Epic-06 | Migration Guide | 2 | ⏳ |

**Total: 100 User Stories, ~233 Story Points**

**Прогрес реалізації:**
- ✅ Завершено: 56 US (~137 SP) - Phase 1, Phase 2, частина Phase 3
- ⏳ Очікує: 44 US (~96 SP)
- 📊 Загальний прогрес: ~59%

### Appendix B: Sprint Calendar

| Sprint | Dates | Phase | Story Points |
|--------|-------|-------|--------------|
| Sprint 1.1 | Week 1 | Foundation | 25 SP |
| Sprint 1.2 | Week 2 | Foundation | 33 SP |
| Sprint 2.1 | Week 3 | Core Operations | 25 SP |
| Sprint 2.2 | Week 4 | Core Operations | 25 SP |
| Sprint 3.1 | Week 5 | Extended (Parallel) | 18 SP |
| Sprint 3.2 | Week 6 | Extended (Parallel) | 29 SP |
| Sprint 4.1 | Week 7 | Webhooks | 16 SP |
| Sprint 4.2 | Week 8 | Webhooks | 13 SP |
| Sprint 5.1 | Week 9 | Builders | 27 SP |
| Sprint 5.2 | Week 10 | Polish | 24 SP |
| Release | Week 11 | - | - |

### Appendix C: Glossary

| Термін | Визначення |
|--------|------------|
| **ADR** | Architecture Decision Record |
| **AUTH** | Авторизація коштів без списання |
| **CHARGE** | Пряме списання коштів |
| **3DS** | 3D Secure - протокол автентифікації |
| **recToken** | Токен для повторних платежів |
| **Webhook** | Callback від WayForPay |
| **SP** | Story Points |
| **MVP** | Minimum Viable Product |
| **DoD** | Definition of Done |

---

## Revision History

| Версія | Дата | Автор | Зміни |
|--------|------|-------|-------|
| 1.0 | 08.01.2026 | BA Team | Initial version |
| 1.1 | 08.01.2026 | Dev Team | Phase 1 (Epic-01) завершено; Phase 2 розпочато (CHARGE, REFUND, CHECK_STATUS реалізовано); .NET 6.0 видалено з Target Frameworks |
| 1.2 | 08.01.2026 | Dev Team | Phase 2 (Epic-02) завершено: SETTLE, VOID, CreatePurchase, CreateInvoice додано; Частково Epic-04: InvoiceRequest, InvoiceResponse, PurchaseRequest перенесено до Phase 2 |
| 1.3 | 08.01.2026 | Dev Team | Phase 3 розпочато (частково завершено ~64%): Complete3DS (3D Secure), VERIFY (card tokenization), TRANSACTION_LIST додано; 9 User Stories завершено (US-047 до US-056, окрім US-050); Прогрес: 56 US (~137 SP) з 100, ~59% |
| 1.4 | 08.01.2026 | Dev Team | Phase 3 (Epic-03) завершено (100%): 3DS Detection helpers, PaymentFormBuilder, Regular payments support додано; US-050, US-061-063, US-066 завершено; Всього 14 файлів створено/оновлено; Прогрес: 63 US (~154 SP) з 100, ~66% |

---

*Документ створено для внутрішнього використання команди розробки WayForPaySDK.*
