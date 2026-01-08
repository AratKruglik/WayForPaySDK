# ADR-007: Builder Pattern for Requests

## Статус

Proposed

## Контекст

WayForPaySDK потребує зручного API для створення запитів до WayForPay API. Запити мають складну структуру з великою кількістю параметрів. Наприклад, `ChargeRequest` містить 20+ полів:

| Категорія | Поля |
|-----------|------|
| **Merchant** | MerchantAccount, MerchantDomainName, MerchantTransactionType, MerchantTransactionSecureType |
| **Order** | OrderReference, OrderDate, Amount, Currency |
| **Products** | ProductName[], ProductPrice[], ProductCount[] |
| **Payment** | Card (Number, ExpMonth, ExpYear, CVV, Holder) або RecToken |
| **Client** | FirstName, LastName, Email, Phone, Country, IpAddress... |
| **Callbacks** | ServiceUrl, ReturnUrl |
| **Options** | HoldTimeout, SocialUri |

### Проблеми прямої ініціалізації

При використанні object initializer syntax виникають наступні проблеми:

```csharp
// Проблема 1: Багато коду, важко читати
var request = new ChargeRequest
{
    MerchantAccount = "merchant",
    MerchantDomainName = "example.com",
    MerchantTransactionType = TransactionType.Sale,
    MerchantTransactionSecureType = SecureType.ThreeDs,
    OrderReference = "ORDER-123",
    OrderDate = DateTimeOffset.UtcNow,
    Amount = 100.00m,
    Currency = "UAH",
    Products = new List<Product>
    {
        new() { Name = "Product 1", Price = 50.00m, Count = 1 },
        new() { Name = "Product 2", Price = 50.00m, Count = 1 }
    },
    Card = new Card
    {
        Number = "4111111111111111",
        ExpireMonth = 12,
        ExpireYear = 2025,
        Cvv = "123",
        Holder = "JOHN DOE"
    },
    Client = new Client
    {
        FirstName = "John",
        LastName = "Doe",
        Email = "john@example.com",
        Phone = "+380991234567"
    },
    ServiceUrl = "https://example.com/webhook"
};

// Проблема 2: Не очевидно, які поля обов'язкові
// Проблема 3: Немає валідації до виклику API
// Проблема 4: Взаємовиключні поля (Card vs RecToken) не контролюються
```

### Вимоги з PRD

| Секція PRD | Вимога |
|------------|--------|
| 7.2 | Builder Pattern API з `IChargeRequestBuilder` |
| 8.3 | Приклад використання `ChargeRequestBuilder` |
| 4.4 | Ergonomic API з XML docs для IntelliSense |

### Цільовий API з PRD (секція 8.3)

```csharp
var request = ChargeRequestBuilder.Create()
    .WithOrderReference(Guid.NewGuid().ToString())
    .WithAmount(100.00m, "UAH")
    .WithProducts(
        new Product { Name = "Product 1", Price = 50.00m, Count = 1 },
        new Product { Name = "Product 2", Price = 50.00m, Count = 1 })
    .WithCard(new Card
    {
        Number = "4111111111111111",
        ExpireMonth = 12,
        ExpireYear = 2025,
        Cvv = "123",
        Holder = "JOHN DOE"
    })
    .WithClient(new Client
    {
        FirstName = "John",
        LastName = "Doe",
        Email = "john@example.com",
        Phone = "+380991234567"
    })
    .WithServiceUrl("https://myshop.com/webhook")
    .AsSale()
    .With3DS()
    .Build();
```

## Критерії вибору (Decision Drivers)

- **IntelliSense-friendly** — IDE повинен підказувати доступні методи та їх призначення
- **Required vs Optional** — чітке розмежування обов'язкових та опціональних параметрів
- **Fluent Chaining** — можливість виклику методів ланцюжком
- **Validation** — валідація перед створенням запиту, а не при виклику API
- **Discoverability** — розробник легко знаходить потрібні методи через autocomplete
- **Reusability** — можливість створення базових конфігурацій та їх повторного використання
- **Type Safety** — компілятор повинен попереджати про помилки типів

## Розглянуті варіанти

1. Прямий конструктор — `new ChargeRequest { ... }`
2. Classic Builder — `ChargeRequestBuilder.Create().WithX().Build()`
3. Fluent Builder з immutable state — кожен метод повертає новий builder
4. Factory Methods — `ChargeRequest.ForCard()`, `ChargeRequest.ForToken()`
5. Step Builder — interfaces для required fields (compile-time safety)

## Рішення

Обрано **Варіант 2: Classic Builder з mutable state**, тому що цей підхід забезпечує найкращий баланс між зручністю використання, IntelliSense підтримкою, продуктивністю та можливістю валідації.

### Варіант 1: Прямий конструктор (Object Initializer)

```csharp
var request = new ChargeRequest
{
    MerchantAccount = "merchant",
    MerchantDomainName = "example.com",
    OrderReference = "ORDER-123",
    Amount = 100.00m,
    Currency = "UAH",
    // ... 15+ інших полів
};
```

**Переваги:**

- Простота — не потребує додаткового коду
- Знайомий синтаксис для .NET розробників
- Нульовий overhead на runtime
- Працює з `required` keyword для обов'язкових полів

**Недоліки:**

- Погана читабельність при великій кількості полів
- Немає логічного групування параметрів (Merchant, Order, Payment...)
- Валідація можлива тільки після створення об'єкта
- Взаємовиключні поля (Card vs RecToken) не контролюються на рівні API
- Відсутні convenience методи (AsSale(), With3DS())

### Варіант 2: Classic Builder з mutable state

```csharp
public sealed class ChargeRequestBuilder
{
    // Merchant settings
    private string? _merchantAccount;
    private string? _merchantDomainName;
    private TransactionType _transactionType = TransactionType.Auto;
    private SecureType _secureType = SecureType.Auto;

    // Order details
    private string? _orderReference;
    private DateTimeOffset _orderDate = DateTimeOffset.UtcNow;
    private decimal _amount;
    private string _currency = "UAH";

    // Products
    private readonly List<Product> _products = new();

    // Payment method
    private Card? _card;
    private string? _recToken;

    // Client
    private Client? _client;

    // Callbacks
    private string? _serviceUrl;
    private string? _returnUrl;

    // Options
    private int? _holdTimeout;

    private ChargeRequestBuilder() { }

    public static ChargeRequestBuilder Create() => new();

    // Order methods
    public ChargeRequestBuilder WithOrderReference(string orderReference)
    {
        _orderReference = orderReference ?? throw new ArgumentNullException(nameof(orderReference));
        return this;
    }

    public ChargeRequestBuilder WithAmount(decimal amount, string currency = "UAH")
    {
        _amount = amount > 0 ? amount : throw new ArgumentOutOfRangeException(nameof(amount));
        _currency = currency ?? throw new ArgumentNullException(nameof(currency));
        return this;
    }

    public ChargeRequestBuilder WithOrderDate(DateTimeOffset orderDate)
    {
        _orderDate = orderDate;
        return this;
    }

    // Products methods
    public ChargeRequestBuilder WithProduct(Product product)
    {
        _products.Add(product ?? throw new ArgumentNullException(nameof(product)));
        return this;
    }

    public ChargeRequestBuilder WithProducts(params Product[] products)
    {
        _products.AddRange(products ?? throw new ArgumentNullException(nameof(products)));
        return this;
    }

    public ChargeRequestBuilder WithProducts(IEnumerable<Product> products)
    {
        _products.AddRange(products ?? throw new ArgumentNullException(nameof(products)));
        return this;
    }

    // Payment method (mutually exclusive)
    public ChargeRequestBuilder WithCard(Card card)
    {
        _card = card ?? throw new ArgumentNullException(nameof(card));
        _recToken = null; // Clear recToken when card is set
        return this;
    }

    public ChargeRequestBuilder WithRecToken(string recToken)
    {
        _recToken = recToken ?? throw new ArgumentNullException(nameof(recToken));
        _card = null; // Clear card when recToken is set
        return this;
    }

    // Client
    public ChargeRequestBuilder WithClient(Client client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        return this;
    }

    // Callbacks
    public ChargeRequestBuilder WithServiceUrl(string serviceUrl)
    {
        _serviceUrl = serviceUrl;
        return this;
    }

    public ChargeRequestBuilder WithReturnUrl(string returnUrl)
    {
        _returnUrl = returnUrl;
        return this;
    }

    // Transaction type shortcuts
    public ChargeRequestBuilder AsSale()
    {
        _transactionType = TransactionType.Sale;
        return this;
    }

    public ChargeRequestBuilder AsAuth()
    {
        _transactionType = TransactionType.Auth;
        return this;
    }

    public ChargeRequestBuilder AsAuth(int holdTimeoutSeconds)
    {
        _transactionType = TransactionType.Auth;
        _holdTimeout = holdTimeoutSeconds;
        return this;
    }

    // 3DS shortcuts
    public ChargeRequestBuilder With3DS()
    {
        _secureType = SecureType.ThreeDs;
        return this;
    }

    public ChargeRequestBuilder Without3DS()
    {
        _secureType = SecureType.NonThreeDs;
        return this;
    }

    // Build with validation
    public ChargeRequest Build()
    {
        Validate();

        return new ChargeRequest
        {
            MerchantAccount = _merchantAccount!,
            MerchantDomainName = _merchantDomainName!,
            MerchantTransactionType = _transactionType,
            MerchantTransactionSecureType = _secureType,
            OrderReference = _orderReference!,
            OrderDate = _orderDate,
            Amount = _amount,
            Currency = _currency,
            Products = _products.ToList(),
            Card = _card,
            RecToken = _recToken,
            Client = _client,
            ServiceUrl = _serviceUrl,
            ReturnUrl = _returnUrl,
            HoldTimeout = _holdTimeout
        };
    }

    private void Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(_orderReference))
            errors.Add("OrderReference is required");

        if (_amount <= 0)
            errors.Add("Amount must be greater than zero");

        if (_products.Count == 0)
            errors.Add("At least one product is required");

        if (_card is null && string.IsNullOrWhiteSpace(_recToken))
            errors.Add("Either Card or RecToken is required");

        if (errors.Count > 0)
            throw new ValidationException(errors);
    }
}
```

**Переваги:**

- **IntelliSense** — всі методи видимі через autocomplete
- **Fluent API** — методи повертають `this` для chaining
- **Логічне групування** — методи організовані за категоріями (WithOrder*, WithCard, As*, With3DS)
- **Convenience methods** — AsSale(), AsAuth(), With3DS() спрощують типові сценарії
- **Validation** — перевірка в Build() до створення запиту
- **Mutual exclusion** — WithCard() очищає RecToken і навпаки
- **Reusability** — builder можна зберегти та перевикористати
- **Low overhead** — мінімальний вплив на продуктивність

**Недоліки:**

- Mutable state — builder змінюється при кожному виклику методу
- Thread safety — не можна безпечно використовувати з декількох потоків
- Compile-time safety — обов'язкові поля перевіряються тільки в runtime

### Варіант 3: Fluent Builder з immutable state

```csharp
public sealed class ChargeRequestBuilder
{
    private readonly ChargeRequestState _state;

    private ChargeRequestBuilder(ChargeRequestState state) => _state = state;

    public static ChargeRequestBuilder Create() =>
        new(ChargeRequestState.Empty);

    public ChargeRequestBuilder WithOrderReference(string orderReference) =>
        new(_state with { OrderReference = orderReference });

    public ChargeRequestBuilder WithAmount(decimal amount, string currency = "UAH") =>
        new(_state with { Amount = amount, Currency = currency });

    // ... інші методи створюють новий builder

    public ChargeRequest Build() => _state.ToRequest();

    private sealed record ChargeRequestState
    {
        public static ChargeRequestState Empty => new();

        public string? OrderReference { get; init; }
        public decimal Amount { get; init; }
        public string Currency { get; init; } = "UAH";
        // ... інші поля

        public ChargeRequest ToRequest() => new()
        {
            OrderReference = OrderReference!,
            Amount = Amount,
            Currency = Currency
            // ...
        };
    }
}
```

**Переваги:**

- Повна immutability — thread-safe
- Можливість "розгалуження" builder-а для різних варіантів
- Функціональний стиль
- Легше тестувати

**Недоліки:**

- **Значний overhead** — створення нового об'єкта при кожному виклику методу
- **Memory pressure** — багато проміжних об'єктів для GC
- Надмірна складність для типового use case
- Не типовий для .NET екосистеми

### Варіант 4: Factory Methods

```csharp
public sealed record ChargeRequest
{
    // Factory methods
    public static ChargeRequest ForCard(
        string orderReference,
        decimal amount,
        IEnumerable<Product> products,
        Card card,
        string currency = "UAH") => new()
    {
        OrderReference = orderReference,
        Amount = amount,
        Currency = currency,
        Products = products.ToList(),
        Card = card
    };

    public static ChargeRequest ForToken(
        string orderReference,
        decimal amount,
        IEnumerable<Product> products,
        string recToken,
        string currency = "UAH") => new()
    {
        OrderReference = orderReference,
        Amount = amount,
        Currency = currency,
        Products = products.ToList(),
        RecToken = recToken
    };

    // Properties...
}
```

**Переваги:**

- Чітке розмежування сценаріїв (Card vs Token)
- Compile-time safety для основних параметрів
- Простота імплементації

**Недоліки:**

- **Комбінаторний вибух** — потрібно багато factory methods для різних комбінацій
- Важко додавати опціональні параметри
- Не fluent — один виклик методу
- Складно налаштовувати 3DS, AUTH, callbacks

### Варіант 5: Step Builder (Wizard Pattern)

```csharp
public interface IOrderStep
{
    IAmountStep WithOrderReference(string orderReference);
}

public interface IAmountStep
{
    IProductsStep WithAmount(decimal amount, string currency = "UAH");
}

public interface IProductsStep
{
    IPaymentStep WithProducts(params Product[] products);
}

public interface IPaymentStep
{
    IOptionalStep WithCard(Card card);
    IOptionalStep WithRecToken(string recToken);
}

public interface IOptionalStep
{
    IOptionalStep WithClient(Client client);
    IOptionalStep WithServiceUrl(string serviceUrl);
    IOptionalStep AsSale();
    IOptionalStep With3DS();
    ChargeRequest Build();
}

public sealed class ChargeRequestBuilder : IOrderStep, IAmountStep, IProductsStep, IPaymentStep, IOptionalStep
{
    public static IOrderStep Create() => new ChargeRequestBuilder();

    // Реалізація всіх інтерфейсів...
}

// Використання - compile-time safety!
var request = ChargeRequestBuilder.Create()
    .WithOrderReference("ORDER-123")      // IOrderStep -> IAmountStep
    .WithAmount(100.00m, "UAH")           // IAmountStep -> IProductsStep
    .WithProducts(product1, product2)     // IProductsStep -> IPaymentStep
    .WithCard(card)                       // IPaymentStep -> IOptionalStep
    .WithClient(client)                   // IOptionalStep -> IOptionalStep
    .AsSale()                             // IOptionalStep -> IOptionalStep
    .Build();                             // IOptionalStep -> ChargeRequest
```

**Переваги:**

- **Compile-time safety** — неможливо пропустити обов'язкові кроки
- **Guided API** — IntelliSense показує тільки доступні методи на кожному кроці
- Чіткий порядок налаштування

**Недоліки:**

- **Складність імплементації** — багато інтерфейсів та boilerplate
- **Rigidity** — фіксований порядок викликів
- Важко підтримувати при додаванні нових полів
- Не типовий для .NET — може здивувати розробників
- Опціональні поля все одно потребують runtime validation

## Детальна імплементація обраного рішення

### Структура файлів

```
WayForPaySDK/
└── Builders/
    ├── ChargeRequestBuilder.cs
    ├── RefundRequestBuilder.cs
    ├── InvoiceRequestBuilder.cs
    ├── PurchaseFormBuilder.cs
    └── CheckRequestBuilder.cs
```

### Повна імплементація ChargeRequestBuilder

```csharp
namespace WayForPaySDK.Builders;

/// <summary>
/// Fluent builder для створення <see cref="ChargeRequest"/>.
/// </summary>
/// <example>
/// <code>
/// var request = ChargeRequestBuilder.Create()
///     .WithOrderReference("ORDER-123")
///     .WithAmount(100.00m, "UAH")
///     .WithProducts(new Product { Name = "Item", Price = 100m, Count = 1 })
///     .WithCard(card)
///     .AsSale()
///     .Build();
/// </code>
/// </example>
public sealed class ChargeRequestBuilder
{
    // Merchant settings (injected from options)
    private string? _merchantAccount;
    private string? _merchantDomainName;
    private TransactionType _transactionType = TransactionType.Auto;
    private SecureType _secureType = SecureType.Auto;

    // Order details
    private string? _orderReference;
    private DateTimeOffset _orderDate = DateTimeOffset.UtcNow;
    private decimal _amount;
    private string _currency = "UAH";

    // Products
    private readonly List<Product> _products = [];

    // Payment method (mutually exclusive)
    private Card? _card;
    private string? _recToken;

    // Client info
    private Client? _client;

    // Callbacks
    private string? _serviceUrl;
    private string? _returnUrl;

    // Options
    private int? _holdTimeout;
    private string? _socialUri;

    private ChargeRequestBuilder() { }

    /// <summary>
    /// Створює новий екземпляр builder-а.
    /// </summary>
    public static ChargeRequestBuilder Create() => new();

    /// <summary>
    /// Створює новий екземпляр builder-а з попередньо налаштованими merchant credentials.
    /// </summary>
    /// <param name="options">Налаштування SDK з merchant credentials.</param>
    public static ChargeRequestBuilder Create(WayForPayOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        return new ChargeRequestBuilder()
            .WithMerchant(options.MerchantAccount, options.MerchantDomainName);
    }

    #region Merchant Settings

    /// <summary>
    /// Встановлює merchant credentials.
    /// </summary>
    /// <param name="merchantAccount">Ідентифікатор мерчанта в системі WayForPay.</param>
    /// <param name="merchantDomainName">Доменне ім'я мерчанта.</param>
    public ChargeRequestBuilder WithMerchant(string merchantAccount, string merchantDomainName)
    {
        _merchantAccount = merchantAccount ?? throw new ArgumentNullException(nameof(merchantAccount));
        _merchantDomainName = merchantDomainName ?? throw new ArgumentNullException(nameof(merchantDomainName));
        return this;
    }

    #endregion

    #region Order Details

    /// <summary>
    /// Встановлює унікальний ідентифікатор замовлення.
    /// </summary>
    /// <param name="orderReference">Унікальний номер замовлення в системі мерчанта.</param>
    public ChargeRequestBuilder WithOrderReference(string orderReference)
    {
        _orderReference = orderReference ?? throw new ArgumentNullException(nameof(orderReference));
        return this;
    }

    /// <summary>
    /// Встановлює суму та валюту платежу.
    /// </summary>
    /// <param name="amount">Сума платежу (має бути більше 0).</param>
    /// <param name="currency">Код валюти (UAH, USD, EUR). За замовчуванням UAH.</param>
    public ChargeRequestBuilder WithAmount(decimal amount, string currency = "UAH")
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero");

        _amount = amount;
        _currency = currency ?? throw new ArgumentNullException(nameof(currency));
        return this;
    }

    /// <summary>
    /// Встановлює дату замовлення. За замовчуванням використовується поточний час.
    /// </summary>
    /// <param name="orderDate">Дата та час створення замовлення.</param>
    public ChargeRequestBuilder WithOrderDate(DateTimeOffset orderDate)
    {
        _orderDate = orderDate;
        return this;
    }

    #endregion

    #region Products

    /// <summary>
    /// Додає один товар до замовлення.
    /// </summary>
    /// <param name="product">Товар для додавання.</param>
    public ChargeRequestBuilder WithProduct(Product product)
    {
        _products.Add(product ?? throw new ArgumentNullException(nameof(product)));
        return this;
    }

    /// <summary>
    /// Додає декілька товарів до замовлення.
    /// </summary>
    /// <param name="products">Товари для додавання.</param>
    public ChargeRequestBuilder WithProducts(params Product[] products)
    {
        ArgumentNullException.ThrowIfNull(products);
        _products.AddRange(products);
        return this;
    }

    /// <summary>
    /// Додає колекцію товарів до замовлення.
    /// </summary>
    /// <param name="products">Колекція товарів для додавання.</param>
    public ChargeRequestBuilder WithProducts(IEnumerable<Product> products)
    {
        ArgumentNullException.ThrowIfNull(products);
        _products.AddRange(products);
        return this;
    }

    #endregion

    #region Payment Method

    /// <summary>
    /// Встановлює картові дані для оплати.
    /// Взаємовиключний з <see cref="WithRecToken"/>.
    /// </summary>
    /// <param name="card">Дані банківської карти.</param>
    public ChargeRequestBuilder WithCard(Card card)
    {
        _card = card ?? throw new ArgumentNullException(nameof(card));
        _recToken = null; // Взаємовиключення
        return this;
    }

    /// <summary>
    /// Встановлює токен для рекурентного платежу.
    /// Взаємовиключний з <see cref="WithCard"/>.
    /// </summary>
    /// <param name="recToken">Токен, отриманий з попередньої транзакції.</param>
    public ChargeRequestBuilder WithRecToken(string recToken)
    {
        _recToken = recToken ?? throw new ArgumentNullException(nameof(recToken));
        _card = null; // Взаємовиключення
        return this;
    }

    #endregion

    #region Client Info

    /// <summary>
    /// Встановлює інформацію про клієнта/покупця.
    /// </summary>
    /// <param name="client">Дані клієнта.</param>
    public ChargeRequestBuilder WithClient(Client client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        return this;
    }

    #endregion

    #region Callbacks

    /// <summary>
    /// Встановлює URL для отримання webhook-сповіщень про статус платежу.
    /// </summary>
    /// <param name="serviceUrl">URL endpoint для callback.</param>
    public ChargeRequestBuilder WithServiceUrl(string serviceUrl)
    {
        _serviceUrl = serviceUrl;
        return this;
    }

    /// <summary>
    /// Встановлює URL для повернення користувача після оплати.
    /// </summary>
    /// <param name="returnUrl">URL для редиректу.</param>
    public ChargeRequestBuilder WithReturnUrl(string returnUrl)
    {
        _returnUrl = returnUrl;
        return this;
    }

    #endregion

    #region Transaction Type

    /// <summary>
    /// Налаштовує пряме списання коштів (SALE).
    /// Кошти списуються одразу після авторизації.
    /// </summary>
    public ChargeRequestBuilder AsSale()
    {
        _transactionType = TransactionType.Sale;
        return this;
    }

    /// <summary>
    /// Налаштовує авторизацію з відкладеним списанням (AUTH).
    /// Кошти блокуються, але не списуються до виклику Settle.
    /// </summary>
    public ChargeRequestBuilder AsAuth()
    {
        _transactionType = TransactionType.Auth;
        return this;
    }

    /// <summary>
    /// Налаштовує авторизацію з відкладеним списанням та таймаутом.
    /// </summary>
    /// <param name="holdTimeoutSeconds">Час утримання коштів у секундах.</param>
    public ChargeRequestBuilder AsAuth(int holdTimeoutSeconds)
    {
        if (holdTimeoutSeconds <= 0)
            throw new ArgumentOutOfRangeException(nameof(holdTimeoutSeconds));

        _transactionType = TransactionType.Auth;
        _holdTimeout = holdTimeoutSeconds;
        return this;
    }

    #endregion

    #region 3D Secure

    /// <summary>
    /// Вмикає обов'язкову 3D Secure автентифікацію.
    /// </summary>
    public ChargeRequestBuilder With3DS()
    {
        _secureType = SecureType.ThreeDs;
        return this;
    }

    /// <summary>
    /// Вимикає 3D Secure автентифікацію (якщо дозволено мерчанту).
    /// </summary>
    public ChargeRequestBuilder Without3DS()
    {
        _secureType = SecureType.NonThreeDs;
        return this;
    }

    #endregion

    #region Additional Options

    /// <summary>
    /// Встановлює посилання на соціальну мережу для верифікації.
    /// </summary>
    /// <param name="socialUri">URI профілю в соціальній мережі.</param>
    public ChargeRequestBuilder WithSocialUri(string socialUri)
    {
        _socialUri = socialUri;
        return this;
    }

    #endregion

    #region Build

    /// <summary>
    /// Створює <see cref="ChargeRequest"/> з налаштованими параметрами.
    /// </summary>
    /// <returns>Готовий до відправки запит.</returns>
    /// <exception cref="ValidationException">
    /// Кидається, якщо обов'язкові поля не заповнені або дані невалідні.
    /// </exception>
    public ChargeRequest Build()
    {
        Validate();

        return new ChargeRequest
        {
            MerchantAccount = _merchantAccount!,
            MerchantDomainName = _merchantDomainName!,
            MerchantTransactionType = _transactionType,
            MerchantTransactionSecureType = _secureType,
            OrderReference = _orderReference!,
            OrderDate = _orderDate,
            Amount = _amount,
            Currency = _currency,
            Products = _products.AsReadOnly(),
            Card = _card,
            RecToken = _recToken,
            Client = _client,
            ServiceUrl = _serviceUrl,
            ReturnUrl = _returnUrl,
            HoldTimeout = _holdTimeout,
            SocialUri = _socialUri
        };
    }

    private void Validate()
    {
        var errors = new List<string>();

        // Required fields
        if (string.IsNullOrWhiteSpace(_merchantAccount))
            errors.Add("MerchantAccount is required. Use WithMerchant() or Create(options).");

        if (string.IsNullOrWhiteSpace(_merchantDomainName))
            errors.Add("MerchantDomainName is required. Use WithMerchant() or Create(options).");

        if (string.IsNullOrWhiteSpace(_orderReference))
            errors.Add("OrderReference is required. Use WithOrderReference().");

        if (_amount <= 0)
            errors.Add("Amount must be greater than zero. Use WithAmount().");

        if (_products.Count == 0)
            errors.Add("At least one product is required. Use WithProduct() or WithProducts().");

        // Payment method validation (mutually exclusive)
        if (_card is null && string.IsNullOrWhiteSpace(_recToken))
            errors.Add("Payment method is required. Use WithCard() or WithRecToken().");

        // Product validation
        var invalidProducts = _products
            .Select((p, i) => new { Product = p, Index = i })
            .Where(x => x.Product.Price <= 0 || x.Product.Count <= 0)
            .ToList();

        foreach (var invalid in invalidProducts)
        {
            errors.Add($"Product at index {invalid.Index} has invalid Price or Count.");
        }

        // Amount validation (sum of products should match)
        var productsTotal = _products.Sum(p => p.Price * p.Count);
        if (productsTotal != _amount)
        {
            errors.Add($"Amount ({_amount}) does not match products total ({productsTotal}).");
        }

        if (errors.Count > 0)
        {
            throw new ValidationException(
                $"ChargeRequest validation failed with {errors.Count} error(s).",
                errors);
        }
    }

    #endregion
}
```

### InvoiceRequestBuilder

```csharp
namespace WayForPaySDK.Builders;

/// <summary>
/// Fluent builder для створення <see cref="InvoiceRequest"/>.
/// </summary>
public sealed class InvoiceRequestBuilder
{
    private string? _merchantAccount;
    private string? _merchantDomainName;
    private string? _orderReference;
    private DateTimeOffset _orderDate = DateTimeOffset.UtcNow;
    private decimal _amount;
    private string _currency = "UAH";
    private readonly List<Product> _products = [];
    private string? _clientEmail;
    private string? _clientPhone;
    private int? _orderTimeout;
    private int? _orderLifetime;
    private string _language = "UA";
    private PaymentSystem _paymentSystems = PaymentSystem.All;
    private string? _serviceUrl;

    private InvoiceRequestBuilder() { }

    public static InvoiceRequestBuilder Create() => new();

    public static InvoiceRequestBuilder Create(WayForPayOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return new InvoiceRequestBuilder()
            .WithMerchant(options.MerchantAccount, options.MerchantDomainName);
    }

    public InvoiceRequestBuilder WithMerchant(string merchantAccount, string merchantDomainName)
    {
        _merchantAccount = merchantAccount ?? throw new ArgumentNullException(nameof(merchantAccount));
        _merchantDomainName = merchantDomainName ?? throw new ArgumentNullException(nameof(merchantDomainName));
        return this;
    }

    public InvoiceRequestBuilder WithOrderReference(string orderReference)
    {
        _orderReference = orderReference ?? throw new ArgumentNullException(nameof(orderReference));
        return this;
    }

    public InvoiceRequestBuilder WithAmount(decimal amount, string currency = "UAH")
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        _amount = amount;
        _currency = currency ?? throw new ArgumentNullException(nameof(currency));
        return this;
    }

    public InvoiceRequestBuilder WithProducts(params Product[] products)
    {
        ArgumentNullException.ThrowIfNull(products);
        _products.AddRange(products);
        return this;
    }

    public InvoiceRequestBuilder WithClientEmail(string email)
    {
        _clientEmail = email ?? throw new ArgumentNullException(nameof(email));
        return this;
    }

    public InvoiceRequestBuilder WithClientPhone(string phone)
    {
        _clientPhone = phone;
        return this;
    }

    public InvoiceRequestBuilder WithTimeout(int seconds)
    {
        _orderTimeout = seconds;
        return this;
    }

    public InvoiceRequestBuilder WithLifetime(int seconds)
    {
        _orderLifetime = seconds;
        return this;
    }

    public InvoiceRequestBuilder WithLanguage(string language)
    {
        _language = language ?? "UA";
        return this;
    }

    public InvoiceRequestBuilder WithPaymentSystems(PaymentSystem paymentSystems)
    {
        _paymentSystems = paymentSystems;
        return this;
    }

    public InvoiceRequestBuilder WithServiceUrl(string serviceUrl)
    {
        _serviceUrl = serviceUrl;
        return this;
    }

    public InvoiceRequest Build()
    {
        // Validation logic...

        return new InvoiceRequest
        {
            MerchantAccount = _merchantAccount!,
            MerchantDomainName = _merchantDomainName!,
            OrderReference = _orderReference!,
            OrderDate = _orderDate,
            Amount = _amount,
            Currency = _currency,
            Products = _products.AsReadOnly(),
            ClientEmail = _clientEmail!,
            ClientPhone = _clientPhone,
            OrderTimeout = _orderTimeout,
            OrderLifetime = _orderLifetime,
            Language = _language,
            PaymentSystems = _paymentSystems,
            ServiceUrl = _serviceUrl
        };
    }
}
```

### Інтеграція з IWayForPayClient

```csharp
// Приклад використання в сервісі
public class PaymentService
{
    private readonly IWayForPayClient _client;
    private readonly WayForPayOptions _options;

    public PaymentService(IWayForPayClient client, IOptions<WayForPayOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task<ChargeResponse> ProcessPaymentAsync(Order order, Card card)
    {
        var request = ChargeRequestBuilder.Create(_options)
            .WithOrderReference(order.Id.ToString())
            .WithAmount(order.Total, "UAH")
            .WithProducts(order.Items.Select(i => new Product
            {
                Name = i.ProductName,
                Price = i.UnitPrice,
                Count = i.Quantity
            }))
            .WithCard(card)
            .WithClient(new Client
            {
                FirstName = order.Customer.FirstName,
                LastName = order.Customer.LastName,
                Email = order.Customer.Email,
                Phone = order.Customer.Phone
            })
            .WithServiceUrl($"{_options.BaseUrl}/api/payment/webhook")
            .AsSale()
            .With3DS()
            .Build();

        return await _client.ChargeAsync(request);
    }

    public async Task<ChargeResponse> ProcessRecurringPaymentAsync(
        string orderReference,
        decimal amount,
        string recToken,
        Product product)
    {
        var request = ChargeRequestBuilder.Create(_options)
            .WithOrderReference(orderReference)
            .WithAmount(amount, "UAH")
            .WithProduct(product)
            .WithRecToken(recToken)
            .AsSale()
            .Build();

        return await _client.ChargeAsync(request);
    }
}
```

## Наслідки

### Позитивні

- **Discoverability** — IntelliSense показує всі доступні методи з XML документацією
- **Readability** — fluent API читається як природна мова
- **Validation** — помилки виявляються при Build(), а не при виклику API
- **Flexibility** — опціональні параметри легко додаються або пропускаються
- **Mutual Exclusion** — взаємовиключні поля контролюються автоматично
- **Reusability** — можна зберегти builder та перевикористати для схожих запитів
- **Extensibility** — легко додавати нові методи без breaking changes

### Негативні

- **Runtime Validation** — обов'язкові поля перевіряються в runtime, а не compile-time
- **Mutable State** — builder не thread-safe, не можна використовувати з декількох потоків одночасно
- **Additional Code** — потребує написання builder класів для кожного типу запиту

### Нейтральні

- **Learning Curve** — розробники мають знати про Builder pattern (широко відомий)
- **Memory** — невеликий overhead на зберігання проміжного стану builder-а

## Порівняльна таблиця

| Критерій | Object Init | Classic Builder | Immutable Builder | Factory Methods | Step Builder |
|----------|-------------|-----------------|-------------------|-----------------|--------------|
| IntelliSense | :white_check_mark: | :white_check_mark: | :white_check_mark: | :white_check_mark: | :white_check_mark: |
| Fluent API | :x: | :white_check_mark: | :white_check_mark: | :x: | :white_check_mark: |
| Compile-time Safety | :x: | :x: | :x: | Partial | :white_check_mark: |
| Validation | :x: | :white_check_mark: | :white_check_mark: | :x: | :white_check_mark: |
| Performance | :white_check_mark: | :white_check_mark: | :x: | :white_check_mark: | :white_check_mark: |
| Simplicity | :white_check_mark: | :white_check_mark: | :x: | :white_check_mark: | :x: |
| Thread Safety | :white_check_mark: | :x: | :white_check_mark: | :white_check_mark: | :x: |
| Reusability | :x: | :white_check_mark: | :white_check_mark: | :x: | :white_check_mark: |
| Mutual Exclusion | :x: | :white_check_mark: | :white_check_mark: | :white_check_mark: | :white_check_mark: |

## Посилання

- [PRD](../PRD.md) — секція 7.2 (Builder Pattern API), секція 8.3 (Приклад використання)
- [ADR-003](ADR-003-domain-models-design.md) — Domain Models Design (records з required init)
- [ADR-004](ADR-004-error-handling.md) — Error Handling (ValidationException)
- [Microsoft Docs: Builder Pattern](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/builder)
- [Fluent Interface (Martin Fowler)](https://martinfowler.com/bliki/FluentInterface.html)
- [Effective Java: Builder Pattern (Joshua Bloch)](https://www.informit.com/articles/article.aspx?p=1216151)

## Примітки

- Кожен builder має статичний метод `Create()` для створення нового екземпляра
- Метод `Create(WayForPayOptions)` дозволяє автоматично налаштувати merchant credentials
- Валідація суми товарів порівнюється з загальною сумою замовлення
- Builder-и розміщуються в namespace `WayForPaySDK.Builders`
- XML документація обов'язкова для всіх public методів
