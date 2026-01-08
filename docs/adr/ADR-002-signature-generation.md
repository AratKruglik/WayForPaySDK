# ADR-002: Signature Generation

## Статус

Proposed

## Контекст

WayForPay API вимагає криптографічний підпис для кожного запиту та відповіді. Це є критичним компонентом безпеки SDK, оскільки:

- Підпис автентифікує мерчанта перед WayForPay
- Підпис підтверджує цілісність даних запиту
- WayForPay підписує відповіді, що дозволяє валідувати їх автентичність
- Webhook callbacks також підписуються і потребують валідації

### Алгоритм підпису WayForPay

WayForPay використовує HMAC-MD5 для генерації підпису:

```
signature = HMAC_MD5(secretKey, data)
```

де `data` — це конкатенація значень полів, розділених символом `;`.

### Порядок полів для різних операцій

Порядок полів є критичним і відрізняється для кожної операції:

**CHARGE Request:**
```
merchantAccount;merchantDomainName;orderReference;orderDate;amount;currency;productName[];productCount[];productPrice[]
```

**CHARGE Response:**
```
merchantAccount;orderReference;amount;currency;authCode;cardPan;transactionStatus;reasonCode
```

**REFUND Request:**
```
merchantAccount;orderReference;amount;currency
```

**Webhook Callback:**
```
merchantAccount;orderReference;amount;currency;authCode;cardPan;transactionStatus;reasonCode
```

### Особливості обробки масивів

Масиви (productName[], productCount[], productPrice[]) "розгортаються" та кожен елемент з'єднується через `;`:

```
productName[0];productName[1];productCount[0];productCount[1];productPrice[0];productPrice[1]
```

### Обмеження

- Згідно з PRD (секція 4.2 NFR-02): обов'язковий HMAC-MD5
- Згідно з PRD (секція 5.1): код розміщується у `Crypto/` папці
- Секретний ключ не повинен зберігатися в логах або exception messages
- Підпис має генеруватися для запитів, валідуватися для відповідей та webhooks

## Критерії вибору (Decision Drivers)

- **Single Responsibility Principle** — компонент підпису має відповідати лише за криптографію
- **Testability** — можливість тестування окремо від HTTP клієнта
- **Reusability** — повторне використання для request, response та webhook підписів
- **Security** — безпечна обробка секретних ключів (без логування, proper disposal)
- **Extensibility** — можливість заміни алгоритму у майбутньому (якщо WayForPay змінить вимоги)
- **DI Integration** — сумісність з Microsoft.Extensions.DependencyInjection

## Розглянуті варіанти

1. Вбудована логіка підпису в WayForPayClient
2. Окремий сервіс ISignatureGenerator через інтерфейс
3. Static utility клас з статичними методами
4. Extension methods для string/byte[]

## Рішення

Обрано **Варіант 2: Окремий сервіс ISignatureGenerator**, тому що цей підхід найкраще відповідає принципам SOLID, забезпечує легке тестування та дозволяє повторно використовувати логіку підпису у різних контекстах (requests, responses, webhooks).

### Варіант 1: Вбудована логіка підпису в WayForPayClient

```csharp
public class WayForPayClient : IWayForPayClient
{
    private readonly string _secretKey;

    public async Task<ChargeResponse> ChargeAsync(ChargeRequest request, CancellationToken ct)
    {
        var fields = BuildSignatureFields(request);
        var signature = ComputeHmacMd5(fields, _secretKey);
        // ... send request
    }

    private string ComputeHmacMd5(string[] fields, string secret)
    {
        var data = string.Join(";", fields);
        using var hmac = new HMACMD5(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
```

**Переваги:**

- Простота імплементації — все в одному місці
- Менше абстракцій та файлів
- Немає overhead від DI resolution

**Недоліки:**

- Порушення Single Responsibility Principle — клієнт відповідає і за HTTP, і за криптографію
- Складне unit-тестування — неможливо протестувати підпис окремо від HTTP
- Дублювання коду — WebhookHandler потребуватиме тієї ж логіки
- Неможливість заміни алгоритму без модифікації клієнта
- Утруднене тестування з mock-підписами

### Варіант 2: Окремий сервіс ISignatureGenerator

```csharp
public interface ISignatureGenerator
{
    string GenerateSignature(IEnumerable<string> fields);
    bool ValidateSignature(string signature, IEnumerable<string> fields);
}

public sealed class HmacMd5SignatureGenerator : ISignatureGenerator
{
    private readonly byte[] _secretKeyBytes;

    public HmacMd5SignatureGenerator(string secretKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(secretKey);
        _secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
    }

    public string GenerateSignature(IEnumerable<string> fields)
    {
        var data = string.Join(";", fields);
        using var hmac = new HMACMD5(_secretKeyBytes);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public bool ValidateSignature(string signature, IEnumerable<string> fields)
    {
        var expected = GenerateSignature(fields);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(signature.ToLowerInvariant()),
            Encoding.UTF8.GetBytes(expected));
    }
}
```

**Переваги:**

- Дотримання Single Responsibility Principle
- Легке unit-тестування окремо від HTTP
- Повторне використання в WayForPayClient та WebhookHandler
- Можливість заміни реалізації через DI
- Mock-friendly для інтеграційних тестів
- Timing-safe порівняння підписів (захист від timing attacks)

**Недоліки:**

- Додаткова абстракція та файл
- Незначний overhead від DI resolution
- Потребує налаштування DI

### Варіант 3: Static utility клас

```csharp
public static class SignatureHelper
{
    public static string GenerateSignature(IEnumerable<string> fields, string secretKey)
    {
        var data = string.Join(";", fields);
        using var hmac = new HMACMD5(Encoding.UTF8.GetBytes(secretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public static bool ValidateSignature(string signature, IEnumerable<string> fields, string secretKey)
    {
        var expected = GenerateSignature(fields, secretKey);
        return signature.Equals(expected, StringComparison.OrdinalIgnoreCase);
    }
}
```

**Переваги:**

- Простота використання — не потребує DI
- Немає overhead від створення об'єктів
- Легко викликати з будь-якого місця

**Недоліки:**

- Неможливість мокування для unit-тестів
- Секретний ключ передається як параметр — вищий ризик логування
- Порушення принципу Dependency Inversion
- Неможливість заміни реалізації без зміни коду
- Створення нового HMACMD5 на кожен виклик

### Варіант 4: Extension methods

```csharp
public static class SignatureExtensions
{
    public static string ToWayForPaySignature(this IEnumerable<string> fields, string secretKey)
    {
        var data = string.Join(";", fields);
        using var hmac = new HMACMD5(Encoding.UTF8.GetBytes(secretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public static bool IsValidSignature(this string signature, IEnumerable<string> fields, string secretKey)
    {
        var expected = fields.ToWayForPaySignature(secretKey);
        return signature.Equals(expected, StringComparison.OrdinalIgnoreCase);
    }
}

// Використання
var signature = new[] { "merchant", "order123", "100" }.ToWayForPaySignature(secretKey);
```

**Переваги:**

- Fluent API — зручний синтаксис виклику
- Discoverability через IntelliSense на колекціях

**Недоліки:**

- Ті самі недоліки що й у static класу
- "Забруднення" namespace IEnumerable<string>
- Неочевидне місцезнаходження логіки
- Складність з версіонуванням API

## Детальний дизайн обраного рішення

### Структура файлів

```
WayForPaySDK/
├── Crypto/
│   ├── ISignatureGenerator.cs
│   ├── HmacMd5SignatureGenerator.cs
│   └── SignatureValidator.cs
```

### Інтерфейс ISignatureGenerator

```csharp
namespace WayForPaySDK.Crypto;

/// <summary>
/// Генератор криптографічних підписів для WayForPay API.
/// </summary>
public interface ISignatureGenerator
{
    /// <summary>
    /// Генерує HMAC-MD5 підпис для набору полів.
    /// </summary>
    /// <param name="fields">Поля для підпису у правильному порядку.</param>
    /// <returns>Hex-encoded підпис у нижньому регістрі.</returns>
    string GenerateSignature(IEnumerable<string> fields);

    /// <summary>
    /// Валідує підпис відповіді або webhook від WayForPay.
    /// </summary>
    /// <param name="signature">Підпис для перевірки.</param>
    /// <param name="fields">Поля, що були підписані.</param>
    /// <returns>true якщо підпис валідний; інакше false.</returns>
    bool ValidateSignature(string signature, IEnumerable<string> fields);
}
```

### Реалізація HmacMd5SignatureGenerator

```csharp
namespace WayForPaySDK.Crypto;

using System.Security.Cryptography;
using System.Text;

/// <summary>
/// HMAC-MD5 реалізація генератора підписів для WayForPay.
/// </summary>
public sealed class HmacMd5SignatureGenerator : ISignatureGenerator, IDisposable
{
    private readonly byte[] _secretKeyBytes;
    private bool _disposed;

    /// <summary>
    /// Створює новий екземпляр генератора підписів.
    /// </summary>
    /// <param name="secretKey">Секретний ключ мерчанта.</param>
    /// <exception cref="ArgumentException">Якщо secretKey null або порожній.</exception>
    public HmacMd5SignatureGenerator(string secretKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(secretKey);
        _secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
    }

    /// <inheritdoc />
    public string GenerateSignature(IEnumerable<string> fields)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(fields);

        var data = string.Join(";", fields);

        using var hmac = new HMACMD5(_secretKeyBytes);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));

        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    /// <inheritdoc />
    public bool ValidateSignature(string signature, IEnumerable<string> fields)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(signature);
        ArgumentNullException.ThrowIfNull(fields);

        var expected = GenerateSignature(fields);

        // Timing-safe comparison для захисту від timing attacks
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(signature.ToLowerInvariant()),
            Encoding.UTF8.GetBytes(expected));
    }

    /// <summary>
    /// Звільняє ресурси та очищує секретний ключ з пам'яті.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;

        // Очищення секретного ключа з пам'яті
        CryptographicOperations.ZeroMemory(_secretKeyBytes);
        _disposed = true;
    }
}
```

### SignatureValidator для webhook

```csharp
namespace WayForPaySDK.Crypto;

/// <summary>
/// Валідатор підписів для webhook callbacks від WayForPay.
/// </summary>
public sealed class SignatureValidator
{
    private readonly ISignatureGenerator _signatureGenerator;

    public SignatureValidator(ISignatureGenerator signatureGenerator)
    {
        _signatureGenerator = signatureGenerator ?? throw new ArgumentNullException(nameof(signatureGenerator));
    }

    /// <summary>
    /// Валідує підпис webhook callback.
    /// </summary>
    /// <param name="payload">Payload від WayForPay.</param>
    /// <returns>true якщо підпис валідний.</returns>
    /// <exception cref="SignatureException">Якщо підпис невалідний.</exception>
    public bool ValidateWebhookSignature(WebhookPayload payload)
    {
        ArgumentNullException.ThrowIfNull(payload);

        var fields = new[]
        {
            payload.MerchantAccount,
            payload.OrderReference,
            payload.Amount.ToString("F2", CultureInfo.InvariantCulture),
            payload.Currency,
            payload.AuthCode ?? string.Empty,
            payload.CardPan ?? string.Empty,
            payload.TransactionStatus,
            payload.ReasonCode.ToString()
        };

        return _signatureGenerator.ValidateSignature(payload.MerchantSignature, fields);
    }
}
```

### Інтеграція з DI

```csharp
namespace WayForPaySDK.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWayForPay(
        this IServiceCollection services,
        Action<WayForPayOptions> configure)
    {
        services.Configure(configure);

        // Реєстрація signature generator як singleton
        services.AddSingleton<ISignatureGenerator>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<WayForPayOptions>>().Value;
            return new HmacMd5SignatureGenerator(options.MerchantSecretKey);
        });

        services.AddSingleton<SignatureValidator>();

        // ... інші реєстрації

        return services;
    }
}
```

### Використання в WayForPayClient

```csharp
public sealed class WayForPayClient : IWayForPayClient
{
    private readonly HttpClient _httpClient;
    private readonly ISignatureGenerator _signatureGenerator;
    private readonly WayForPayOptions _options;

    public WayForPayClient(
        HttpClient httpClient,
        ISignatureGenerator signatureGenerator,
        IOptions<WayForPayOptions> options)
    {
        _httpClient = httpClient;
        _signatureGenerator = signatureGenerator;
        _options = options.Value;
    }

    public async Task<ChargeResponse> ChargeAsync(ChargeRequest request, CancellationToken ct = default)
    {
        // Побудова полів для підпису
        var signatureFields = BuildChargeSignatureFields(request);

        // Генерація підпису
        var signature = _signatureGenerator.GenerateSignature(signatureFields);

        // Створення підписаного запиту
        var signedRequest = request with { MerchantSignature = signature };

        // Відправка запиту
        var response = await SendRequestAsync<ChargeResponse>(signedRequest, ct);

        // Валідація підпису відповіді
        var responseFields = BuildChargeResponseSignatureFields(response);
        if (!_signatureGenerator.ValidateSignature(response.MerchantSignature, responseFields))
        {
            throw new SignatureException("Invalid response signature from WayForPay");
        }

        return response;
    }

    private static IEnumerable<string> BuildChargeSignatureFields(ChargeRequest request)
    {
        yield return request.MerchantAccount;
        yield return request.MerchantDomainName;
        yield return request.OrderReference;
        yield return request.OrderDate.ToUnixTimeSeconds().ToString();
        yield return request.Amount.ToString("F2", CultureInfo.InvariantCulture);
        yield return request.Currency;

        // Розгортання масивів продуктів
        foreach (var product in request.Products)
            yield return product.Name;
        foreach (var product in request.Products)
            yield return product.Count.ToString();
        foreach (var product in request.Products)
            yield return product.Price.ToString("F2", CultureInfo.InvariantCulture);
    }
}
```

### Приклад Unit-тесту

```csharp
public class HmacMd5SignatureGeneratorTests
{
    private const string TestSecretKey = "test_secret_key_123";

    [Fact]
    public void GenerateSignature_WithValidFields_ReturnsCorrectHash()
    {
        // Arrange
        using var sut = new HmacMd5SignatureGenerator(TestSecretKey);
        var fields = new[] { "merchant", "example.com", "ORDER123", "1704700000", "100.00", "UAH", "Product1", "1", "100.00" };

        // Act
        var signature = sut.GenerateSignature(fields);

        // Assert
        signature.Should().NotBeNullOrEmpty();
        signature.Should().MatchRegex("^[a-f0-9]{32}$"); // MD5 produces 32 hex chars
    }

    [Fact]
    public void GenerateSignature_SameInput_ReturnsSameOutput()
    {
        // Arrange
        using var sut = new HmacMd5SignatureGenerator(TestSecretKey);
        var fields = new[] { "merchant", "ORDER123", "100.00", "UAH" };

        // Act
        var signature1 = sut.GenerateSignature(fields);
        var signature2 = sut.GenerateSignature(fields);

        // Assert
        signature1.Should().Be(signature2);
    }

    [Fact]
    public void ValidateSignature_WithValidSignature_ReturnsTrue()
    {
        // Arrange
        using var sut = new HmacMd5SignatureGenerator(TestSecretKey);
        var fields = new[] { "merchant", "ORDER123", "100.00", "UAH" };
        var signature = sut.GenerateSignature(fields);

        // Act
        var isValid = sut.ValidateSignature(signature, fields);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateSignature_WithInvalidSignature_ReturnsFalse()
    {
        // Arrange
        using var sut = new HmacMd5SignatureGenerator(TestSecretKey);
        var fields = new[] { "merchant", "ORDER123", "100.00", "UAH" };
        var invalidSignature = "00000000000000000000000000000000";

        // Act
        var isValid = sut.ValidateSignature(invalidSignature, fields);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateSignature_CaseInsensitive_ReturnsTrue()
    {
        // Arrange
        using var sut = new HmacMd5SignatureGenerator(TestSecretKey);
        var fields = new[] { "merchant", "ORDER123" };
        var signature = sut.GenerateSignature(fields);
        var upperSignature = signature.ToUpperInvariant();

        // Act
        var isValid = sut.ValidateSignature(upperSignature, fields);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithNullSecretKey_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => new HmacMd5SignatureGenerator(null!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithEmptySecretKey_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => new HmacMd5SignatureGenerator(string.Empty);
        act.Should().Throw<ArgumentException>();
    }
}
```

## Наслідки

### Позитивні

- **Single Responsibility** — компонент підпису відповідає лише за криптографію
- **Testability** — легке unit-тестування з ізольованим компонентом
- **Reusability** — використовується в WayForPayClient, WebhookHandler, SignatureValidator
- **Security** — timing-safe порівняння, очищення ключа при disposal
- **Extensibility** — можливість додати інші алгоритми (якщо WayForPay змінить вимоги)
- **DI Integration** — природна інтеграція з Microsoft.Extensions.DependencyInjection
- **Mocking** — легко мокувати для інтеграційних тестів клієнта

### Негативні

- **Додаткова абстракція** — більше файлів та інтерфейсів
- **DI залежність** — потребує налаштування контейнера
- **Learning curve** — розробники мають розуміти архітектуру

### Нейтральні

- **Lifetime management** — ISignatureGenerator реєструється як Singleton для перформансу
- **Thread safety** — HmacMd5SignatureGenerator є thread-safe (створює новий HMACMD5 на кожен виклик)

## Посилання

- [PRD](../prd.md) — секція 4.2 NFR-02 (Безпека), секція 5.1 (Crypto/)
- [WayForPay API Documentation](https://wiki.wayforpay.com/) — опис алгоритму підпису
- [ADR-003](ADR-003-domain-models-design.md) — дизайн domain моделей
- [Microsoft Docs: HMACMD5](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.hmacmd5)
- [Microsoft Docs: CryptographicOperations.FixedTimeEquals](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.cryptographicoperations.fixedtimeequals)

## Примітки

- MD5 вважається криптографічно слабким, але WayForPay вимагає саме HMAC-MD5
- Секретний ключ ніколи не логується та не включається в exception messages
- Для production рекомендується зберігати секретний ключ у Azure Key Vault або аналогу
- При disposal HmacMd5SignatureGenerator очищує ключ з пам'яті через CryptographicOperations.ZeroMemory
