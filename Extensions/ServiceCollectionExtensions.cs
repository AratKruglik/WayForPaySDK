using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WayForPaySDK.Crypto;
using WayForPaySDK.Http;
using WayForPaySDK.Options;

namespace WayForPaySDK.Extensions;

/// <summary>
/// Extension methods for configuring WayForPay SDK services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds WayForPay SDK services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Action to configure WayForPay options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddWayForPay(
        this IServiceCollection services,
        Action<WayForPayOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.Configure(configureOptions);

        return services.AddWayForPayCore();
    }

    /// <summary>
    /// Adds WayForPay SDK services to the service collection using configuration binding.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configurationSection">The configuration section containing WayForPay settings.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddWayForPay(
        this IServiceCollection services,
        Microsoft.Extensions.Configuration.IConfigurationSection configurationSection)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configurationSection);

        services.Configure<WayForPayOptions>(configurationSection);

        return services.AddWayForPayCore();
    }

    private static IServiceCollection AddWayForPayCore(this IServiceCollection services)
    {
        services.AddSingleton<ISignatureGenerator, SignatureGenerator>();

        services.AddHttpClient<IWayForPayHttpClient, WayForPayHttpClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<WayForPayOptions>>().Value;
            client.BaseAddress = new Uri(options.ApiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }
}
