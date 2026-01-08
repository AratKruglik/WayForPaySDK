# Architecture Decision Records (ADR)

Цей каталог містить архітектурні рішення (ADR) для проекту WayForPaySDK.

## Що таке ADR?

**Architecture Decision Record (ADR)** — це документ, що фіксує важливе архітектурне рішення разом з контекстом та наслідками. ADR допомагають:

- Зрозуміти **чому** було прийнято певне рішення
- Відслідковувати еволюцію архітектури
- Швидко онбордити нових розробників
- Уникати повторного обговорення вже прийнятих рішень

## Формат

Використовується формат **MADR** (Markdown Any Decision Records) версії 3.0.

Шаблон: [adr-template.md](adr-template.md)

## Статуси

| Статус | Опис |
|--------|------|
| **Proposed** | Рішення запропоновано, обговорюється |
| **Accepted** | Рішення прийнято та діє |
| **Deprecated** | Рішення застаріло, не рекомендується |
| **Superseded** | Рішення замінено іншим ADR |

## Індекс ADR

### Інфраструктура

| ADR | Назва | Статус | Дата |
|-----|-------|--------|------|
| [ADR-008](ADR-008-multi-target-framework.md) | Multi-Target Framework Strategy | Proposed | 2026-01-08 |
| [ADR-001](ADR-001-http-client-strategy.md) | HTTP Client Strategy | Proposed | 2026-01-08 |
| [ADR-006](ADR-006-dependency-injection.md) | Dependency Injection Integration | Proposed | 2026-01-08 |

### Безпека та криптографія

| ADR | Назва | Статус | Дата |
|-----|-------|--------|------|
| [ADR-002](ADR-002-signature-generation.md) | Signature Generation | Proposed | 2026-01-08 |

### Моделі та серіалізація

| ADR | Назва | Статус | Дата |
|-----|-------|--------|------|
| [ADR-003](ADR-003-domain-models-design.md) | Domain Models Design | Proposed | 2026-01-08 |
| [ADR-005](ADR-005-json-serialization.md) | JSON Serialization Strategy | Proposed | 2026-01-08 |

### API Design

| ADR | Назва | Статус | Дата |
|-----|-------|--------|------|
| [ADR-004](ADR-004-error-handling.md) | Error Handling Strategy | Proposed | 2026-01-08 |
| [ADR-007](ADR-007-builder-pattern-api.md) | Builder Pattern for Requests | Proposed | 2026-01-08 |
| [ADR-009](ADR-009-webhook-handler-design.md) | Webhook Handler Design | Proposed | 2026-01-08 |

## Діаграма залежностей

```
ADR-008 (Multi-target)
    │
    ▼
ADR-003 (Domain Models)
    │
    ├──► ADR-005 (JSON Serialization)
    │
    ▼
ADR-002 (Signature Generation)
    │
    ▼
ADR-001 (HTTP Client)
    │
    ├──► ADR-004 (Error Handling)
    │
    ▼
ADR-006 (DI Integration)
    │
    ├──► ADR-007 (Builder Pattern)
    │
    ▼
ADR-009 (Webhook Handler)
```

## Пов'язані документи

- [Product Requirements Document (PRD)](../PRD.md)

## Як додати новий ADR

1. Скопіюйте [adr-template.md](adr-template.md)
2. Перейменуйте на `ADR-XXX-short-title.md`
3. Заповніть всі секції
4. Додайте до індексу в README.md
5. Створіть Pull Request для review
