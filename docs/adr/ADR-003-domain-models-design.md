# ADR-003: Domain Models Design

## Статус

Proposed

## Контекст

WayForPaySDK потребує набору доменних моделей для представлення даних, що передаються між клієнтським кодом та WayForPay API. Ці моделі є ключовими компонентами SDK, оскільки:

- Вони використовуються у всіх операціях (Charge, Refund, Invoice тощо)
- Серіалізуються в JSON для HTTP запитів
- Десеріалізуються з JSON відповідей API
- Повинні бути зручними для використання розробниками
- Повинні забезпечувати type safety та валідацію даних

### Моделі, що потребують дизайну

Згідно з PRD (секція 6), необхідно реалізувати наступні моделі:

| Модель | Призначення | Ключові поля |
|--------|-------------|--------------|
| **Card** | Дані банківської карти | Number, ExpireMonth, ExpireYear, Cvv, Holder |
| **CardToken** | Токен для рекурентних платежів | Token, CardPan, CardType |
| **Client** | Інформація про покупця | FirstName, LastName, Email, Phone, Country, IpAddress... |
| **Product** | Товар у замовленні | Name, Price, Count |
| **Transaction** | Інформація про транзакцію | OrderReference, Amount, Currency, TransactionStatus... |
| **Reason** | Результат операції | Code, Message |
| **RegularPaymentSettings** | Налаштування підписки | Modes, Amount, DateNext, DateEnd, Count |
| **Enums** | Перелічення | TransactionType, SecureType, PaymentSystem, RegularMode |

### Обмеження

- .NET 10.0 як target framework
- Nullable reference types увімкнено
- Implicit usings увімкнено
- Необхідна сумісність з System.Text.Json
- Моделі мають підтримувати source-generated JSON serialization

## Критерії вибору (Decision Drivers)

- **Immutability** — моделі мають бути незмінними для thread safety та передбачуваності
- **JSON Serialization** — повна сумісність з System.Text.Json та source generators
- **Validation** — можливість валідації даних при створенні
- **Ergonomics** — зручність використання для розробників (readable код, IntelliSense)
- **Equality** — коректне порівняння об'єктів (для тестування, кешування)
- **With-expressions** — можливість створення модифікованих копій
- **Modern C#** — використання сучасних можливостей мови

## Розглянуті варіанти

1. Mutable classes з public setters (традиційні POCO)
2. Immutable classes з конструкторами (readonly properties)
3. C# records з required init properties
4. Records з positional syntax (positional records)

## Рішення

Обрано **Варіант 3: C# records з required init properties**, тому що цей підхід забезпечує найкращий баланс між immutability, зручністю використання, підтримкою JSON serialization та сучасними можливостями C#.

### Варіант 1: Mutable classes з public setters

```csharp
public class Card
{
    public string Number { get; set; } = default!;
    public int ExpireMonth { get; set; }
    public int ExpireYear { get; set; }
    public string Cvv { get; set; } = default!;
    public string Holder { get; set; } = default!;
}
```

**Переваги:**

- Проста та знайома модель для більшості .NET розробників
- Легко серіалізується/десеріалізується без додаткової конфігурації
- Підтримка object initializer syntax

**Недоліки:**

- Відсутність immutability — об'єкт можна змінити після створення
- Не thread-safe — потенційні race conditions при паралельному доступі
- Необхідно вручну імплементувати Equals/GetHashCode
- Можливість створення об'єкта в невалідному стані (не всі поля ініціалізовані)
- Не підтримує with-expressions

### Варіант 2: Immutable classes з конструкторами

```csharp
public sealed class Card
{
    public string Number { get; }
    public int ExpireMonth { get; }
    public int ExpireYear { get; }
    public string Cvv { get; }
    public string Holder { get; }

    public Card(string number, int expireMonth, int expireYear, string cvv, string holder)
    {
        Number = number ?? throw new ArgumentNullException(nameof(number));
        ExpireMonth = expireMonth;
        ExpireYear = expireYear;
        Cvv = cvv ?? throw new ArgumentNullException(nameof(cvv));
        Holder = holder ?? throw new ArgumentNullException(nameof(holder));
    }
}
```

**Переваги:**

- Повна immutability після створення
- Thread-safe
- Валідація в конструкторі гарантує валідний стан

**Недоліки:**

- Громіздкий boilerplate код для кожної моделі
- Необхідно вручну імплементувати Equals/GetHashCode
- Складна десеріалізація з JSON (потрібен JsonConstructor або custom converter)
- Не підтримує with-expressions
- При додаванні нового поля потрібно змінювати конструктор (breaking change)

### Варіант 3: C# records з required init properties

```csharp
public sealed record Card
{
    public required string Number { get; init; }
    public required int ExpireMonth { get; init; }
    public required int ExpireYear { get; init; }
    public required string Cvv { get; init; }
    public required string Holder { get; init; }
}

// Використання
var card = new Card
{
    Number = "4111111111111111",
    ExpireMonth = 12,
    ExpireYear = 2025,
    Cvv = "123",
    Holder = "JOHN DOE"
};

// With-expression для створення копії
var updatedCard = card with { ExpireYear = 2026 };
```

**Переваги:**

- Immutability "з коробки" (init-only properties)
- Автоматична імплементація Equals/GetHashCode на основі значень
- Підтримка with-expressions для створення модифікованих копій
- Відмінна підтримка System.Text.Json (працює без додаткової конфігурації)
- `required` keyword забезпечує ініціалізацію обов'язкових полів
- Чистий, декларативний синтаксис
- Автоматична імплементація ToString() для debugging
- Сумісність з source-generated JSON serialization

**Недоліки:**

- Вимагає C# 11+ (доступно в .NET 7+, .NET 10 повністю підтримує)
- Валідація можлива тільки через init accessors або окремі методи
- Дещо більший overhead для equality порівняно з reference equality

### Варіант 4: Records з positional syntax

```csharp
public sealed record Card(
    string Number,
    int ExpireMonth,
    int ExpireYear,
    string Cvv,
    string Holder);

// Використання
var card = new Card("4111111111111111", 12, 2025, "123", "JOHN DOE");
```

**Переваги:**

- Максимально компактний синтаксис
- Всі переваги records (Equals, GetHashCode, with-expressions)
- Деконструкція (pattern matching)

**Недоліки:**

- Погана читабельність при багатьох параметрах (Client має 10+ полів)
- Порядок параметрів не очевидний — легко переплутати
- Складніша десеріалізація з JSON (потрібен конструктор)
- Breaking changes при додаванні параметрів
- Опціональні поля потребують default values у конструкторі

## Детальний дизайн обраного рішення

### Структура Domain моделей

```csharp
// Card.cs
public sealed record Card
{
    /// <summary>Номер карти (16 цифр)</summary>
    public required string Number { get; init; }

    /// <summary>Місяць закінчення терміну дії (1-12)</summary>
    public required int ExpireMonth { get; init; }

    /// <summary>Рік закінчення терміну дії (4 цифри)</summary>
    public required int ExpireYear { get; init; }

    /// <summary>CVV/CVC код (3-4 цифри)</summary>
    public required string Cvv { get; init; }

    /// <summary>Ім'я власника карти латиницею</summary>
    public required string Holder { get; init; }
}

// CardToken.cs
public sealed record CardToken
{
    /// <summary>Токен для повторних списань</summary>
    public required string Token { get; init; }

    /// <summary>Маскований номер карти</summary>
    public string? CardPan { get; init; }

    /// <summary>Тип карти (Visa, MasterCard)</summary>
    public string? CardType { get; init; }
}

// Client.cs
public sealed record Client
{
    /// <summary>Ідентифікатор клієнта в системі мерчанта</summary>
    public string? AccountId { get; init; }

    /// <summary>Ім'я клієнта</summary>
    public required string FirstName { get; init; }

    /// <summary>Прізвище клієнта</summary>
    public required string LastName { get; init; }

    /// <summary>Email клієнта</summary>
    public required string Email { get; init; }

    /// <summary>Телефон у міжнародному форматі</summary>
    public required string Phone { get; init; }

    /// <summary>Країна (ISO 3166-1 alpha-3)</summary>
    public string? Country { get; init; }

    /// <summary>IP адреса клієнта</summary>
    public string? IpAddress { get; init; }

    /// <summary>Адреса доставки</summary>
    public string? Address { get; init; }

    /// <summary>Місто</summary>
    public string? City { get; init; }

    /// <summary>Область/Штат</summary>
    public string? State { get; init; }

    /// <summary>Поштовий індекс</summary>
    public string? ZipCode { get; init; }
}

// Product.cs
public sealed record Product
{
    /// <summary>Назва товару</summary>
    public required string Name { get; init; }

    /// <summary>Ціна за одиницю</summary>
    public required decimal Price { get; init; }

    /// <summary>Кількість</summary>
    public required int Count { get; init; }
}

// Transaction.cs
public sealed record Transaction
{
    // Основна інформація
    public required string OrderReference { get; init; }
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }
    public required TransactionStatus TransactionStatus { get; init; }
    public TransactionType MerchantTransactionType { get; init; }

    // Часові мітки
    public required DateTimeOffset CreatedDate { get; init; }
    public DateTimeOffset? ProcessingDate { get; init; }

    // Результат
    public required int ReasonCode { get; init; }
    public required string Reason { get; init; }

    // Банківські дані
    public string? AuthCode { get; init; }
    public string? AuthTicket { get; init; }

    // Картові дані
    public string? CardPan { get; init; }
    public string? CardType { get; init; }
    public string? IssuerBankCountry { get; init; }
    public string? IssuerBankName { get; init; }

    // Рекурентні платежі
    public CardToken? RecToken { get; init; }

    // 3D Secure
    public string? D3AcsUrl { get; init; }
    public string? D3Md { get; init; }
    public string? D3Pareq { get; init; }

    // Клієнт
    public string? Email { get; init; }
    public string? Phone { get; init; }

    // Фінанси
    public PaymentSystem? PaymentSystem { get; init; }
    public decimal? Fee { get; init; }
    public decimal? BaseAmount { get; init; }
    public string? BaseCurrency { get; init; }

    // Return
    public string? ReturnUrl { get; init; }
}

// Reason.cs
public sealed record Reason
{
    public required int Code { get; init; }
    public required string Message { get; init; }

    /// <summary>Операція успішна</summary>
    public bool IsSuccess => Code == ReasonCodes.Ok;

    /// <summary>Потрібна 3DS автентифікація</summary>
    public bool Is3DsRequired => Code == ReasonCodes.Waiting3Ds;

    /// <summary>Операція в обробці</summary>
    public bool IsPending => Code == ReasonCodes.InProcessing;
}

// RegularPaymentSettings.cs
public sealed record RegularPaymentSettings
{
    /// <summary>Доступні режими періодичності</summary>
    public required IReadOnlyList<RegularMode> Modes { get; init; }

    /// <summary>Сума регулярного платежу</summary>
    public required decimal Amount { get; init; }

    /// <summary>Дата наступного платежу</summary>
    public required DateTimeOffset DateNext { get; init; }

    /// <summary>Дата закінчення (альтернатива Count)</summary>
    public DateTimeOffset? DateEnd { get; init; }

    /// <summary>Кількість платежів (альтернатива DateEnd)</summary>
    public int? Count { get; init; }

    /// <summary>Регулярний платіж активний</summary>
    public bool IsActive { get; init; } = true;
}
```

### Enums

```csharp
// TransactionType.cs
public enum TransactionType
{
    Auto,
    Sale,
    Auth
}

// SecureType.cs
public enum SecureType
{
    Auto,
    ThreeDs,
    NonThreeDs
}

// TransactionStatus.cs
public enum TransactionStatus
{
    Approved,
    Pending,
    InProcessing,
    WaitingAuthComplete,
    Declined,
    Refunded,
    Expired,
    Voided
}

// PaymentSystem.cs
[Flags]
public enum PaymentSystem
{
    None = 0,
    Card = 1 << 0,
    Privat24 = 1 << 1,
    ApplePay = 1 << 2,
    GooglePay = 1 << 3,
    MasterPass = 1 << 4,
    VisaCheckout = 1 << 5,
    PayParts = 1 << 6,
    PayPartsMono = 1 << 7,
    Credit = 1 << 8,
    QrCode = 1 << 9,

    All = Card | Privat24 | ApplePay | GooglePay | MasterPass |
          VisaCheckout | PayParts | PayPartsMono | Credit | QrCode
}

// RegularMode.cs
public enum RegularMode
{
    Once,
    Daily,
    Weekly,
    Monthly,
    Quarterly,
    Halfyearly,
    Yearly,
    Client
}
```

### Стратегія валідації

Валідація реалізується через окремий компонент `IValidator<T>` для збереження чистоти моделей:

```csharp
public interface IValidator<T>
{
    ValidationResult Validate(T instance);
}

public sealed class CardValidator : IValidator<Card>
{
    public ValidationResult Validate(Card card)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(card.Number) || card.Number.Length != 16)
            errors.Add("Card number must be 16 digits");

        if (card.ExpireMonth < 1 || card.ExpireMonth > 12)
            errors.Add("Expire month must be between 1 and 12");

        // ... інші перевірки

        return errors.Count == 0
            ? ValidationResult.Success
            : ValidationResult.Failure(errors);
    }
}
```

## Наслідки

### Позитивні

- **Thread Safety** — immutable records безпечні для паралельного доступу
- **Predictability** — об'єкти не можуть бути випадково змінені після створення
- **Testing** — value-based equality спрощує написання assertion-ів у тестах
- **JSON Support** — System.Text.Json нативно підтримує init-only properties та required members
- **IntelliSense** — IDE показує required властивості при створенні об'єкта
- **With-expressions** — легко створювати модифіковані копії без mutation
- **Modern C#** — використання сучасних можливостей мови (.NET 10)

### Негативні

- **Learning Curve** — розробники, незнайомі з records, можуть потребувати часу на адаптацію
- **Immutability Overhead** — створення нового об'єкта замість mutation (незначний performance impact)
- **Validation Separation** — валідація винесена в окремий компонент, не вбудована в модель

### Нейтральні

- **Backward Compatibility** — records з required init вимагають C# 11+ (не проблема для .NET 10)
- **Serialization Configuration** — потребує налаштування JsonSerializerContext для AOT (покривається ADR-005)

## Приклади використання

### Створення Card

```csharp
var card = new Card
{
    Number = "4111111111111111",
    ExpireMonth = 12,
    ExpireYear = 2025,
    Cvv = "123",
    Holder = "JOHN DOE"
};
```

### Створення Client з опціональними полями

```csharp
var client = new Client
{
    FirstName = "John",
    LastName = "Doe",
    Email = "john@example.com",
    Phone = "+380991234567",
    // Опціональні поля
    Country = "UKR",
    IpAddress = "192.168.1.1"
};
```

### With-expression для модифікації

```csharp
var updatedTransaction = transaction with
{
    TransactionStatus = TransactionStatus.Approved,
    ProcessingDate = DateTimeOffset.UtcNow
};
```

### Порівняння (equality)

```csharp
var card1 = new Card { Number = "4111111111111111", ... };
var card2 = new Card { Number = "4111111111111111", ... };

// Value-based equality
Assert.Equal(card1, card2); // true
Assert.True(card1 == card2); // true
```

## Посилання

- [PRD](../PRD.md) — секція 6 "Моделі даних"
- [ADR-005](ADR-005-json-serialization.md) — JSON Serialization Strategy
- [Microsoft Docs: Records](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record)
- [Microsoft Docs: Required Members](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/required)
- [System.Text.Json and Records](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/immutability)

## Примітки

- Усі domain models розміщуються в namespace `WayForPaySDK.Domain`
- Request/Response models використовують той самий підхід (records з required init)
- Для колекцій використовується `IReadOnlyList<T>` замість `List<T>` для immutability
- XML documentation обов'язкова для всіх public properties
