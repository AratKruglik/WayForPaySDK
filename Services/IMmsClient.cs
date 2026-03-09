using WayForPaySDK.Responses;

namespace WayForPaySDK.Services;

public interface IMmsClient
{
    Task<AddPartnerResponse> AddPartnerAsync(
        string partnerCode,
        string site,
        string phone,
        string email,
        string? description = null,
        string? compensationCardNumber = null,
        string? compensationCardExpYear = null,
        string? compensationCardExpMonth = null,
        string? compensationCardCvv = null,
        string? compensationCardHolder = null,
        string? compensationCardToken = null,
        string? compensationAccount = null,
        string? compensationAccountIban = null,
        string? compensationAccountMfo = null,
        string? compensationAccountOkpo = null,
        string? compensationAccountName = null,
        CancellationToken cancellationToken = default);

    Task<PartnerInfoResponse> GetPartnerInfoAsync(
        string partnerCode,
        CancellationToken cancellationToken = default);

    Task<UpdatePartnerResponse> UpdatePartnerAsync(
        string partnerCode,
        string? site = null,
        string? phone = null,
        string? email = null,
        string? description = null,
        string? compensationCardNumber = null,
        string? compensationCardExpYear = null,
        string? compensationCardExpMonth = null,
        string? compensationCardCvv = null,
        string? compensationCardHolder = null,
        string? compensationCardToken = null,
        string? compensationAccount = null,
        string? compensationAccountIban = null,
        string? compensationAccountMfo = null,
        string? compensationAccountOkpo = null,
        string? compensationAccountName = null,
        CancellationToken cancellationToken = default);

    Task<AddMerchantResponse> AddMerchantAsync(
        string site,
        string phone,
        string email,
        string? description = null,
        string? compensationCardNumber = null,
        string? compensationCardExpYear = null,
        string? compensationCardExpMonth = null,
        string? compensationCardCvv = null,
        string? compensationCardHolder = null,
        string? compensationCardToken = null,
        string? compensationAccount = null,
        string? compensationAccountIban = null,
        string? compensationAccountMfo = null,
        string? compensationAccountOkpo = null,
        string? compensationAccountName = null,
        CancellationToken cancellationToken = default);

    Task<MerchantInfoResponse> GetMerchantInfoAsync(
        CancellationToken cancellationToken = default);

    Task<MerchantBalanceResponse> GetMerchantBalanceAsync(
        string? toDate = null,
        CancellationToken cancellationToken = default);
}
