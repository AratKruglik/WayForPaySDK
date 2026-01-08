namespace WayForPaySDK.Handlers;

/// <summary>
/// Represents the status of webhook processing response.
/// </summary>
public enum WebhookStatus
{
    /// <summary>
    /// Webhook was successfully processed and accepted.
    /// </summary>
    Accept,

    /// <summary>
    /// Webhook was rejected (e.g., order not found, duplicate processing).
    /// </summary>
    Decline
}
