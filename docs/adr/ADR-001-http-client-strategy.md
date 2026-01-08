# ADR-001: HTTP Client Strategy

## Статус

Proposed

## Контекст

WayForPaySDK потребує надійного HTTP клієнта для взаємодії з WayForPay API. Основний endpoint API:

- **URL:** `https://api.wayforpay.com/api`
- **Метод:** POST
- **Формат:** JSON

SDK повинен виконувати HTTP запити до WayForPay API для всіх операцій (Charge, Refund, Check Status, Invoice тощо). Вибір правильної стратегії HTTP клієнта критично важливий для:

- **Продуктивності** — мінімізація overhead на кожен запит
- **Надійності** — обробка transient errors, timeout-ів
- **Масштабованості** — коректна робота під навантаженням
- **Тестованості** — можливість мокування HTTP взаємодії

### Технічні обмеження

- .NET 10.0 як target framework (планується multi-target для .NET 6/7/8)
- Асинхронні операції (async/await)
- Інтеграція з DI контейнером
- Сумісність з Polly для resilience patterns

### Вимоги з PRD

| Вимога | Секція PRD | Опис |
|--------|------------|------|
| NFR-01 | 4.1 | SDK overhead < 50ms на запит |
| NFR-03 | 4.3 | Retry policy, configurable timeout, circuit breaker |
| NFR-06 | 4.6 | HttpMessageHandler заміщуваний для тестування |
| DI | 5.3 | IHttpClientFactory інтеграція |

### Відомі проблеми HttpClient

При неправильному використанні HttpClient виникають серйозні проблеми:

1. **Socket Exhaustion** — створення нового HttpClient для кожного запиту призводить до вичерпання доступних сокетів
2. **DNS Changes Not Respected** — singleton HttpClient не оновлює DNS записи
3. **Connection Pooling** — відсутність правильного управління connection pool

## Критерії вибору (Decision Drivers)

- **Connection Pooling** — ефективне повторне використання TCP з'єднань, уникнення socket exhaustion
- **DNS Refresh** — автоматичне оновлення DNS записів при зміні IP адреси сервера
- **Тестованість** — можливість заміни HttpMessageHandler для unit/integration тестів
- **DI інтеграція** — безшовна інтеграція з Microsoft.Extensions.DependencyInjection
- **Polly підтримка** — можливість додавання retry, circuit breaker, timeout policies
- **Конфігурованість** — налаштування timeout-ів, headers, base address через options pattern
- **Minimal Overhead** — мінімальний вплив на latency запитів

## Розглянуті варіанти

1. Raw HttpClient — `new HttpClient()` для кожного запиту
2. Singleton HttpClient — один екземпляр на весь час життя застосунку
3. IHttpClientFactory з Named Client — фабрика з іменованим клієнтом
4. IHttpClientFactory з Typed Client — фабрика з типізованим клієнтом

## Рішення

Обрано **Варіант 4: IHttpClientFactory з Typed Client**, тому що цей підхід є рекомендованим Microsoft best practice для .NET застосунків, забезпечує автоматичний DNS refresh, connection pooling, легке тестування та нативну інтеграцію з DI та Polly.

### Варіант 1: Raw HttpClient

```csharp
public class WayForPayClient : IWayForPayClient
{
    public async Task<ChargeResponse> ChargeAsync(ChargeRequest request, CancellationToken ct)
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://api.wayforpay.com/");
        client.Timeout = TimeSpan.FromSeconds(30);

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("api", content, ct);
        // ...
    }
}
```

**Переваги:**

- Простота імплементації — немає залежностей
- Явний контроль над lifecycle

**Недоліки:**

- **Socket Exhaustion** — кожен запит створює нове TCP з'єднання, сокети залишаються в стані TIME_WAIT
- Відсутність connection pooling — значний overhead на встановлення з'єднання
- Повна відсутність resilience patterns
- Важко тестувати — HttpClient важко мокувати без wrapper-ів
- Порушення NFR-01 (performance) та NFR-03 (reliability)

### Варіант 2: Singleton HttpClient

```csharp
public class WayForPayClient : IWayForPayClient
{
    private static readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://api.wayforpay.com/"),
        Timeout = TimeSpan.FromSeconds(30)
    };

    public async Task<ChargeResponse> ChargeAsync(ChargeRequest request, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api", content, ct);
        // ...
    }
}
```

**Переваги:**

- Вирішує проблему socket exhaustion
- Повторне використання TCP з'єднань (connection pooling)
- Простота імплементації

**Недоліки:**

- **DNS не оновлюється** — якщо IP адреса api.wayforpay.com зміниться, клієнт продовжить використовувати старий IP
- Важко конфігурувати — static instance не інтегрується з DI
- Складне тестування — потрібні workarounds для мокування
- Неможливо використовувати різні налаштування для різних scenarios
- Часткове порушення NFR-03 та NFR-06

### Варіант 3: IHttpClientFactory з Named Client

```csharp
// Registration
services.AddHttpClient("WayForPay", client =>
{
    client.BaseAddress = new Uri("https://api.wayforpay.com/");
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
})
.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
{
    PooledConnectionLifetime = TimeSpan.FromMinutes(2)
})
.AddPolicyHandler(GetRetryPolicy());

// Usage
public class WayForPayClient : IWayForPayClient
{
    private readonly IHttpClientFactory _factory;

    public WayForPayClient(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task<ChargeResponse> ChargeAsync(ChargeRequest request, CancellationToken ct)
    {
        using var client = _factory.CreateClient("WayForPay");
        // ...
    }
}
```

**Переваги:**

- Автоматичний DNS refresh через PooledConnectionLifetime
- Connection pooling з коректним lifecycle management
- Інтеграція з DI контейнером
- Підтримка Polly policies
- Тестованість через HttpMessageHandler

**Недоліки:**

- Magic string "WayForPay" — можлива помилка при написанні
- Клієнт не типізований — IHttpClientFactory повертає generic HttpClient
- Менш зручний API для споживачів SDK
- Потрібна додаткова обгортка для типізації

### Варіант 4: IHttpClientFactory з Typed Client

```csharp
// WayForPayHttpClient.cs - internal typed client
internal sealed class WayForPayHttpClient
{
    private readonly HttpClient _httpClient;

    public WayForPayHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : class
        where TResponse : class
    {
        var json = JsonSerializer.Serialize(request, WayForPayJsonContext.Default.Options);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await _httpClient.PostAsync("api", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync<TResponse>(
            responseStream,
            WayForPayJsonContext.Default.Options,
            cancellationToken)
            ?? throw new WayForPayException("Empty response from API");
    }
}

// Registration in ServiceCollectionExtensions.cs
public static IServiceCollection AddWayForPay(
    this IServiceCollection services,
    Action<WayForPayOptions> configure)
{
    services.Configure(configure);

    services.AddHttpClient<WayForPayHttpClient>((sp, client) =>
    {
        var options = sp.GetRequiredService<IOptions<WayForPayOptions>>().Value;

        client.BaseAddress = new Uri(options.UseSandbox
            ? "https://api.sandbox.wayforpay.com/"
            : "https://api.wayforpay.com/");
        client.Timeout = options.Timeout;
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    })
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(2),
        PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1),
        MaxConnectionsPerServer = 10
    })
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetTimeoutPolicy());

    services.AddScoped<IWayForPayClient, WayForPayClient>();

    return services;
}

private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

private static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
{
    return Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(30));
}
```

**Переваги:**

- **Connection Pooling** — SocketsHttpHandler забезпечує ефективне повторне використання з'єднань
- **DNS Refresh** — PooledConnectionLifetime автоматично оновлює з'єднання, враховуючи зміни DNS
- **Type Safety** — типізований клієнт, IntelliSense підтримка
- **DI інтеграція** — нативна підтримка Microsoft.Extensions.DependencyInjection
- **Polly підтримка** — легке додавання retry, circuit breaker, timeout policies
- **Тестованість** — можливість заміни HttpMessageHandler через DI
- **Конфігурованість** — інтеграція з Options pattern
- **Best Practices** — рекомендований Microsoft підхід для .NET 6+

**Недоліки:**

- Складніша початкова конфігурація порівняно з простим HttpClient
- Залежність від Microsoft.Extensions.Http
- Трохи більший boilerplate код

## Детальна імплементація

### Структура файлів

```
WayForPaySDK/
├── Client/
│   ├── IWayForPayClient.cs
│   ├── WayForPayClient.cs
│   └── WayForPayClientOptions.cs
├── Http/
│   └── WayForPayHttpClient.cs
├── Extensions/
│   └── ServiceCollectionExtensions.cs
└── Policies/
    └── PolicyConfiguration.cs
```

### WayForPayOptions

```csharp
public sealed class WayForPayOptions
{
    public required string MerchantAccount { get; set; }
    public required string MerchantSecretKey { get; set; }
    public required string MerchantDomainName { get; set; }

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public bool UseSandbox { get; set; } = false;

    // Retry configuration
    public int MaxRetryAttempts { get; set; } = 3;
    public TimeSpan InitialRetryDelay { get; set; } = TimeSpan.FromSeconds(1);

    // Circuit breaker
    public bool EnableCircuitBreaker { get; set; } = true;
    public int CircuitBreakerThreshold { get; set; } = 5;
    public TimeSpan CircuitBreakerDuration { get; set; } = TimeSpan.FromSeconds(30);
}
```

### Приклад реєстрації та використання

```csharp
// Program.cs або Startup.cs
var builder = WebApplication.CreateBuilder(args);

// Варіант 1: Inline конфігурація
builder.Services.AddWayForPay(options =>
{
    options.MerchantAccount = "test_merchant";
    options.MerchantSecretKey = "secret_key";
    options.MerchantDomainName = "example.com";
    options.Timeout = TimeSpan.FromSeconds(30);
    options.UseSandbox = builder.Environment.IsDevelopment();
});

// Варіант 2: Конфігурація з appsettings.json
builder.Services.AddWayForPay(
    builder.Configuration.GetSection("WayForPay"));

var app = builder.Build();

// Використання в сервісі
public class PaymentService
{
    private readonly IWayForPayClient _client;

    public PaymentService(IWayForPayClient client)
    {
        _client = client;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(Order order)
    {
        var request = new ChargeRequest
        {
            // ... параметри запиту
        };

        var response = await _client.ChargeAsync(request);

        return response.IsSuccess
            ? PaymentResult.Success(response.Transaction)
            : PaymentResult.Failed(response.Reason);
    }
}
```

### Приклад тестування

```csharp
public class WayForPayClientTests
{
    [Fact]
    public async Task ChargeAsync_WithValidRequest_ReturnsApprovedResponse()
    {
        // Arrange
        var expectedResponse = new ChargeResponse
        {
            Transaction = new Transaction { TransactionStatus = TransactionStatus.Approved },
            Reason = new Reason { Code = 1100, Message = "Ok" }
        };

        var mockHandler = new MockHttpMessageHandler(request =>
        {
            var json = JsonSerializer.Serialize(expectedResponse);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        });

        var httpClient = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri("https://api.wayforpay.com/")
        };

        var wayForPayHttpClient = new WayForPayHttpClient(httpClient);
        var signatureGenerator = new HmacMd5SignatureGenerator();
        var options = Options.Create(new WayForPayOptions
        {
            MerchantAccount = "test",
            MerchantSecretKey = "secret",
            MerchantDomainName = "example.com"
        });

        var client = new WayForPayClient(wayForPayHttpClient, signatureGenerator, options);

        // Act
        var response = await client.ChargeAsync(new ChargeRequest { /* ... */ });

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.Transaction.TransactionStatus.Should().Be(TransactionStatus.Approved);
    }
}
```

### Конфігурація Polly Policies

```csharp
public static class PolicyConfiguration
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(WayForPayOptions options)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(
                options.MaxRetryAttempts,
                retryAttempt => TimeSpan.FromSeconds(
                    options.InitialRetryDelay.TotalSeconds * Math.Pow(2, retryAttempt - 1)),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    // Логування retry спроби
                });
    }

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(WayForPayOptions options)
    {
        if (!options.EnableCircuitBreaker)
            return Policy.NoOpAsync<HttpResponseMessage>();

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                options.CircuitBreakerThreshold,
                options.CircuitBreakerDuration,
                onBreak: (outcome, timespan) =>
                {
                    // Логування відкриття circuit breaker
                },
                onReset: () =>
                {
                    // Логування закриття circuit breaker
                });
    }
}
```

## Наслідки

### Позитивні

- **Socket Exhaustion Prevention** — IHttpClientFactory автоматично управляє lifecycle HttpMessageHandler-ів
- **DNS Refresh** — PooledConnectionLifetime забезпечує періодичне оновлення DNS записів
- **Performance** — connection pooling мінімізує overhead на встановлення з'єднань (< 50ms overhead)
- **Resilience** — вбудована підтримка Polly для retry, timeout, circuit breaker
- **Testability** — легке мокування через HttpMessageHandler
- **Maintainability** — чіткий розподіл відповідальностей, типізований API
- **Observability** — можливість додавання logging та metrics через delegating handlers

### Негативні

- **Dependency** — обов'язкова залежність від Microsoft.Extensions.Http
- **Complexity** — складніша початкова конфігурація для простих сценаріїв
- **Learning Curve** — розробники мають розуміти IHttpClientFactory pattern

### Нейтральні

- **DI Requirement** — SDK прив'язаний до Microsoft.Extensions.DependencyInjection (стандарт для .NET)
- **Polly Dependency** — опціональна залежність для resilience patterns

## Порівняльна таблиця

| Критерій | Raw HttpClient | Singleton | Named Client | Typed Client |
|----------|----------------|-----------|--------------|--------------|
| Socket Exhaustion | :x: | :white_check_mark: | :white_check_mark: | :white_check_mark: |
| DNS Refresh | :x: | :x: | :white_check_mark: | :white_check_mark: |
| Connection Pooling | :x: | :white_check_mark: | :white_check_mark: | :white_check_mark: |
| DI Integration | :x: | :x: | :white_check_mark: | :white_check_mark: |
| Type Safety | N/A | N/A | :x: | :white_check_mark: |
| Polly Support | :x: | :x: | :white_check_mark: | :white_check_mark: |
| Testability | :x: | :x: | :white_check_mark: | :white_check_mark: |
| Configuration | :x: | :x: | :white_check_mark: | :white_check_mark: |

## Посилання

- [PRD](../PRD.md) — секція 4.1 (NFR-01), секція 4.3 (NFR-03), секція 5.3 (DI)
- [Microsoft Docs: IHttpClientFactory](https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory)
- [Microsoft Docs: Make HTTP requests with IHttpClientFactory](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests)
- [Polly Documentation](https://github.com/App-vNext/Polly)
- [Steve Gordon: HttpClientFactory in ASP.NET Core](https://www.stevejgordon.co.uk/httpclientfactory-aspnetcore-outgoing-request-middleware-pipeline-delegatinghandlers)

## Примітки

- PooledConnectionLifetime встановлено на 2 хвилини як компроміс між DNS refresh та performance
- MaxConnectionsPerServer = 10 обмежує кількість одночасних з'єднань до WayForPay API
- Circuit breaker відкривається після 5 послідовних помилок на 30 секунд
- Retry policy використовує exponential backoff: 1s, 2s, 4s
- Для production рекомендується налаштувати logging через delegating handler для моніторингу HTTP запитів
