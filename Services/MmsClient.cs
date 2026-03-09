using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using WayForPaySDK.Crypto;
using WayForPaySDK.Exceptions;
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

        request.MerchantSignature = _signatureGenerator.GenerateSignature(request.GetSignatureFields());

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

        request.MerchantSignature = _signatureGenerator.GenerateSignature(request.GetSignatureFields());

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

        request.MerchantSignature = _signatureGenerator.GenerateSignature(request.GetSignatureFields());

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

        request.MerchantSignature = _signatureGenerator.GenerateSignature(request.GetSignatureFields());

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

        request.MerchantSignature = _signatureGenerator.GenerateSignature(request.GetSignatureFields());

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

        request.MerchantSignature = _signatureGenerator.GenerateSignature(request.GetSignatureFields());

        return await SendMmsRequestAsync<MerchantBalanceRequest, MerchantBalanceResponse>(request, cancellationToken);
    }

    private async Task<TResponse> SendMmsRequestAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken)
        where TRequest : MmsRequest
        where TResponse : MmsResponse
    {
        try
        {
            var mmsApiUrl = ApiUrlBuilder.BuildAlternateUrl(
                _options.ApiBaseUrl, $"/mms/{request.MmsOperation}.php");

            var response = await _httpClient.PostAsJsonAsync(
                mmsApiUrl,
                request,
                _jsonOptions,
                cancellationToken);

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(
                    $"HTTP request failed with status {(int)response.StatusCode}.");
            }

            var result = JsonSerializer.Deserialize<TResponse>(content, _jsonOptions);

            if (result is null)
            {
                throw new JsonParseException("Response deserialized to null.");
            }

            return result;
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException($"HTTP request failed: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            throw new JsonParseException($"Failed to parse response: {ex.Message}", ex);
        }
    }

}
