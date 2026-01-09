#if NET8_0_OR_GREATER
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

    public static async Task<IActionResult> HandleAsync(
        this IWebhookHandler handler,
        HttpRequest request,
        Func<WebhookPayload, Task> processPayload,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(processPayload);

        try
        {
            var payload = await handler.ParseAsync(request, cancellationToken);
            await processPayload(payload);
            var response = handler.CreateResponse(payload, WebhookStatus.Accept);
            return response.ToActionResult();
        }
        catch
        {
            // If processing fails, we still need to respond to WayForPay
            var declineResponse = new WebhookResponse
            {
                OrderReference = string.Empty,
                Status = "decline",
                Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Signature = string.Empty
            };
            return declineResponse.ToActionResult();
        }
    }

    public static async Task<IActionResult> HandleAsync(
        this IWebhookHandler handler,
        HttpRequest request,
        Func<WebhookPayload, Task<WebhookStatus>> processPayload,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(processPayload);

        try
        {
            var payload = await handler.ParseAsync(request, cancellationToken);
            var status = await processPayload(payload);
            var response = handler.CreateResponse(payload, status);
            return response.ToActionResult();
        }
        catch
        {
            var declineResponse = new WebhookResponse
            {
                OrderReference = string.Empty,
                Status = "decline",
                Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Signature = string.Empty
            };
            return declineResponse.ToActionResult();
        }
    }
}
#endif
