# ADR-004: Error Handling Strategy

## Статус

Proposed

## Контекст

WayForPaySDK як бібліотека для інтеграції з платіжною системою повинен надавати чіткий та передбачуваний механізм обробки помилок. Помилки можуть виникати на різних рівнях:

### Типи помилок

| Тип помилки | Джерело | Приклад |
|-------------|---------|---------|
| **Мережеві помилки** | HTTP клієнт | Timeout, Connection refused, DNS failure |
| **API помилки** | WayForPay API | Reason codes (1101=Invalid merchant, 1104=Insufficient funds) |
| **Помилки валідації** | SDK | Невалідний номер карти, відсутні обов'язкові поля |
| **Помилки підпису** | SDK | Невалідний signature у відповіді від WayForPay |
| **Помилки конфігурації** | SDK | Відсутній MerchantSecretKey, невалідний endpoint |

### Reason Codes від WayForPay (PRD секція 3.2)

```
1100 - Ok (успішно)
1101 - Invalid merchant data
1102 - Invalid signature
1104 - Insufficient funds
1105 - Order already paid
1108 - Invalid card data
1109 - Invalid CVV
1110 - Card expired
1112 - 3DS required
1130 - Transaction declined
1131 - Merchant blocked
1132 - Invalid amount
1133 - Currency not allowed
```

### Вимоги

Згідно з PRD (секція 5.1), SDK повинен мати папку `Exceptions/` з наступними класами:
- `WayForPayException` (базовий)
- `ApiException`
- `SignatureException`
- `ValidationException`
- `TimeoutException`

## Критерії вибору (Decision Drivers)

- **Чітке розмежування типів помилок** — різні типи помилок потребують різної обробки
- **Достатня інформація для діагностики** — помилка повинна містити всю необхідну інформацію для debugging
- **Узгодженість з .NET conventions** — стандартні практики екосистеми .NET
- **Можливість retry** — transient errors повинні бути ідентифіковані для реалізації retry policy
- **Type safety** — компілятор повинен допомагати обробляти помилки
- **Простота використання** — зручний API для розробників

## Розглянуті варіанти

1. **Exception-based** — ієрархія виключень для кожного типу помилки
2. **Result pattern** — `Result<T, TError>` без виключень
3. **Nullable returns** — `null` для помилок
4. **Hybrid approach** — Exceptions для неочікуваних + Result для очікуваних

## Рішення

Обрано **Варіант 1: Exception-based з ієрархією виключень**, тому що:

1. Це стандартний підхід у .NET SDK бібліотеках
2. Забезпечує чітку семантику для кожного типу помилки
3. Дозволяє catch по конкретному типу виключення
4. Узгоджується з очікуваннями .NET розробників
5. Підтримується async/await без додаткових обгорток

### Варіант 1: Exception-based з ієрархією виключень

```csharp
// Ієрархія виключень
WayForPayException (base)
├── ApiException (contains ReasonCode, Reason)
├── SignatureException (ExpectedSignature, ActualSignature)
├── ValidationException (IReadOnlyList<ValidationError>)
├── NetworkException (IsTransient)
└── ConfigurationException

// Використання
try
{
    var response = await client.ChargeAsync(request);
}
catch (SignatureException ex)
{
    logger.LogError("Signature mismatch: expected {Expected}, got {Actual}",
        ex.ExpectedSignature, ex.ActualSignature);
}
catch (ApiException ex) when (ex.ReasonCode == ReasonCodes.InsufficientFunds)
{
    return PaymentResult.InsufficientFunds();
}
catch (NetworkException ex) when (ex.IsTransient)
{
    // Retry logic
}
catch (WayForPayException ex)
{
    logger.LogError(ex, "Payment failed");
}
```

**Переваги:**

- Стандартний .NET підхід, знайомий розробникам
- Чітка семантика — виключення означає "щось пішло не так"
- Селективний catch по типу виключення
- Exception filters (`when`) для тонкої обробки
- Повна інформація в stack trace для debugging
- Працює нативно з async/await

**Недоліки:**

- Performance overhead при throw (незначний для I/O операцій)
- Може призвести до "exception-driven flow" якщо зловживати
- Caller може забути обробити виключення

### Варіант 2: Result pattern (Result<T, TError>)

```csharp
public readonly struct Result<T, TError>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public TError Error { get; }
}

// Використання
var result = await client.ChargeAsync(request);
if (result.IsSuccess)
{
    ProcessPayment(result.Value);
}
else
{
    HandleError(result.Error);
}
```

**Переваги:**

- Explicit error handling — компілятор "нагадує" про можливість помилки
- Кращий performance (немає throw)
- Функціональний стиль програмування

**Недоліки:**

- Не є стандартним для .NET SDK бібліотек
- Потребує додаткової бібліотеки або власної імплементації
- Ускладнює async/await (потрібні спеціальні методи)
- Розробники очікують exceptions від платіжних SDK
- Не відповідає PRD вимогам (секція 5.1)

### Варіант 3: Nullable returns

```csharp
public async Task<ChargeResponse?> ChargeAsync(ChargeRequest request);

// Використання
var response = await client.ChargeAsync(request);
if (response is null)
{
    // Що саме пішло не так? Невідомо!
}
```

**Переваги:**

- Простота імплементації
- Мінімальний overhead

**Недоліки:**

- Втрата інформації про помилку
- Неможливо розрізнити типи помилок
- Не відповідає семантиці — `null` не є помилкою
- Порушує принцип "fail fast"

### Варіант 4: Hybrid approach

```csharp
// Exceptions для неочікуваних помилок
// Result для очікуваних бізнес-помилок

public async Task<Result<ChargeResponse, PaymentError>> ChargeAsync(...)
{
    // Throws: NetworkException, ConfigurationException
    // Returns error: InsufficientFunds, CardDeclined
}
```

**Переваги:**

- Розділення unexpected/expected errors
- Гнучкість обробки

**Недоліки:**

- Складність API — два різних механізми
- Неоднозначність — що є expected, а що unexpected?
- Збільшена когнітивна складність
- Непослідовний API

## Детальний дизайн обраного рішення

### Ієрархія виключень

```csharp
// WayForPayException.cs - базове виключення
namespace WayForPaySDK.Exceptions;

/// <summary>
/// Базове виключення для всіх помилок WayForPaySDK.
/// </summary>
public class WayForPayException : Exception
{
    /// <summary>
    /// Час виникнення помилки (UTC).
    /// </summary>
    public DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Унікальний ідентифікатор помилки для трейсингу.
    /// </summary>
    public string ErrorId { get; }

    public WayForPayException(string message)
        : base(message)
    {
        Timestamp = DateTimeOffset.UtcNow;
        ErrorId = Guid.NewGuid().ToString("N")[..8];
    }

    public WayForPayException(string message, Exception innerException)
        : base(message, innerException)
    {
        Timestamp = DateTimeOffset.UtcNow;
        ErrorId = Guid.NewGuid().ToString("N")[..8];
    }
}
```

```csharp
// ApiException.cs - помилки від WayForPay API
namespace WayForPaySDK.Exceptions;

/// <summary>
/// Виключення для помилок, повернутих WayForPay API.
/// </summary>
public sealed class ApiException : WayForPayException
{
    /// <summary>
    /// Код помилки від WayForPay (reason code).
    /// </summary>
    public int ReasonCode { get; }

    /// <summary>
    /// Текстовий опис помилки від WayForPay.
    /// </summary>
    public string Reason { get; }

    /// <summary>
    /// Номер замовлення, для якого виникла помилка.
    /// </summary>
    public string? OrderReference { get; }

    /// <summary>
    /// Чи є помилка тимчасовою (можна повторити запит).
    /// </summary>
    public bool IsTransient => ReasonCode is
        ReasonCodes.InProcessing or
        ReasonCodes.SystemError or
        ReasonCodes.TemporaryUnavailable;

    /// <summary>
    /// Чи потрібна 3DS автентифікація.
    /// </summary>
    public bool Requires3Ds => ReasonCode == ReasonCodes.Waiting3Ds;

    public ApiException(int reasonCode, string reason, string? orderReference = null)
        : base($"WayForPay API error: {reason} (code: {reasonCode})")
    {
        ReasonCode = reasonCode;
        Reason = reason;
        OrderReference = orderReference;
    }
}
```

```csharp
// SignatureException.cs - помилки підпису
namespace WayForPaySDK.Exceptions;

/// <summary>
/// Виключення при невідповідності підпису відповіді.
/// Може свідчити про man-in-the-middle атаку.
/// </summary>
public sealed class SignatureException : WayForPayException
{
    /// <summary>
    /// Очікуваний підпис (розрахований на основі відповіді).
    /// </summary>
    public string ExpectedSignature { get; }

    /// <summary>
    /// Фактичний підпис з відповіді WayForPay.
    /// </summary>
    public string ActualSignature { get; }

    /// <summary>
    /// Номер замовлення, для якого виникла невідповідність.
    /// </summary>
    public string? OrderReference { get; }

    public SignatureException(
        string expectedSignature,
        string actualSignature,
        string? orderReference = null)
        : base($"Response signature mismatch. This may indicate a security issue.")
    {
        ExpectedSignature = expectedSignature;
        ActualSignature = actualSignature;
        OrderReference = orderReference;
    }
}
```

```csharp
// ValidationException.cs - помилки валідації
namespace WayForPaySDK.Exceptions;

/// <summary>
/// Виключення при невалідних даних у запиті.
/// </summary>
public sealed class ValidationException : WayForPayException
{
    /// <summary>
    /// Список помилок валідації.
    /// </summary>
    public IReadOnlyList<ValidationError> Errors { get; }

    public ValidationException(IReadOnlyList<ValidationError> errors)
        : base(BuildMessage(errors))
    {
        Errors = errors;
    }

    public ValidationException(string fieldName, string errorMessage)
        : this([new ValidationError(fieldName, errorMessage)])
    {
    }

    private static string BuildMessage(IReadOnlyList<ValidationError> errors)
    {
        if (errors.Count == 1)
            return $"Validation failed: {errors[0].FieldName} - {errors[0].ErrorMessage}";

        return $"Validation failed with {errors.Count} errors: " +
               string.Join("; ", errors.Select(e => $"{e.FieldName}: {e.ErrorMessage}"));
    }
}

/// <summary>
/// Деталі помилки валідації для конкретного поля.
/// </summary>
public sealed record ValidationError(string FieldName, string ErrorMessage);
```

```csharp
// NetworkException.cs - мережеві помилки
namespace WayForPaySDK.Exceptions;

/// <summary>
/// Виключення при мережевих помилках (timeout, connection refused, тощо).
/// </summary>
public sealed class NetworkException : WayForPayException
{
    /// <summary>
    /// Чи є помилка тимчасовою (варто повторити запит).
    /// </summary>
    public bool IsTransient { get; }

    /// <summary>
    /// HTTP статус код (якщо доступний).
    /// </summary>
    public int? HttpStatusCode { get; }

    /// <summary>
    /// URL, до якого виконувався запит.
    /// </summary>
    public string? RequestUrl { get; }

    /// <summary>
    /// Тривалість запиту до помилки.
    /// </summary>
    public TimeSpan? Elapsed { get; }

    public NetworkException(
        string message,
        bool isTransient,
        Exception? innerException = null,
        int? httpStatusCode = null,
        string? requestUrl = null,
        TimeSpan? elapsed = null)
        : base(message, innerException!)
    {
        IsTransient = isTransient;
        HttpStatusCode = httpStatusCode;
        RequestUrl = requestUrl;
        Elapsed = elapsed;
    }

    /// <summary>
    /// Створює NetworkException для timeout.
    /// </summary>
    public static NetworkException Timeout(TimeSpan timeout, string? requestUrl = null) =>
        new($"Request timed out after {timeout.TotalSeconds:F1}s",
            isTransient: true,
            requestUrl: requestUrl,
            elapsed: timeout);

    /// <summary>
    /// Створює NetworkException для connection refused.
    /// </summary>
    public static NetworkException ConnectionRefused(string? requestUrl, Exception? inner = null) =>
        new("Connection refused by server",
            isTransient: true,
            innerException: inner,
            requestUrl: requestUrl);

    /// <summary>
    /// Створює NetworkException для HTTP помилки.
    /// </summary>
    public static NetworkException HttpError(int statusCode, string? requestUrl = null) =>
        new($"HTTP error {statusCode}",
            isTransient: statusCode >= 500,
            httpStatusCode: statusCode,
            requestUrl: requestUrl);
}
```

```csharp
// ConfigurationException.cs - помилки конфігурації
namespace WayForPaySDK.Exceptions;

/// <summary>
/// Виключення при невалідній конфігурації SDK.
/// </summary>
public sealed class ConfigurationException : WayForPayException
{
    /// <summary>
    /// Назва параметра конфігурації з помилкою.
    /// </summary>
    public string ParameterName { get; }

    /// <summary>
    /// Поточне значення параметра (якщо не є секретом).
    /// </summary>
    public string? CurrentValue { get; }

    public ConfigurationException(string parameterName, string message, string? currentValue = null)
        : base($"Configuration error for '{parameterName}': {message}")
    {
        ParameterName = parameterName;
        CurrentValue = currentValue;
    }

    /// <summary>
    /// Створює виключення для відсутнього обов'язкового параметра.
    /// </summary>
    public static ConfigurationException MissingRequired(string parameterName) =>
        new(parameterName, "This parameter is required but was not provided");

    /// <summary>
    /// Створює виключення для невалідного значення.
    /// </summary>
    public static ConfigurationException InvalidValue(string parameterName, string? value, string reason) =>
        new(parameterName, reason, value);
}
```

### Константи Reason Codes

```csharp
// ReasonCodes.cs
namespace WayForPaySDK.Constants;

/// <summary>
/// Коди результатів операцій WayForPay API.
/// </summary>
public static class ReasonCodes
{
    /// <summary>Операція успішна</summary>
    public const int Ok = 1100;

    /// <summary>Невалідні дані мерчанта</summary>
    public const int InvalidMerchant = 1101;

    /// <summary>Невалідний підпис</summary>
    public const int InvalidSignature = 1102;

    /// <summary>Недостатньо коштів</summary>
    public const int InsufficientFunds = 1104;

    /// <summary>Замовлення вже оплачено</summary>
    public const int OrderAlreadyPaid = 1105;

    /// <summary>Невалідні дані карти</summary>
    public const int InvalidCardData = 1108;

    /// <summary>Невалідний CVV</summary>
    public const int InvalidCvv = 1109;

    /// <summary>Термін дії карти закінчився</summary>
    public const int CardExpired = 1110;

    /// <summary>Потрібна 3DS автентифікація</summary>
    public const int Waiting3Ds = 1112;

    /// <summary>Транзакція відхилена</summary>
    public const int TransactionDeclined = 1130;

    /// <summary>Мерчант заблокований</summary>
    public const int MerchantBlocked = 1131;

    /// <summary>Невалідна сума</summary>
    public const int InvalidAmount = 1132;

    /// <summary>Валюта не підтримується</summary>
    public const int CurrencyNotAllowed = 1133;

    /// <summary>В обробці</summary>
    public const int InProcessing = 1001;

    /// <summary>Системна помилка</summary>
    public const int SystemError = 5000;

    /// <summary>Тимчасово недоступний</summary>
    public const int TemporaryUnavailable = 5001;

    /// <summary>
    /// Перевіряє, чи код означає успішну операцію.
    /// </summary>
    public static bool IsSuccess(int code) => code == Ok;

    /// <summary>
    /// Перевіряє, чи помилка є тимчасовою.
    /// </summary>
    public static bool IsTransient(int code) => code is InProcessing or SystemError or TemporaryUnavailable;
}
```

### Приклади використання

#### Базова обробка помилок

```csharp
public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
{
    try
    {
        var chargeRequest = BuildChargeRequest(request);
        var response = await _client.ChargeAsync(chargeRequest);

        return PaymentResult.Success(response.Transaction);
    }
    catch (ValidationException ex)
    {
        _logger.LogWarning("Validation failed: {Errors}",
            string.Join(", ", ex.Errors.Select(e => e.ErrorMessage)));
        return PaymentResult.ValidationFailed(ex.Errors);
    }
    catch (ApiException ex) when (ex.Requires3Ds)
    {
        return PaymentResult.Requires3Ds(ex.OrderReference);
    }
    catch (ApiException ex) when (ex.ReasonCode == ReasonCodes.InsufficientFunds)
    {
        return PaymentResult.InsufficientFunds();
    }
    catch (ApiException ex)
    {
        _logger.LogError("API error {ReasonCode}: {Reason}", ex.ReasonCode, ex.Reason);
        return PaymentResult.Failed(ex.Reason);
    }
    catch (SignatureException ex)
    {
        _logger.LogCritical("SECURITY: Signature mismatch for order {Order}! ErrorId: {ErrorId}",
            ex.OrderReference, ex.ErrorId);
        throw; // Re-throw security issues
    }
    catch (NetworkException ex) when (ex.IsTransient)
    {
        _logger.LogWarning("Transient network error, should retry: {Message}", ex.Message);
        throw; // Let retry policy handle it
    }
    catch (WayForPayException ex)
    {
        _logger.LogError(ex, "Payment failed. ErrorId: {ErrorId}", ex.ErrorId);
        return PaymentResult.Failed("Payment processing error");
    }
}
```

#### Інтеграція з Polly для retry

```csharp
services.AddHttpClient<IWayForPayClient, WayForPayClient>()
    .AddPolicyHandler(Policy<HttpResponseMessage>
        .Handle<NetworkException>(ex => ex.IsTransient)
        .Or<ApiException>(ex => ex.IsTransient)
        .WaitAndRetryAsync(3,
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                logger.LogWarning("Retry {RetryAttempt} after {Delay}s due to: {Error}",
                    retryAttempt, timespan.TotalSeconds, outcome.Exception?.Message);
            }));
```

#### Webhook обробка з валідацією підпису

```csharp
[HttpPost("webhook")]
public async Task<IActionResult> HandleWebhook()
{
    try
    {
        var payload = await _webhookHandler.ParseAsync(Request);
        await _orderService.UpdateStatusAsync(payload.OrderReference, payload.TransactionStatus);
        return Ok(_webhookHandler.CreateResponse(payload));
    }
    catch (SignatureException ex)
    {
        _logger.LogCritical("Webhook signature validation failed! ErrorId: {ErrorId}", ex.ErrorId);
        return BadRequest("Invalid signature");
    }
    catch (ValidationException ex)
    {
        _logger.LogWarning("Invalid webhook payload: {Errors}", ex.Errors);
        return BadRequest("Invalid payload");
    }
}
```

## Наслідки

### Позитивні

- **Стандартний .NET підхід** — відповідає очікуванням .NET розробників
- **Type-safe error handling** — можливість catch по конкретному типу
- **Багата діагностична інформація** — ErrorId, Timestamp, ReasonCode для debugging
- **Підтримка retry** — властивість `IsTransient` для ідентифікації помилок, які варто повторити
- **Інтеграція з Polly** — легка інтеграція з бібліотеками resilience
- **Exception filters** — гнучка обробка з `catch ... when`
- **Stack traces** — повна інформація про місце виникнення помилки

### Негативні

- **Performance overhead** — throw exception має overhead (незначний для I/O операцій)
- **Можливість пропустити обробку** — caller може забути catch
- **Boilerplate** — потребує try-catch блоків у calling code

### Нейтральні

- **Документація** — потрібно чітко документувати, які виключення кидає кожен метод
- **Testing** — тести повинні перевіряти правильність виключень

## Посилання

- [PRD](../PRD.md) — секція 3.2 (Reason Codes), секція 5.1 (Exceptions)
- [ADR-003](ADR-003-domain-models-design.md) — Domain Models Design
- [Microsoft Docs: Exception Handling Best Practices](https://learn.microsoft.com/en-us/dotnet/standard/exceptions/best-practices-for-exceptions)
- [Polly Resilience Library](https://github.com/App-vNext/Polly)

## Примітки

- Усі exceptions розміщуються в namespace `WayForPaySDK.Exceptions`
- `WayForPayException.ErrorId` використовується для кореляції логів
- `SignatureException` повинен логуватися як CRITICAL — може свідчити про атаку
- Для transient errors рекомендується використовувати Polly retry policies
- XML documentation обов'язкова для всіх public членів exceptions
