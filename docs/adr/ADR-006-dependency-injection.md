# ADR-006: Dependency Injection Integration

## Статус

Proposed

## Контекст

WayForPaySDK потребує інтеграції з Microsoft.Extensions.DependencyInjection для забезпечення:

- **Простоти використання** — реєстрація всіх сервісів SDK однією командою
- **Конфігурованості** — налаштування через Options pattern та IConfiguration
- **Тестованості** — можливість заміни залежностей для unit-тестування
- **Сумісності** — інтеграція з ASP.NET Core та іншими .NET застосунками

### Компоненти для реєстрації

SDK складається з наступних сервісів, які потребують реєстрації в DI контейнері:

| Інтерфейс | Реалізація | Опис | Lifetime |
|-----------|------------|------|----------|
| `IWayForPayClient` | `WayForPayClient` | Головний клієнт API | Scoped |
| `ISignatureGenerator` | `HmacMd5SignatureGenerator` | Генерація HMAC-MD5 підписів | Singleton |
| `IWebhookHandler` | `WebhookHandler` | Обробка callback-ів | Scoped |
| `WayForPayOptions` | — | Конфігурація SDK | Options pattern |

### Вимоги з PRD

| Вимога | Секція PRD | Опис |
|--------|------------|------|
| DI Integration | 5.3 | IServiceCollection extension, IHttpClientFactory |
| ServiceCollectionExtensions | 7.4 | AddWayForPay() method |
| Configuration | 8.1 | Базова конфігурація через Action<WayForPayOptions> |
| IConfiguration binding | 8.1 | Підтримка appsettings.json |

### Патерни конфігурації в .NET

Сучасні .NET бібліотеки використовують декілька підходів до реєстрації:

1. **Simple Extension** — `AddService(Action<Options>)`
2. **Builder Pattern** — `AddService().Configure().AddFeature()`
3. **Multiple Overloads** — різні перевантаження для різних сценаріїв
4. **IConfiguration Binding** — `AddService(IConfigurationSection)`

## Критерії вибору (Decision Drivers)

- **Простота базового використання** — мінімум коду для типового сценарію
- **Гнучкість конфігурації** — підтримка inline та IConfiguration налаштувань
- **Розширюваність** — можливість додавання Polly, кастомних handlers
- **Консистентність** — відповідність паттернам Microsoft.Extensions.*
- **Discoverability** — IntelliSense та документація для легкого освоєння
- **Backward Compatibility** — стабільний API для майбутніх версій

## Розглянуті варіанти

1. Single Extension Method — `AddWayForPay(Action<WayForPayOptions>)`
2. Builder Pattern — `AddWayForPay().ConfigureClient().AddPolly()`
3. Multiple Overloads — комбінація `Action<Options>` та `IConfiguration`
4. Manual Registration — лише документація без extension methods

## Рішення

Обрано **Варіант 3: Multiple Overloads з Builder Pattern**, тому що цей підхід забезпечує:
- Простоту для базових сценаріїв через `AddWayForPay(Action<Options>)`
- Гнучкість через `AddWayForPay(IConfiguration)`
- Розширюваність через Builder pattern для advanced налаштувань
- Консистентність з іншими Microsoft.Extensions.* бібліотеками

### Варіант 1: Single Extension Method

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWayForPay(
        this IServiceCollection services,
        Action<WayForPayOptions> configure)
    {
        services.Configure(configure);
        services.AddHttpClient<IWayForPayClient, WayForPayClient>();
        services.AddSingleton<ISignatureGenerator, HmacMd5SignatureGenerator>();
        services.AddScoped<IWebhookHandler, WebhookHandler>();
        return services;
    }
}

// Використання
services.AddWayForPay(options =>
{
    options.MerchantAccount = "merchant";
    options.MerchantSecretKey = "secret";
    options.MerchantDomainName = "example.com";
});
```

**Переваги:**

- Максимальна простота — один метод для всього
- Мінімум boilerplate коду
- Легко зрозуміти та використовувати

**Недоліки:**

- Відсутність підтримки IConfiguration binding
- Неможливо налаштувати HttpClient (timeout, handlers)
- Неможливо додати Polly policies
- Обмежена розширюваність

### Варіант 2: Builder Pattern

```csharp
public static class ServiceCollectionExtensions
{
    public static IWayForPayBuilder AddWayForPay(this IServiceCollection services)
    {
        return new WayForPayBuilder(services);
    }
}

public interface IWayForPayBuilder
{
    IWayForPayBuilder Configure(Action<WayForPayOptions> configure);
    IWayForPayBuilder ConfigureHttpClient(Action<HttpClient> configure);
    IWayForPayBuilder AddPollyPolicies(Action<IHttpClientBuilder> configure);
    IWayForPayBuilder AddLogging();
    IServiceCollection Build();
}

// Використання
services.AddWayForPay()
    .Configure(options =>
    {
        options.MerchantAccount = "merchant";
        options.MerchantSecretKey = "secret";
    })
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(60);
    })
    .AddPollyPolicies(builder =>
    {
        builder.AddTransientHttpErrorPolicy(p =>
            p.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1)));
    })
    .Build();
```

**Переваги:**

- Максимальна гнучкість та розширюваність
- Чітке розділення конфігурації
- Fluent API з IntelliSense підтримкою

**Недоліки:**

- Складніший для базових сценаріїв
- Потрібен виклик `.Build()` в кінці
- Більше boilerplate коду для простих випадків
- Нестандартний патерн порівняно з Microsoft.Extensions.*

### Варіант 3: Multiple Overloads з Builder Pattern

```csharp
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Додає WayForPay SDK сервіси з inline конфігурацією
    /// </summary>
    public static IHttpClientBuilder AddWayForPay(
        this IServiceCollection services,
        Action<WayForPayOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        services.Configure(configure);
        return services.AddWayForPayCore();
    }

    /// <summary>
    /// Додає WayForPay SDK сервіси з конфігурацією з IConfiguration
    /// </summary>
    public static IHttpClientBuilder AddWayForPay(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.Configure<WayForPayOptions>(configuration);
        return services.AddWayForPayCore();
    }

    /// <summary>
    /// Додає WayForPay SDK сервіси з конфігурацією з IConfigurationSection
    /// </summary>
    public static IHttpClientBuilder AddWayForPay(
        this IServiceCollection services,
        IConfigurationSection configurationSection)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configurationSection);

        services.Configure<WayForPayOptions>(configurationSection);
        return services.AddWayForPayCore();
    }

    /// <summary>
    /// Додає WayForPay SDK сервіси з Action та додатковим налаштуванням HttpClient
    /// </summary>
    public static IHttpClientBuilder AddWayForPay(
        this IServiceCollection services,
        Action<WayForPayOptions> configure,
        Action<HttpClient>? configureClient)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        services.Configure(configure);
        return services.AddWayForPayCore(configureClient);
    }

    private static IHttpClientBuilder AddWayForPayCore(
        this IServiceCollection services,
        Action<HttpClient>? configureClient = null)
    {
        // Реєстрація singleton сервісів
        services.AddSingleton<ISignatureGenerator, HmacMd5SignatureGenerator>();

        // Реєстрація scoped сервісів
        services.AddScoped<IWebhookHandler, WebhookHandler>();

        // Реєстрація HttpClient з Typed Client pattern
        var httpClientBuilder = services.AddHttpClient<IWayForPayClient, WayForPayClient>(
            (sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<WayForPayOptions>>().Value;

                client.BaseAddress = new Uri(options.UseSandbox
                    ? WayForPayEndpoints.SandboxApi
                    : WayForPayEndpoints.ProductionApi);

                client.Timeout = options.Timeout;

                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                configureClient?.Invoke(client);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1),
                MaxConnectionsPerServer = 10
            });

        return httpClientBuilder;
    }
}
```

**Переваги:**

- Простота для базових сценаріїв — `AddWayForPay(options => { ... })`
- Підтримка IConfiguration — `AddWayForPay(configuration.GetSection("WayForPay"))`
- Повертає `IHttpClientBuilder` для розширення Polly, handlers
- Консистентність з Microsoft.Extensions.* паттернами
- Типова практика в .NET екосистемі (Entity Framework, MassTransit, etc.)

**Недоліки:**

- Більше методів для підтримки
- Потрібна документація для пояснення варіантів
- Не всі можливості очевидні без документації

### Варіант 4: Manual Registration

```csharp
// Документація замість extension methods
// Program.cs
services.Configure<WayForPayOptions>(options =>
{
    options.MerchantAccount = "merchant";
    options.MerchantSecretKey = "secret";
    options.MerchantDomainName = "example.com";
});

services.AddSingleton<ISignatureGenerator, HmacMd5SignatureGenerator>();
services.AddScoped<IWebhookHandler, WebhookHandler>();

services.AddHttpClient<IWayForPayClient, WayForPayClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<WayForPayOptions>>().Value;
    client.BaseAddress = new Uri("https://api.wayforpay.com/");
    client.Timeout = options.Timeout;
});
```

**Переваги:**

- Повний контроль користувача над реєстрацією
- Немає "магії" — все явно
- Легше налагодження проблем

**Недоліки:**

- Багато boilerplate коду для кожного проекту
- Легко зробити помилку при реєстрації
- Дублювання коду між проектами
- Порушення DRY принципу
- Гірший developer experience

## Детальна імплементація

### WayForPayOptions

```csharp
/// <summary>
/// Налаштування WayForPay SDK
/// </summary>
public sealed class WayForPayOptions
{
    /// <summary>
    /// Ідентифікатор мерчанта в системі WayForPay
    /// </summary>
    public required string MerchantAccount { get; set; }

    /// <summary>
    /// Секретний ключ для підпису запитів (HMAC-MD5)
    /// </summary>
    public required string MerchantSecretKey { get; set; }

    /// <summary>
    /// Доменне ім'я мерчанта
    /// </summary>
    public required string MerchantDomainName { get; set; }

    /// <summary>
    /// Таймаут HTTP запитів (за замовчуванням 30 секунд)
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Використовувати sandbox середовище для тестування
    /// </summary>
    public bool UseSandbox { get; set; } = false;

    /// <summary>
    /// URL для отримання callback-ів (опціонально)
    /// </summary>
    public string? DefaultServiceUrl { get; set; }

    /// <summary>
    /// URL повернення після оплати (опціонально)
    /// </summary>
    public string? DefaultReturnUrl { get; set; }
}
```

### Приклади використання

#### Базова конфігурація з Action

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWayForPay(options =>
{
    options.MerchantAccount = "test_merchant";
    options.MerchantSecretKey = "flk3409refn54t54t*FNJRET";
    options.MerchantDomainName = "www.market.ua";
    options.Timeout = TimeSpan.FromSeconds(30);
    options.UseSandbox = builder.Environment.IsDevelopment();
});

var app = builder.Build();
```

#### Конфігурація з appsettings.json

```json
// appsettings.json
{
  "WayForPay": {
    "MerchantAccount": "test_merchant",
    "MerchantSecretKey": "flk3409refn54t54t*FNJRET",
    "MerchantDomainName": "www.market.ua",
    "Timeout": "00:00:30",
    "UseSandbox": false,
    "DefaultServiceUrl": "https://www.market.ua/api/payment/callback",
    "DefaultReturnUrl": "https://www.market.ua/payment/result"
  }
}
```

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWayForPay(
    builder.Configuration.GetSection("WayForPay"));

var app = builder.Build();
```

#### Розширена конфігурація з Polly

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWayForPay(options =>
{
    options.MerchantAccount = "test_merchant";
    options.MerchantSecretKey = "secret_key";
    options.MerchantDomainName = "example.com";
})
.AddTransientHttpErrorPolicy(policy =>
    policy.WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
        onRetry: (outcome, timespan, attempt, context) =>
        {
            // Логування retry спроби
        }))
.AddTransientHttpErrorPolicy(policy =>
    policy.CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 5,
        durationOfBreak: TimeSpan.FromSeconds(30)));

var app = builder.Build();
```

#### Конфігурація з кастомним HttpMessageHandler

```csharp
// Program.cs
builder.Services.AddWayForPay(options =>
{
    options.MerchantAccount = "test_merchant";
    options.MerchantSecretKey = "secret_key";
    options.MerchantDomainName = "example.com";
})
.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
{
    PooledConnectionLifetime = TimeSpan.FromMinutes(5),
    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
    MaxConnectionsPerServer = 20,
    EnableMultipleHttp2Connections = true
})
.AddHttpMessageHandler<LoggingDelegatingHandler>();
```

#### Конфігурація з User Secrets (Development)

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    // User Secrets для development
    builder.Services.AddWayForPay(options =>
    {
        options.MerchantAccount = builder.Configuration["WayForPay:MerchantAccount"]!;
        options.MerchantSecretKey = builder.Configuration["WayForPay:MerchantSecretKey"]!;
        options.MerchantDomainName = builder.Configuration["WayForPay:MerchantDomainName"]!;
        options.UseSandbox = true;
    });
}
else
{
    // Production з appsettings
    builder.Services.AddWayForPay(
        builder.Configuration.GetSection("WayForPay"));
}
```

#### Конфігурація з Azure Key Vault

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Azure Key Vault інтеграція
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{builder.Configuration["KeyVault:Name"]}.vault.azure.net/"),
    new DefaultAzureCredential());

builder.Services.AddWayForPay(options =>
{
    options.MerchantAccount = builder.Configuration["WayForPay-MerchantAccount"]!;
    options.MerchantSecretKey = builder.Configuration["WayForPay-MerchantSecretKey"]!;
    options.MerchantDomainName = builder.Configuration["WayForPay-MerchantDomainName"]!;
});
```

### Структура файлів

```
WayForPaySDK/
├── Extensions/
│   ├── ServiceCollectionExtensions.cs
│   └── HttpClientBuilderExtensions.cs
├── Options/
│   ├── WayForPayOptions.cs
│   └── WayForPayOptionsValidator.cs
└── Constants/
    └── WayForPayEndpoints.cs
```

### Валідація Options

```csharp
/// <summary>
/// Валідатор для WayForPayOptions
/// </summary>
public sealed class WayForPayOptionsValidator : IValidateOptions<WayForPayOptions>
{
    public ValidateOptionsResult Validate(string? name, WayForPayOptions options)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.MerchantAccount))
        {
            errors.Add($"{nameof(options.MerchantAccount)} is required");
        }

        if (string.IsNullOrWhiteSpace(options.MerchantSecretKey))
        {
            errors.Add($"{nameof(options.MerchantSecretKey)} is required");
        }

        if (string.IsNullOrWhiteSpace(options.MerchantDomainName))
        {
            errors.Add($"{nameof(options.MerchantDomainName)} is required");
        }

        if (options.Timeout <= TimeSpan.Zero)
        {
            errors.Add($"{nameof(options.Timeout)} must be positive");
        }

        return errors.Count > 0
            ? ValidateOptionsResult.Fail(errors)
            : ValidateOptionsResult.Success;
    }
}
```

### Реєстрація валідатора

```csharp
private static IHttpClientBuilder AddWayForPayCore(
    this IServiceCollection services,
    Action<HttpClient>? configureClient = null)
{
    // Валідація options при старті
    services.AddSingleton<IValidateOptions<WayForPayOptions>, WayForPayOptionsValidator>();

    // Eager validation
    services.AddOptions<WayForPayOptions>()
        .ValidateOnStart();

    // ... решта реєстрації
}
```

## Наслідки

### Позитивні

- **Developer Experience** — простий старт з `AddWayForPay(options => { ... })`
- **Flexibility** — підтримка IConfiguration для production сценаріїв
- **Extensibility** — IHttpClientBuilder дозволяє додавати Polly, logging, metrics
- **Consistency** — відповідність паттернам Microsoft.Extensions.*
- **Testability** — легка заміна сервісів через DI для тестування
- **Validation** — вбудована валідація options при старті застосунку
- **Security** — підтримка User Secrets, Azure Key Vault для credentials

### Негативні

- **Multiple Methods** — потрібна документація для пояснення різних overloads
- **Learning Curve** — розробники мають знати IHttpClientBuilder pattern
- **Dependency** — обов'язкова залежність від Microsoft.Extensions.DependencyInjection

### Нейтральні

- **Options Pattern** — стандартний .NET підхід, але потребує розуміння
- **IHttpClientBuilder Return** — дозволяє розширення, але може бути незрозумілим для початківців

## Порівняльна таблиця

| Критерій | Single Method | Builder | Multiple Overloads | Manual |
|----------|---------------|---------|-------------------|--------|
| Простота базового використання | :white_check_mark: | :x: | :white_check_mark: | :x: |
| IConfiguration підтримка | :x: | :white_check_mark: | :white_check_mark: | :white_check_mark: |
| Polly інтеграція | :x: | :white_check_mark: | :white_check_mark: | :white_check_mark: |
| Консистентність з .NET | :white_check_mark: | :x: | :white_check_mark: | :white_check_mark: |
| Мінімум boilerplate | :white_check_mark: | :x: | :white_check_mark: | :x: |
| IntelliSense | :white_check_mark: | :white_check_mark: | :white_check_mark: | :x: |
| Розширюваність | :x: | :white_check_mark: | :white_check_mark: | :white_check_mark: |

## Посилання

- [PRD](../PRD.md) — секція 5.3 (DI Integration), секція 7.4 (ServiceCollectionExtensions), секція 8.1 (Базова конфігурація)
- [ADR-001](ADR-001-http-client-strategy.md) — HTTP Client Strategy (IHttpClientFactory)
- [Microsoft Docs: Options pattern](https://learn.microsoft.com/en-us/dotnet/core/extensions/options)
- [Microsoft Docs: IHttpClientFactory](https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory)
- [Microsoft Docs: Configuration](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration)

## Примітки

- `IHttpClientBuilder` повертається для можливості додавання Polly policies та кастомних handlers
- Options validation виконується eager (при старті) для швидкого виявлення помилок конфігурації
- Для production рекомендується використовувати IConfiguration з зовнішніх джерел (Azure Key Vault, AWS Secrets Manager) для credentials
- `UseSandbox` автоматично перемикає base URL між production та sandbox endpoints
- Рекомендований lifetime для `IWayForPayClient` — Scoped, оскільки HttpClient управляється через IHttpClientFactory
