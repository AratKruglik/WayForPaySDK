# WayForPaySDK — User Stories Backlog

Цей каталог містить User Stories для імплементації WayForPaySDK.

## Огляд

**WayForPaySDK** — це .NET SDK для інтеграції з платіжною системою WayForPay, українським провайдером онлайн-платежів.

| Метрика | Значення |
|---------|----------|
| **Всього Epic-ів** | 6 |
| **Всього User Stories** | 100 |
| **Приблизний обсяг** | ~175 Story Points |
| **Target Framework** | .NET 6.0, 8.0, 9.0, 10.0 |

---

## Epic-и

### Порядок імплементації

```
┌──────────────────────────────────────────────────────────────┐
│                     Epic-01: Core Infrastructure              │
│  Domain models, Signature, HTTP Client, DI, Exceptions, JSON │
│                        (28 US, ~58 SP)                        │
└────────────────────────────┬─────────────────────────────────┘
                             │
         ┌───────────────────┼───────────────────┐
         │                   │                   │
         ▼                   ▼                   ▼
┌─────────────────┐  ┌──────────────────┐  ┌─────────────────┐
│   Epic-02       │  │     Epic-04      │  │    Epic-05      │
│   Payment Ops   │  │ Invoice & Forms  │  │    Webhooks     │
│  (18 US, ~50 SP)│  │  (10 US, ~24 SP) │  │ (13 US, ~29 SP) │
└────────┬────────┘  └────────┬─────────┘  └─────────────────┘
         │                    │
         ▼                    │
┌─────────────────┐           │
│    Epic-03      │           │
│  3DS & Advanced │           │
│ (10 US, ~23 SP) │           │
└────────┬────────┘           │
         │                    │
         └────────────────────┤
                              │
                              ▼
              ┌──────────────────────────┐
              │       Epic-06            │
              │   Builders & Polish      │
              │    (21 US, ~49 SP)       │
              └──────────────────────────┘
```

### Індекс Epic-ів

| Epic | Назва | User Stories | Story Points | Статус |
|------|-------|--------------|--------------|--------|
| [Epic-01](Epic-01-core-infrastructure.md) | Core Infrastructure | US-001 — US-028 | ~58 SP | 📋 Draft |
| [Epic-02](Epic-02-payment-operations.md) | Payment Operations | US-029 — US-046 | ~50 SP | 📋 Draft |
| [Epic-03](Epic-03-3ds-and-advanced.md) | 3D Secure & Advanced | US-047 — US-056 | ~23 SP | 📋 Draft |
| [Epic-04](Epic-04-invoice-and-forms.md) | Invoice & Forms | US-057 — US-066 | ~24 SP | 📋 Draft |
| [Epic-05](Epic-05-webhook-integration.md) | Webhook Integration | US-067 — US-079 | ~29 SP | 📋 Draft |
| [Epic-06](Epic-06-builders-and-polish.md) | Builders & Polish | US-080 — US-100 | ~49 SP | 📋 Draft |

---

## Статуси

| Статус | Значення |
|--------|----------|
| 📋 Draft | User Story визначена, готова до рефайнменту |
| 🔍 Ready | Деталізована, оцінена, готова до Sprint |
| 🚧 In Progress | В роботі |
| ✅ Done | Завершена |

---

## Story Points

| Розмір | Points | Типовий час | Приклад |
|--------|--------|-------------|---------|
| **XS** | 1 | < 2 години | Простий enum, модель з 2-3 полями |
| **S** | 2 | 2-4 години | Базова модель, helper метод |
| **M** | 3 | 4-8 годин | Імплементація API операції |
| **L** | 5 | 1-2 дні | Складний компонент (WayForPayClient) |
| **XL** | 8 | 2-3 дні | Комплексна функціональність |

---

## Трейсабільність

### PRD → User Stories

| PRD Section | Epic | User Stories |
|-------------|------|--------------|
| FR-01: CHARGE | Epic-02 | US-030 — US-034 |
| FR-02: REFUND | Epic-02 | US-035 — US-037 |
| FR-03: CHECK_STATUS | Epic-02 | US-038 — US-040 |
| FR-04: SETTLE | Epic-02 | US-041 — US-043 |
| FR-05: COMPLETE_3DS | Epic-03 | US-047 — US-050 |
| FR-06: TRANSACTION_LIST | Epic-03 | US-054 — US-056 |
| FR-07: INVOICE | Epic-04 | US-057 — US-059 |
| FR-08: PURCHASE | Epic-04 | US-060 — US-063 |
| FR-09: VERIFY | Epic-03 | US-051 — US-053 |
| FR-10: Webhook | Epic-05 | US-067 — US-079 |
| FR-11: Payment Systems | Epic-04 | US-064 — US-065 |
| FR-12: Recurring | Epic-04 | US-066 |

### ADR → User Stories

| ADR | Epic | User Stories |
|-----|------|--------------|
| ADR-001: HTTP Client | Epic-01 | US-015, US-016 |
| ADR-002: Signature | Epic-01 | US-010 — US-012 |
| ADR-003: Domain Models | Epic-01 | US-003 — US-009 |
| ADR-004: Error Handling | Epic-01, Epic-02 | US-020 — US-024, US-045 — US-046 |
| ADR-005: JSON | Epic-01 | US-026 — US-028 |
| ADR-006: DI | Epic-01 | US-013, US-014, US-017 — US-019 |
| ADR-007: Builder Pattern | Epic-06 | US-080 — US-092 |
| ADR-008: Multi-target | Epic-01 | US-001 |
| ADR-009: Webhook Handler | Epic-05 | US-067 — US-079 |

---

## Quick Start

### Для продукт-менеджера

1. Починайте з [Epic-01](Epic-01-core-infrastructure.md) — це фундамент
2. Переглядайте User Stories, валідуйте acceptance criteria
3. Оцініть пріоритети між Epic-02/03/04/05 (можуть виконуватись паралельно)
4. Epic-06 потребує завершення попередніх

### Для розробника

1. Кожна User Story має:
   - As/I want/So that опис
   - Given/When/Then acceptance criteria
   - Технічні нотатки з деталями реалізації
   - Залежності від інших US
   - Референси на PRD та ADR

2. Перед початком роботи:
   - Прочитайте пов'язані ADR
   - Перевірте залежності
   - Переконайтесь у готовності попередніх US

---

## Пов'язані документи

- [Product Requirements Document (PRD)](../PRD.md)
- [Architecture Decision Records (ADR)](../adr/README.md)

---

## Changelog

| Дата | Версія | Зміни |
|------|--------|-------|
| 2026-01-08 | 1.0 | Початкова версія з 100 User Stories |
