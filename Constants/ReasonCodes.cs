namespace WayForPaySDK.Constants;

public static class ReasonCodes
{
    public const int Ok = 1100;
    public const int DeclinedToCardIssuer = 1101;
    public const int BadCvv2 = 1102;
    public const int ExpiredCard = 1103;
    public const int InsufficientFunds = 1104;
    public const int InvalidCard = 1105;
    public const int ExceedWithdrawalFrequency = 1106;
    public const int ThreeDsAuthFail = 1108;
    public const int FormatError = 1109;
    public const int InvalidCurrency = 1110;
    public const int DuplicateOrderId = 1112;
    public const int InvalidSignature = 1113;
    public const int Fraud = 1114;
    public const int ParameterMissing = 1115;
    public const int TokenNotFound = 1116;
    public const int ApiNotAllowed = 1117;
    public const int MerchantRestriction = 1118;
    public const int AuthenticationUnavailable = 1120;
    public const int AccountNotFound = 1121;
    public const int GateDeclined = 1122;
    public const int RefundNotAllowed = 1123;
    public const int CardholderSessionExpired = 1124;
    public const int CardholderCancelledRequest = 1125;
    public const int IllegalOrderState = 1126;
    public const int OrderNotFound = 1127;
    public const int RefundLimitExceeded = 1128;
    public const int ScriptError = 1129;
    public const int InvalidAmount = 1130;
    public const int TransactionInProcessing = 1131;
    public const int TransactionDelayed = 1132;
    public const int InvalidCommission = 1133;
    public const int TransactionPending = 1134;
    public const int CardLimitsFailed = 1135;
    public const int MerchantBalanceSmall = 1136;
    public const int InvalidConfirmationAmount = 1137;
    public const int RefundInProcessing = 1138;
    public const int ExternalDeclineWhileCredit = 1139;
    public const int ExceedWithdrawalFrequencyWhileCredit = 1140;
    public const int PartialVoidNotSupported = 1141;
    public const int RefusedCredit = 1142;
    public const int InvalidPhoneNumber = 1143;
    public const int TransactionAwaitingDelivery = 1144;
    public const int TransactionAwaitingDecision = 1145;
    public const int RestrictedCard = 1146;
    public const int ClientNotFound = 1147;
    public const int ClientNotLinked = 1148;
    public const int ClientLocked = 1149;
    public const int OkRegular = 4100;
    public const int Wait3DsData = 5100;

    public static bool IsSuccess(int code) =>
        code == Ok || code == OkRegular || code == Wait3DsData;

    public static bool IsWaiting3Ds(int code) =>
        code == Wait3DsData;
}
