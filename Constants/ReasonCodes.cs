namespace WayForPaySDK.Constants;

/// <summary>
/// WayForPay API reason codes.
/// </summary>
public static class ReasonCodes
{
    /// <summary>
    /// Transaction approved successfully.
    /// </summary>
    public const int Ok = 1100;

    /// <summary>
    /// Declined by card issuer.
    /// </summary>
    public const int DeclinedToCardIssuer = 1101;

    /// <summary>
    /// Invalid CVV2/CVC2 code.
    /// </summary>
    public const int BadCvv2 = 1102;

    /// <summary>
    /// Card has expired.
    /// </summary>
    public const int ExpiredCard = 1103;

    /// <summary>
    /// Insufficient funds on card.
    /// </summary>
    public const int InsufficientFunds = 1104;

    /// <summary>
    /// Invalid card number.
    /// </summary>
    public const int InvalidCard = 1105;

    /// <summary>
    /// Exceeded withdrawal frequency limit.
    /// </summary>
    public const int ExceedWithdrawalFrequency = 1106;

    /// <summary>
    /// 3D Secure authentication failed.
    /// </summary>
    public const int ThreeDsAuthFail = 1108;

    /// <summary>
    /// Request format error.
    /// </summary>
    public const int FormatError = 1109;

    /// <summary>
    /// Invalid currency specified.
    /// </summary>
    public const int InvalidCurrency = 1110;

    /// <summary>
    /// Duplicate order ID.
    /// </summary>
    public const int DuplicateOrderId = 1112;

    /// <summary>
    /// Invalid request signature.
    /// </summary>
    public const int InvalidSignature = 1113;

    /// <summary>
    /// Transaction declined due to fraud suspicion.
    /// </summary>
    public const int Fraud = 1114;

    /// <summary>
    /// Required parameter is missing.
    /// </summary>
    public const int ParameterMissing = 1115;

    /// <summary>
    /// Card token not found.
    /// </summary>
    public const int TokenNotFound = 1116;

    /// <summary>
    /// API method not allowed for merchant.
    /// </summary>
    public const int ApiNotAllowed = 1117;

    /// <summary>
    /// Merchant restriction applied.
    /// </summary>
    public const int MerchantRestriction = 1118;

    /// <summary>
    /// 3D Secure authentication unavailable.
    /// </summary>
    public const int AuthenticationUnavailable = 1120;

    /// <summary>
    /// Account not found.
    /// </summary>
    public const int AccountNotFound = 1121;

    /// <summary>
    /// Declined by payment gateway.
    /// </summary>
    public const int GateDeclined = 1122;

    /// <summary>
    /// Refund not allowed for this transaction.
    /// </summary>
    public const int RefundNotAllowed = 1123;

    /// <summary>
    /// Cardholder session has expired.
    /// </summary>
    public const int CardholderSessionExpired = 1124;

    /// <summary>
    /// Cardholder cancelled the request.
    /// </summary>
    public const int CardholderCancelledRequest = 1125;

    /// <summary>
    /// Illegal order state for this operation.
    /// </summary>
    public const int IllegalOrderState = 1126;

    /// <summary>
    /// Order not found.
    /// </summary>
    public const int OrderNotFound = 1127;

    /// <summary>
    /// Refund limit exceeded.
    /// </summary>
    public const int RefundLimitExceeded = 1128;

    /// <summary>
    /// Internal script error.
    /// </summary>
    public const int ScriptError = 1129;

    /// <summary>
    /// Invalid amount specified.
    /// </summary>
    public const int InvalidAmount = 1130;

    /// <summary>
    /// Transaction is currently being processed.
    /// </summary>
    public const int TransactionInProcessing = 1131;

    /// <summary>
    /// Transaction has been delayed.
    /// </summary>
    public const int TransactionDelayed = 1132;

    /// <summary>
    /// Invalid commission value.
    /// </summary>
    public const int InvalidCommission = 1133;

    /// <summary>
    /// Transaction is pending.
    /// </summary>
    public const int TransactionPending = 1134;

    /// <summary>
    /// Card limits check failed.
    /// </summary>
    public const int CardLimitsFailed = 1135;

    /// <summary>
    /// Merchant balance is too small.
    /// </summary>
    public const int MerchantBalanceSmall = 1136;

    /// <summary>
    /// Invalid confirmation amount.
    /// </summary>
    public const int InvalidConfirmationAmount = 1137;

    /// <summary>
    /// Refund is being processed.
    /// </summary>
    public const int RefundInProcessing = 1138;

    /// <summary>
    /// External decline while crediting.
    /// </summary>
    public const int ExternalDeclineWhileCredit = 1139;

    /// <summary>
    /// Exceeded withdrawal frequency while crediting.
    /// </summary>
    public const int ExceedWithdrawalFrequencyWhileCredit = 1140;

    /// <summary>
    /// Partial void is not supported.
    /// </summary>
    public const int PartialVoidNotSupported = 1141;

    /// <summary>
    /// Credit was refused.
    /// </summary>
    public const int RefusedCredit = 1142;

    /// <summary>
    /// Invalid phone number.
    /// </summary>
    public const int InvalidPhoneNumber = 1143;

    /// <summary>
    /// Transaction awaiting delivery confirmation.
    /// </summary>
    public const int TransactionAwaitingDelivery = 1144;

    /// <summary>
    /// Transaction awaiting decision.
    /// </summary>
    public const int TransactionAwaitingDecision = 1145;

    /// <summary>
    /// Card is restricted.
    /// </summary>
    public const int RestrictedCard = 1146;

    /// <summary>
    /// Client not found.
    /// </summary>
    public const int ClientNotFound = 1147;

    /// <summary>
    /// Client is not linked.
    /// </summary>
    public const int ClientNotLinked = 1148;

    /// <summary>
    /// Client account is locked.
    /// </summary>
    public const int ClientLocked = 1149;

    /// <summary>
    /// Regular payment approved.
    /// </summary>
    public const int OkRegular = 4100;

    /// <summary>
    /// Waiting for 3D Secure data.
    /// </summary>
    public const int Wait3DsData = 5100;

    /// <summary>
    /// Checks if the reason code indicates a successful transaction.
    /// </summary>
    public static bool IsSuccess(int code) =>
        code == Ok || code == OkRegular || code == Wait3DsData;

    /// <summary>
    /// Checks if the reason code indicates waiting for 3D Secure.
    /// </summary>
    public static bool IsWaiting3Ds(int code) =>
        code == Wait3DsData;
}
