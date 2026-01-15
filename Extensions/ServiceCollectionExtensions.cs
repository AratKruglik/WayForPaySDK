using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WayForPaySDK.Crypto;
using WayForPaySDK.Handlers;
using WayForPaySDK.Http;
using WayForPaySDK.Options;
using WayForPaySDK.Services;

namespace WayForPaySDK.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWayForPay(
        this IServiceCollection services,
        Action<WayForPayOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.Configure(configureOptions);

        return services.AddWayForPayCore();
    }

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
        services.AddSingleton<IValidateOptions<WayForPayOptions>, WayForPayOptionsValidator>();
        services.AddSingleton<ISignatureGenerator, SignatureGenerator>();

        services.AddHttpClient<IWayForPayClient, WayForPayClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<WayForPayOptions>>().Value;
            client.BaseAddress = new Uri(options.ApiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .ConfigurePrimaryHttpMessageHandler(sp =>
        {
            var options = sp.GetRequiredService<IOptions<WayForPayOptions>>().Value;
            var handler = new HttpClientHandler();

            if (options.ServerCertificateCustomValidationCallback != null)
            {
                handler.ServerCertificateCustomValidationCallback =
                    options.ServerCertificateCustomValidationCallback;
            }

            return handler;
        });

        services.AddScoped<IWebhookHandler, WebhookHandler>();

        return services;
    }
}
