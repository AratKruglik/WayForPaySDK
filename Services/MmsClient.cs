using System.Text.Json;
using Microsoft.Extensions.Options;
using WayForPaySDK.Crypto;
using WayForPaySDK.Http;
using WayForPaySDK.Options;
using WayForPaySDK.Requests;
using WayForPaySDK.Responses;
using WayForPaySDK.Serialization;

namespace WayForPaySDK.Services;

public sealed class MmsClient : IMmsClient
{
    private readonly HttpClient _httpClient;
    private readonly WayForPayOptions _options;
    private readonly ISignatureGenerator _signatureGenerator;
    private readonly JsonSerializerOptions _jsonOptions;

    public MmsClient(
        HttpClient httpClient,
        IOptions<WayForPayOptions> options,
        ISignatureGenerator signatureGenerator)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _signatureGenerator = signatureGenerator;
        _jsonOptions = new JsonSerializerOptions(WayForPayJsonContext.Default.Options);
    }

    public async Task<AddPartnerResponse> AddPartnerAsync(
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
        CancellationToken cancellationToken = default)
    {
        var request = new AddPartnerRequest
        {
            MerchantAccount = _options.MerchantAccount,
            MerchantSignature = string.Empty,
            PartnerCode = partnerCode,
            Site = site,
            Phone = phone,
            Email = email,
            Description = description,
            CompensationCardNumber = compensationCardNumber,
            CompensationCardExpYear = compensationCardExpYear,
            CompensationCardExpMonth = compensationCardExpMonth,
            CompensationCardCvv = compensationCardCvv,
            CompensationCardHolder = compensationCardHolder,
            CompensationCardToken = compensationCardToken,
            CompensationAccount = compensationAccount,
            CompensationAccountIban = compensationAccountIban,
            CompensationAccountMfo = compensationAccountMfo,
            CompensationAccountOkpo = compensationAccountOkpo,
            CompensationAccountName = compensationAccountName
        };

        SignRequest(request);

        return await SendMmsRequestAsync<AddPartnerRequest, AddPartnerResponse>(request, cancellationToken);
    }

    public async Task<PartnerInfoResponse> GetPartnerInfoAsync(
        string partnerCode,
        CancellationToken cancellationToken = default)
    {
        var request = new PartnerInfoRequest
        {
            MerchantAccount = _options.MerchantAccount,
            MerchantSignature = string.Empty,
            PartnerCode = partnerCode
        };

        SignRequest(request);

        return await SendMmsRequestAsync<PartnerInfoRequest, PartnerInfoResponse>(request, cancellationToken);
    }

    public async Task<UpdatePartnerResponse> UpdatePartnerAsync(
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
        CancellationToken cancellationToken = default)
    {
        var request = new UpdatePartnerRequest
        {
            MerchantAccount = _options.MerchantAccount,
            MerchantSignature = string.Empty,
            PartnerCode = partnerCode,
            Site = site,
            Phone = phone,
            Email = email,
            Description = description,
            CompensationCardNumber = compensationCardNumber,
            CompensationCardExpYear = compensationCardExpYear,
            CompensationCardExpMonth = compensationCardExpMonth,
            CompensationCardCvv = compensationCardCvv,
            CompensationCardHolder = compensationCardHolder,
            CompensationCardToken = compensationCardToken,
            CompensationAccount = compensationAccount,
            CompensationAccountIban = compensationAccountIban,
            CompensationAccountMfo = compensationAccountMfo,
            CompensationAccountOkpo = compensationAccountOkpo,
            CompensationAccountName = compensationAccountName
        };

        SignRequest(request);

        return await SendMmsRequestAsync<UpdatePartnerRequest, UpdatePartnerResponse>(request, cancellationToken);
    }

    public async Task<AddMerchantResponse> AddMerchantAsync(
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
        CancellationToken cancellationToken = default)
    {
        var request = new AddMerchantRequest
        {
            MerchantAccount = _options.MerchantAccount,
            MerchantSignature = string.Empty,
            Site = site,
            Phone = phone,
            Email = email,
            Description = description,
            CompensationCardNumber = compensationCardNumber,
            CompensationCardExpYear = compensationCardExpYear,
            CompensationCardExpMonth = compensationCardExpMonth,
            CompensationCardCvv = compensationCardCvv,
            CompensationCardHolder = compensationCardHolder,
            CompensationCardToken = compensationCardToken,
            CompensationAccount = compensationAccount,
            CompensationAccountIban = compensationAccountIban,
            CompensationAccountMfo = compensationAccountMfo,
            CompensationAccountOkpo = compensationAccountOkpo,
            CompensationAccountName = compensationAccountName
        };

        SignRequest(request);

        return await SendMmsRequestAsync<AddMerchantRequest, AddMerchantResponse>(request, cancellationToken);
    }

    public async Task<MerchantInfoResponse> GetMerchantInfoAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new MerchantInfoRequest
        {
            MerchantAccount = _options.MerchantAccount,
            MerchantSignature = string.Empty
        };

        SignRequest(request);

        return await SendMmsRequestAsync<MerchantInfoRequest, MerchantInfoResponse>(request, cancellationToken);
    }

    public async Task<MerchantBalanceResponse> GetMerchantBalanceAsync(
        string? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var request = new MerchantBalanceRequest
        {
            MerchantAccount = _options.MerchantAccount,
            MerchantSignature = string.Empty,
            ToDate = toDate
        };

        SignRequest(request);

        return await SendMmsRequestAsync<MerchantBalanceRequest, MerchantBalanceResponse>(request, cancellationToken);
    }

    private void SignRequest(MmsRequest request)
    {
        request.MerchantSignature = _signatureGenerator.GenerateSignature(request.GetSignatureFields());
    }

    private async Task<TResponse> SendMmsRequestAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken)
        where TRequest : MmsRequest
        where TResponse : MmsResponse
    {
        var mmsApiUrl = ApiUrlBuilder.BuildAlternateUrl(
            _options.ApiBaseUrl, $"/mms/{request.MmsOperation}.php");

        return await ApiRequestSender.SendAsync<TRequest, TResponse>(
            _httpClient, mmsApiUrl, request, _jsonOptions, cancellationToken);
    }
}
