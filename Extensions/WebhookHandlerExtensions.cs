#if NET8_0_OR_GREATER
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WayForPaySDK.Handlers;
using WayForPaySDK.Serialization;

namespace WayForPaySDK.Extensions;

/// <summary>
/// Extension methods for integrating IWebhookHandler with ASP.NET Core.
/// </summary>
public static class WebhookHandlerExtensions
{
    /// <summary>
    /// Parses webhook payload from HttpRequest.
    /// </summary>
    /// <param name="handler">Webhook handler.</param>
    /// <param name="request">HTTP request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Validated webhook payload.</returns>
    /// <exception cref="ArgumentNullException">Thrown when handler or request is null.</exception>
    public static async Task<WebhookPayload> ParseAsync(
        this IWebhookHandler handler,
        HttpRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(request);

        // Enable request body buffering to allow multiple reads
        request.EnableBuffering();

        return await handler.ParseAsync(request.Body, cancellationToken);
    }

    /// <summary>
    /// Converts WebhookResponse to IActionResult for ASP.NET Core controllers.
    /// </summary>
    /// <param name="response">Webhook response.</param>
    /// <returns>IActionResult with JSON content.</returns>
    /// <exception cref="ArgumentNullException">Thrown when response is null.</exception>
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
    /// Handles webhook processing with automatic parsing, processing, and response generation.
    /// Simplifies webhook handling to a single method call.
    /// </summary>
    /// <param name="handler">Webhook handler.</param>
    /// <param name="request">HTTP request.</param>
    /// <param name="processPayload">Function to process the validated payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>IActionResult with accept or decline response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <remarks>
    /// If processPayload throws an exception, a decline response is automatically returned.
    /// If processPayload completes successfully, an accept response is returned.
    /// </remarks>
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
            // Use a minimal decline response with empty orderReference
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

    /// <summary>
    /// Handles webhook processing with automatic parsing, processing, and response generation.
    /// Allows the processor to return custom WebhookStatus.
    /// </summary>
    /// <param name="handler">Webhook handler.</param>
    /// <param name="request">HTTP request.</param>
    /// <param name="processPayload">Function to process the validated payload and return status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>IActionResult with custom status response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
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
