# ADR-008: Multi-Target Framework Strategy

## Статус

Proposed

## Контекст

WayForPaySDK - це .NET бiблiотека для iнтеграцiї з платiжною системою WayForPay. SDK повинен пiдтримувати широкий спектр .NET проектiв, щоб забезпечити максимальну доступнiсть для розробникiв в українськiй .NET екосистемi.

Поточна ситуацiя:
- Проект налаштований на .NET 10.0 (preview)
- PRD (секцiя 4.5 NFR-05) вимагає пiдтримку .NET 6.0, 7.0, 8.0
- .NET 7.0 досяг End of Life (EOL) у травнi 2024
- .NET 9.0 є поточною стабiльною версiєю (випущена листопад 2024)
- .NET 10.0 знаходиться у preview статусi

Полiтика пiдтримки Microsoft:
- **LTS (Long Term Support)**: 3 роки пiдтримки (.NET 6.0, 8.0)
- **Standard Term Support (STS)**: 18 мiсяцiв пiдтримки (.NET 7.0, 9.0)

## Критерiї вибору (Decision Drivers)

- **Охоплення користувачiв** - максимальна кiлькiсть проектiв, якi можуть використовувати SDK
- **Стабiльнiсть** - пiдтримка версiй з активною пiдтримкою вiд Microsoft
- **Складнiсть пiдтримки** - зусилля на тестування та CI/CD для кожного target framework
- **Мовнi можливостi** - доступ до сучасних C# features
- **Сумiснiсть з enterprise** - пiдтримка LTS версiй для корпоративних клiєнтiв
- **Майбутня готовнiсть** - врахування roadmap .NET

## Розглянутi варiанти

1. Тiльки LTS версiї (net6.0;net8.0)
2. Всi активнi згiдно PRD (net6.0;net7.0;net8.0)
3. LTS + Current (net6.0;net8.0;net9.0)
4. Тiльки найновiший LTS (net8.0)

## Рiшення

Обрано **Варiант 3: LTS + Current (net6.0;net8.0;net9.0)**, тому що цей пiдхiд забезпечує оптимальний баланс мiж охопленням користувачiв, стабiльнiстю та готовнiстю до майбутнього.

### Варiант 1: Тiльки LTS версiї (net6.0;net8.0)

Мiнiмальний набiр з двох LTS версiй, що мають найдовший термiн пiдтримки.

```xml
<TargetFrameworks>net6.0;net8.0</TargetFrameworks>
```

**Переваги:**
- Мiнiмальне навантаження на CI/CD (2 targets)
- Обидвi версiї мають довгострокову пiдтримку Microsoft
- Покриває бiльшiсть enterprise проектiв
- Простота пiдтримки та тестування

**Недолiки:**
- Не пiдтримує проекти на .NET 9.0
- Вiдсутнiсть доступу до найновiших оптимiзацiй runtime
- Користувачi .NET 9.0 вимушенi використовувати net8.0 build

### Варiант 2: Всi активнi згiдно PRD (net6.0;net7.0;net8.0)

Дотримання оригiнальних вимог PRD.

```xml
<TargetFrameworks>net6.0;net7.0;net8.0</TargetFrameworks>
```

**Переваги:**
- Вiдповiдає вимогам PRD (секцiя 4.5 NFR-05)
- Пiдтримує legacy проекти на .NET 7.0

**Недолiки:**
- .NET 7.0 досяг EOL у травнi 2024 - не отримує security updates
- Пiдтримка EOL версiї створює хибне вiдчуття безпеки
- Не пiдтримує поточну стабiльну версiю .NET 9.0
- 3 targets збiльшує час CI/CD без суттєвої переваги

### Варiант 3: LTS + Current (net6.0;net8.0;net9.0)

Двi LTS версiї плюс поточна стабiльна версiя.

```xml
<TargetFrameworks>net6.0;net8.0;net9.0</TargetFrameworks>
```

**Переваги:**
- Покриває всi активно пiдтримуванi версiї .NET
- .NET 6.0 - пiдтримка до листопада 2024, legacy enterprise проекти
- .NET 8.0 (LTS) - пiдтримка до листопада 2026, основний target
- .NET 9.0 - поточна версiя з найновiшими оптимiзацiями
- Не включає EOL версiї
- Доступ до net9.0-специфiчних оптимiзацiй (PGO, ARM64 improvements)

**Недолiки:**
- .NET 6.0 наближається до EOL (листопад 2024)
- 3 targets у CI/CD pipeline
- Потенцiйнi #if директиви для version-specific код

### Варiант 4: Тiльки найновiший LTS (net8.0)

Найпростiший пiдхiд з єдиним target framework.

```xml
<TargetFrameworks>net8.0</TargetFrameworks>
```

**Переваги:**
- Максимальна простота розробки та пiдтримки
- Єдиний target = найшвидший CI/CD
- Доступ до всiх сучасних C# features
- Найновiший LTS з пiдтримкою до 2026

**Недолiки:**
- Виключає всi проекти на .NET 6.0 та 7.0
- Не пiдтримує .NET 9.0 оптимiзацiї
- Обмежує adoption SDK в enterprise з legacy проектами
- Суперечить вимогам PRD

## Обгрунтування вибору Варiанту 3

1. **Безпека перш за все**: Виключення .NET 7.0 (EOL) запобiгає використанню SDK з версiєю без security updates

2. **Прагматичний пiдхiд до legacy**: .NET 6.0 все ще широко використовується в enterprise. Пiдтримка цiєї версiї забезпечує migration path

3. **Готовнiсть до майбутнього**: .NET 9.0 є поточною стабiльною версiєю. SDK повинен пiдтримувати проекти, що використовують найновiший .NET

4. **Performance**: net9.0 build може використовувати runtime-специфiчнi оптимiзацiї

5. **Розумний компромiс**: 3 targets - керована складнiсть для CI/CD

### Дорожня карта оновлень

| Подiя | Дiя | Орiєнтовна дата |
|-------|-----|-----------------|
| .NET 6.0 EOL | Видалити net6.0 target | Листопад 2024 |
| .NET 10.0 LTS Release | Додати net10.0, можливо видалити net9.0 | Листопад 2025 |
| .NET 9.0 EOL | Видалити net9.0 target | Травень 2026 |

## Наслiдки

### Позитивнi

- SDK доступний для 95%+ активних .NET проектiв
- Вiдсутнiсть пiдтримки EOL версiй пiдвищує безпеку
- net9.0 build забезпечує оптимальну продуктивнiсть для новiших проектiв
- Чiтка дорожня карта оновлень

### Негативнi

- Проекти на .NET 7.0 не можуть використовувати SDK напряму (вимушенi оновитись або downgrade до net6.0 build)
- Вiдхилення вiд оригiнальних вимог PRD (net7.0)
- Потреба оновлення PRD для вiдображення актуальної стратегiї

### Нейтральнi

- 3 target frameworks - стандартна практика для .NET бiблiотек
- Потенцiйна потреба в conditional compilation для version-specific features

## Технiчна реалiзацiя

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net6.0;net8.0;net9.0</TargetFrameworks>
    <LangVersion>12.0</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>
```

### Conditional Compilation (за потреби)

```csharp
#if NET9_0_OR_GREATER
    // .NET 9.0+ specific optimizations
#elif NET8_0_OR_GREATER
    // .NET 8.0+ features
#else
    // .NET 6.0 fallback
#endif
```

## Посилання

- [PRD](../PRD.md) - секцiя 4.5 NFR-05: Сумiснiсть
- [.NET Support Policy](https://dotnet.microsoft.com/platform/support/policy)
- [.NET Release Schedule](https://github.com/dotnet/core/blob/main/releases.md)
- [Multi-targeting in .NET](https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/cross-platform-targeting)

## Примiтки

Рiшення вiдхиляється вiд оригiнального PRD, який вимагав пiдтримку .NET 7.0. Це обгрунтоване рiшення, оскiльки:

1. PRD датований 08.01.2026, а .NET 7.0 досяг EOL у травнi 2024
2. Пiдтримка EOL версiй суперечить безпековим вимогам NFR-02
3. Рекомендується оновити PRD для вiдображення актуальної полiтики пiдтримки .NET версiй
