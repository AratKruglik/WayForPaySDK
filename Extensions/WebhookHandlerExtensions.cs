#if NET8_0_OR_GREATER
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WayForPaySDK.Handlers;
using WayForPaySDK.Serialization;

namespace WayForPaySDK.Extensions;

public static class WebhookHandlerExtensions
{
    public static async Task<WebhookPayload> ParseAsync(
        this IWebhookHandler handler,
        HttpRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(request);

        request.EnableBuffering();

        return await handler.ParseAsync(request.Body, cancellationToken);
    }

    public static IActionResult ToActionResult(this WebhookResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        var json = JsonSerializer.Serialize(response, WayForPayJsonContext.Default.WebhookResponse);

        return new ContentResult
        {
            Content = json,
            ContentType = "application/json",
            StatusCode = StatusCodes.Status200OK
        };
    }

    /// <summary>
    /// Handles a webhook request by parsing, processing, and responding.
    /// </summary>
    /// <remarks>
    /// If processing fails, a decline response is returned.
    /// Use the overload with logger parameter to capture exception details.
    /// </remarks>
    public static async Task<IActionResult> HandleAsync(
        this IWebhookHandler handler,
        HttpRequest request,
        Func<WebhookPayload, Task> processPayload,
        CancellationToken cancellationToken = default)
    {
        return await handler.HandleAsync(request, processPayload, null, cancellationToken);
    }

    /// <summary>
    /// Handles a webhook request with logging support.
    /// </summary>
    /// <param name="handler">The webhook handler instance.</param>
    /// <param name="request">The HTTP request containing the webhook payload.</param>
    /// <param name="processPayload">The callback to process the parsed payload.</param>
    /// <param name="logger">Optional logger for capturing exceptions.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public static async Task<IActionResult> HandleAsync(
        this IWebhookHandler handler,
        HttpRequest request,
        Func<WebhookPayload, Task> processPayload,
        ILogger? logger,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(processPayload);

        WebhookPayload? payload = null;
        try
        {
            payload = await handler.ParseAsync(request, cancellationToken);
            await processPayload(payload);
            var response = handler.CreateResponse(payload, WebhookStatus.Accept);
            return response.ToActionResult();
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Webhook processing failed. OrderReference: {OrderReference}",
                payload?.OrderReference ?? "unknown");

            var declineResponse = payload != null
                ? handler.CreateResponse(payload, WebhookStatus.Decline)
                : CreateFallbackDeclineResponse();

            return declineResponse.ToActionResult();
        }
    }

    /// <summary>
    /// Handles a webhook request by parsing, processing with custom status, and responding.
    /// </summary>
    public static async Task<IActionResult> HandleAsync(
        this IWebhookHandler handler,
        HttpRequest request,
        Func<WebhookPayload, Task<WebhookStatus>> processPayload,
        CancellationToken cancellationToken = default)
    {
        return await handler.HandleAsync(request, processPayload, null, cancellationToken);
    }

    /// <summary>
    /// Handles a webhook request with custom status return and logging support.
    /// </summary>
    public static async Task<IActionResult> HandleAsync(
        this IWebhookHandler handler,
        HttpRequest request,
        Func<WebhookPayload, Task<WebhookStatus>> processPayload,
        ILogger? logger,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(processPayload);

        WebhookPayload? payload = null;
        try
        {
            payload = await handler.ParseAsync(request, cancellationToken);
            var status = await processPayload(payload);
            var response = handler.CreateResponse(payload, status);
            return response.ToActionResult();
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Webhook processing failed. OrderReference: {OrderReference}",
                payload?.OrderReference ?? "unknown");

            var declineResponse = payload != null
                ? handler.CreateResponse(payload, WebhookStatus.Decline)
                : CreateFallbackDeclineResponse();

            return declineResponse.ToActionResult();
        }
    }

    /// <summary>
    /// Creates a fallback decline response when payload parsing fails.
    /// </summary>
    /// <remarks>
    /// Note: This response has an empty signature because we don't have the
    /// original payload to generate a proper signature from. WayForPay should
    /// still process this as a decline, but may log a signature mismatch warning.
    /// </remarks>
    private static WebhookResponse CreateFallbackDeclineResponse()
    {
        return new WebhookResponse
        {
            OrderReference = "parsing_failed",
            Status = "decline",
            Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Signature = string.Empty
        };
    }
}
#endif
