# ADR-005: JSON Serialization Strategy

## Статус

Proposed

## Контекст

WayForPaySDK потребує надійного механізму серіалізації та десеріалізації JSON для взаємодії з WayForPay API. Це критичний компонент SDK, оскільки всі API запити та відповіді передаються у форматі JSON.

WayForPay API використовує специфічні формати даних, які потребують особливої обробки:

- **Unix timestamp для дат** — дати передаються як long (секунди з 1970-01-01)
- **Decimal без trailing zeros** — суми повинні бути без зайвих нулів (1.5, а не 1.50)
- **Масиви продуктів** — формат `productName[]`, `productPrice[]`, `productCount[]`
- **Enum як lowercase strings** — статуси та типи у нижньому регістрі

Необхідно обрати стратегію серіалізації, яка забезпечить:
- Коректну обробку всіх специфічних форматів WayForPay
- Високу продуктивність для production використання
- Сумісність з сучасними .NET deployment сценаріями (AOT, trimming)

## Критерії вибору (Decision Drivers)

- **Продуктивність** — мінімальний overhead серіалізації/десеріалізації
- **AOT compatibility** — підтримка Native AOT та trimming для сучасних deployment сценаріїв
- **Мінімальні залежності** — уникнення зовнішніх пакетів де можливо
- **Підтримка custom converters** — можливість реалізації WayForPay-специфічних форматів
- **Maintainability** — простота підтримки та оновлення коду

## Розглянуті варіанти

1. System.Text.Json з reflection
2. System.Text.Json з source generators
3. Newtonsoft.Json
4. Custom serialization

## Рішення

Обрано **System.Text.Json з source generators**, оскільки це вбудоване в .NET рішення, що забезпечує найкращу продуктивність та повну AOT сумісність без зовнішніх залежностей.

### Варіант 1: System.Text.Json з reflection

Стандартний підхід серіалізації з використанням runtime reflection для аналізу типів.

```csharp
var json = JsonSerializer.Serialize(request);
var response = JsonSerializer.Deserialize<ChargeResponse>(json);
```

**Переваги:**
- Простота використання, мінімальний boilerplate код
- Вбудований в .NET, не потребує зовнішніх залежностей
- Знайомий API для більшості .NET розробників

**Недоліки:**
- Повільніший через runtime reflection
- Не сумісний з Native AOT (PublishAot)
- Проблеми з trimming — типи можуть бути видалені лінкером
- Більше пам'яті на runtime metadata

### Варіант 2: System.Text.Json з source generators

Compile-time генерація серіалізаційного коду через C# source generators.

```csharp
[JsonSerializable(typeof(ChargeRequest))]
[JsonSerializable(typeof(ChargeResponse))]
[JsonSerializable(typeof(PurchaseRequest))]
[JsonSerializable(typeof(PurchaseResponse))]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
internal partial class WayForPayJsonContext : JsonSerializerContext { }

// Використання
var json = JsonSerializer.Serialize(request, WayForPayJsonContext.Default.ChargeRequest);
var response = JsonSerializer.Deserialize(json, WayForPayJsonContext.Default.ChargeResponse);
```

**Переваги:**
- Найкраща продуктивність — код генерується на етапі компіляції
- Повна сумісність з Native AOT та trimming
- Вбудований в .NET, не потребує зовнішніх залежностей
- Compile-time помилки замість runtime exceptions
- Менше споживання пам'яті

**Недоліки:**
- Більше boilerplate коду (атрибути для кожного типу)
- Необхідно оновлювати JsonSerializerContext при додаванні нових типів
- Трохи складніша початкова конфігурація

### Варіант 3: Newtonsoft.Json

Популярна сторонняя бібліотека Json.NET з розширеними можливостями.

```csharp
var json = JsonConvert.SerializeObject(request);
var response = JsonConvert.DeserializeObject<ChargeResponse>(json);
```

**Переваги:**
- Багатий функціонал та гнучкість
- Велика екосистема та community
- Добре документований з багатьма прикладами
- Простіша робота зі складними сценаріями

**Недоліки:**
- Зовнішня залежність, збільшує розмір пакету
- Не сумісний з Native AOT
- Повільніший за System.Text.Json
- Окрема залежність для підтримки

### Варіант 4: Custom serialization

Ручна побудова JSON рядків без використання бібліотек серіалізації.

```csharp
public string ToJson()
{
    var sb = new StringBuilder();
    sb.Append("{");
    sb.AppendFormat("\"merchantAccount\":\"{0}\",", MerchantAccount);
    sb.AppendFormat("\"amount\":{0},", Amount.ToString(CultureInfo.InvariantCulture));
    // ...
    sb.Append("}");
    return sb.ToString();
}
```

**Переваги:**
- Повний контроль над форматом
- Максимальна продуктивність для простих випадків
- Нуль залежностей

**Недоліки:**
- Значний обсяг коду для підтримки
- Висока ймовірність помилок (escaping, encoding)
- Складна підтримка та розширення
- Потрібно писати парсер для десеріалізації

## Технічні деталі реалізації

### Custom Converters

Для обробки WayForPay-специфічних форматів необхідно реалізувати custom converters:

#### UnixTimestampConverter

```csharp
public class UnixTimestampConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var timestamp = reader.GetInt64();
        return DateTimeOffset.FromUnixTimeSeconds(timestamp);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.ToUnixTimeSeconds());
    }
}
```

#### DecimalWithoutTrailingZerosConverter

```csharp
public class DecimalWithoutTrailingZerosConverter : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetDecimal();
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        // Видаляємо trailing zeros: 1.50 -> 1.5, 10.00 -> 10
        writer.WriteRawValue(value.ToString("G29", CultureInfo.InvariantCulture));
    }
}
```

#### LowercaseEnumConverter

```csharp
public class LowercaseEnumConverter<T> : JsonConverter<T> where T : struct, Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return Enum.Parse<T>(value!, ignoreCase: true);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString().ToLowerInvariant());
    }
}
```

### Source Generator Context

```csharp
[JsonSerializable(typeof(ChargeRequest))]
[JsonSerializable(typeof(ChargeResponse))]
[JsonSerializable(typeof(PurchaseRequest))]
[JsonSerializable(typeof(PurchaseResponse))]
[JsonSerializable(typeof(RefundRequest))]
[JsonSerializable(typeof(RefundResponse))]
[JsonSerializable(typeof(CheckStatusRequest))]
[JsonSerializable(typeof(CheckStatusResponse))]
[JsonSerializable(typeof(InvoiceRequest))]
[JsonSerializable(typeof(InvoiceResponse))]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    Converters = new[] { typeof(UnixTimestampConverter), typeof(DecimalWithoutTrailingZerosConverter) })]
internal partial class WayForPayJsonContext : JsonSerializerContext { }
```

### Обробка масивів продуктів

WayForPay API очікує окремі масиви для кожного атрибуту продукту:

```json
{
    "productName": ["Product 1", "Product 2"],
    "productPrice": [100.5, 200],
    "productCount": [1, 2]
}
```

Це реалізується через спеціальну модель або custom converter:

```csharp
public class ProductArraysConverter : JsonConverter<IReadOnlyList<Product>>
{
    public override IReadOnlyList<Product> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Парсинг окремих масивів та об'єднання в список Product
    }

    public override void Write(Utf8JsonWriter writer, IReadOnlyList<Product> value, JsonSerializerOptions options)
    {
        // Розбиття списку Product на окремі масиви атрибутів
    }
}
```

## Наслідки

### Позитивні

- **Максимальна продуктивність** — compile-time генерація усуває runtime overhead
- **AOT ready** — SDK буде працювати з Native AOT та trimming без додаткової конфігурації
- **Zero dependencies** — не додаємо зовнішніх залежностей для серіалізації
- **Type safety** — помилки виявляються на етапі компіляції
- **Менше пам'яті** — відсутність reflection metadata

### Негативні

- **Boilerplate** — необхідно явно реєструвати всі типи в JsonSerializerContext
- **Оновлення контексту** — при додаванні нових request/response типів потрібно оновлювати контекст
- **Складність відладки** — generated code може бути складнішим для debugging

### Нейтральні

- Custom converters потрібні незалежно від обраного підходу через специфіку WayForPay API
- Розробники повинні бути обізнані з особливостями source generators

## Посилання

- [PRD](../PRD.md) — секція 6.5 Серіалізація та мережевий протокол
- [ADR-003](ADR-003-domain-models-design.md) — дизайн доменних моделей
- [System.Text.Json Source Generation](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation)
- [WayForPay API Documentation](https://wiki.wayforpay.com/)

## Примітки

При міграції з reflection на source generators необхідно:
1. Додати атрибут `[JsonSerializable]` для кожного типу
2. Оновити виклики серіалізації для використання typed context
3. Перевірити, що всі custom converters зареєстровані в `JsonSourceGenerationOptions`
