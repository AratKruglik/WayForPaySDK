using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using WayForPaySDK.Crypto;
using WayForPaySDK.Domain;
using WayForPaySDK.Exceptions;
using WayForPaySDK.Options;
using WayForPaySDK.Requests;
using WayForPaySDK.Responses;
using WayForPaySDK.Serialization;

namespace WayForPaySDK.Services;

public sealed class WayForPayClient : IWayForPayClient
{
    private readonly HttpClient _httpClient;
    private readonly WayForPayOptions _options;
    private readonly ISignatureGenerator _signatureGenerator;
    private readonly JsonSerializerOptions _jsonOptions;

    public WayForPayClient(
        HttpClient httpClient,
        IOptions<WayForPayOptions> options,
        ISignatureGenerator signatureGenerator)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _signatureGenerator = signatureGenerator;
        _jsonOptions = new JsonSerializerOptions(WayForPayJsonContext.Default.Options);
    }

    public async Task<ChargeResponse> ChargeAsync(
        string orderReference,
        decimal amount,
        string currency,
        Card card,
        IEnumerable<Product> products,
        Client? client = null,
        string? serviceUrl = null,
        CancellationToken cancellationToken = default)
    {
        var productList = products.ToList();

        var request = new ChargeRequest
        {
            MerchantAccount = _options.MerchantAccount,
            MerchantDomainName = _options.MerchantDomainName,
            MerchantSignature = string.Empty, // Will be set below
            OrderReference = orderReference,
            OrderDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Amount = amount,
            Currency = currency.ToUpperInvariant(),
            CardNumber = card.Number,
            ExpMonth = card.ExpireMonth.ToString("D2"),
            ExpYear = card.ExpireYear.ToString(),
            CardCvv = card.Cvv,
            CardHolder = card.Holder,
            ProductName = productList.Select(p => p.Name).ToArray(),
            ProductPrice = productList.Select(p => p.Price).ToArray(),
            ProductCount = productList.Select(p => p.Count).ToArray(),
            ClientFirstName = client?.FirstName,
            ClientLastName = client?.LastName,
            ClientEmail = client?.Email,
            ClientPhone = client?.Phone,
            ClientCountry = client?.Country,
            ClientIpAddress = client?.IpAddress,
            ServiceUrl = serviceUrl
        };

        request.MerchantSignature = _signatureGenerator.GenerateSignature(request.GetSignatureFields());

        return await SendRequestAsync<ChargeRequest, ChargeResponse>(request, cancellationToken);
    }

    public async Task<ChargeResponse> ChargeWithTokenAsync(
        string orderReference,
        decimal amount,
        string currency,
        string recToken,
        IEnumerable<Product> products,
        Client? client = null,
        string? serviceUrl = null,
        CancellationToken cancellationToken = default)
    {
        var productList = products.ToList();

        var request = new ChargeRequest
        {
            MerchantAccount = _options.MerchantAccount,
            MerchantDomainName = _options.MerchantDomainName,
            MerchantSignature = string.Empty,
            OrderReference = orderReference,
            OrderDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Amount = amount,
            Currency = currency.ToUpperInvariant(),
            RecToken = recToken,
            ProductName = productList.Select(p => p.Name).ToArray(),
            ProductPrice = productList.Select(p => p.Price).ToArray(),
            ProductCount = productList.Select(p => p.Count).ToArray(),
            ClientFirstName = client?.FirstName,
            ClientLastName = client?.LastName,
            ClientEmail = client?.Email,
            ClientPhone = client?.Phone,
            ClientCountry = client?.Country,
            ClientIpAddress = client?.IpAddress,
            ServiceUrl = serviceUrl
        };

        request.MerchantSignature = _signatureGenerator.GenerateSignature(request.GetSignatureFields());

        return await SendRequestAsync<ChargeRequest, ChargeResponse>(request, cancellationToken);
    }

    public async Task<RefundResponse> RefundAsync(
        string orderReference,
        decimal amount,
        string currency,
        string comment,
        CancellationToken cancellationToken = default)
    {
        var request = new RefundRequest
        {
            MerchantAccount = _options.MerchantAccount,
            MerchantSignature = string.Empty,
            OrderReference = orderReference,
            Amount = amount,
            Currency = currency.ToUpperInvariant(),
            Comment = comment
        };

        request.MerchantSignature = _signatureGenerator.GenerateSignature(request.GetSignatureFields());

        return await SendRequestAsync<RefundRequest, RefundResponse>(request, cancellationToken);
    }

    public async Task<CheckStatusResponse> CheckStatusAsync(
        string orderReference,
        CancellationToken cancellationToken = default)
    {
        var request = new CheckStatusRequest
        {
            MerchantAccount = _options.MerchantAccount,
            MerchantSignature = string.Empty,
            OrderReference = orderReference
        };

        request.MerchantSignature = _signatureGenerator.GenerateSignature(request.GetSignatureFields());

        return await SendRequestAsync<CheckStatusRequest, CheckStatusResponse>(request, cancellationToken);
    }

    public async Task<SettleResponse> SettleAsync(
        string orderReference,
        decimal amount,
        string currency,
        CancellationToken cancellationToken = default)
    {
        var request = new SettleRequest
        {
            MerchantAccount = _options.MerchantAccount,
            MerchantSignature = string.Empty,
            OrderReference = orderReference,
            Amount = amount,
            Currency = currency.ToUpperInvariant()
        };

        request.MerchantSignature = _signatureGenerator.GenerateSignature(request.GetSignatureFields());

        return await SendRequestAsync<SettleRequest, SettleResponse>(request, cancellationToken);
    }

    public async Task<VoidResponse> VoidAsync(
        string orderReference,
        decimal amount,
        string currency,
        string? comment = null,
        CancellationToken cancellationToken = default)
    {
        var request = new VoidRequest
        {
            MerchantAccount = _options.MerchantAccount,
            MerchantSignature = string.Empty,
            OrderReference = orderReference,
            Amount = amount,
            Currency = currency.ToUpperInvariant(),
            Comment = comment
        };

        request.MerchantSignature = _signatureGenerator.GenerateSignature(request.GetSignatureFields());

        return await SendRequestAsync<VoidRequest, VoidResponse>(request, cancellationToken);
    }

    public async Task<PurchaseResponse> CreatePurchaseAsync(
        string orderReference,
        decimal amount,
        string currency,
        IEnumerable<Product> products,
        Client? client = null,
        string? returnUrl = null,
        string? serviceUrl = null,
        string? language = null,
        CancellationToken cancellationToken = default)
    {
        var productList = products.ToList();

        var request = new PurchaseRequest
        {
            MerchantAccount = _options.MerchantAccount,
            MerchantDomainName = _options.MerchantDomainName,
            MerchantSignature = string.Empty,
            OrderReference = orderReference,
            OrderDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Amount = amount,
            Currency = currency.ToUpperInvariant(),
            ProductName = productList.Select(p => p.Name).ToArray(),
            ProductPrice = productList.Select(p => p.Price).ToArray(),
            ProductCount = productList.Select(p => p.Count).ToArray(),
            ClientFirstName = client?.FirstName,
            ClientLastName = client?.LastName,
            ClientEmail = client?.Email,
            ClientPhone = client?.Phone,
            ClientCountry = client?.Country,
            ReturnUrl = returnUrl,
            ServiceUrl = serviceUrl,
            Language = language
        };

        request.MerchantSignature = _signatureGenerator.GenerateSignature(request.GetSignatureFields());

        return await SendRequestAsync<PurchaseRequest, PurchaseResponse>(request, cancellationToken);
    }

    public async Task<InvoiceResponse> CreateInvoiceAsync(
        string orderReference,
        decimal amount,
        string currency,
        IEnumerable<Product> products,
        Client? client = null,
        string? returnUrl = null,
        string? serviceUrl = null,
        string? language = null,
        int? orderLifetime = null,
        CancellationToken cancellationToken = default)
    {
        var productList = products.ToList();

        var request = new InvoiceRequest
        {
            MerchantAccount = _options.MerchantAccount,
            MerchantDomainName = _options.MerchantDomainName,
            MerchantSignature = string.Empty,
            OrderReference = orderReference,
            OrderDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Amount = amount,
            Currency = currency.ToUpperInvariant(),
            ProductName = productList.Select(p => p.Name).ToArray(),
            ProductPrice = productList.Select(p => p.Price).ToArray(),
            ProductCount = productList.Select(p => p.Count).ToArray(),
            ClientFirstName = client?.FirstName,
            ClientLastName = client?.LastName,
            ClientEmail = client?.Email,
            ClientPhone = client?.Phone,
            ReturnUrl = returnUrl,
            ServiceUrl = serviceUrl,
            Language = language,
            OrderLifetime = orderLifetime
        };

        request.MerchantSignature = _signatureGenerator.GenerateSignature(request.GetSignatureFields());

        return await SendRequestAsync<InvoiceRequest, InvoiceResponse>(request, cancellationToken);
    }

    public async Task<Complete3DSResponse> Complete3DSAsync(
        string d3Md,
        string d3Pares,
        CancellationToken cancellationToken = default)
    {
        var request = new Complete3DSRequest
        {
            MerchantAccount = _options.MerchantAccount,
            MerchantDomainName = _options.MerchantDomainName,
            MerchantSignature = string.Empty,
            D3Md = d3Md,
            D3Pares = d3Pares
        };

        request.MerchantSignature = _signatureGenerator.GenerateSignature(request.GetSignatureFields());

        return await SendRequestAsync<Complete3DSRequest, Complete3DSResponse>(request, cancellationToken);
    }

    public async Task<VerifyResponse> VerifyAsync(
        string orderReference,
        Card card,
        Client? client = null,
        CancellationToken cancellationToken = default)
    {
        var request = new VerifyRequest
        {
            MerchantAccount = _options.MerchantAccount,
            MerchantDomainName = _options.MerchantDomainName,
            MerchantSignature = string.Empty,
            OrderReference = orderReference,
            OrderDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            CardNumber = card.Number,
            ExpMonth = card.ExpireMonth.ToString("D2"),
            ExpYear = card.ExpireYear.ToString(),
            CardCvv = card.Cvv,
            CardHolder = card.Holder,
            ClientFirstName = client?.FirstName,
            ClientLastName = client?.LastName,
            ClientEmail = client?.Email,
            ClientPhone = client?.Phone,
            ClientCountry = client?.Country,
            ClientIpAddress = client?.IpAddress
        };

        request.MerchantSignature = _signatureGenerator.GenerateSignature(request.GetSignatureFields());

        return await SendRequestAsync<VerifyRequest, VerifyResponse>(request, cancellationToken);
    }

    public async Task<TransactionListResponse> GetTransactionListAsync(
        DateTimeOffset dateBegin,
        DateTimeOffset dateEnd,
        CancellationToken cancellationToken = default)
    {
        var request = new TransactionListRequest
        {
            MerchantAccount = _options.MerchantAccount,
            MerchantDomainName = _options.MerchantDomainName,
            MerchantSignature = string.Empty,
            DateBegin = dateBegin.ToUnixTimeSeconds(),
            DateEnd = dateEnd.ToUnixTimeSeconds()
        };

        request.MerchantSignature = _signatureGenerator.GenerateSignature(request.GetSignatureFields());

        return await SendRequestAsync<TransactionListRequest, TransactionListResponse>(request, cancellationToken);
    }

    public async Task<ChargeResponse> ChargeWithRegularAsync(
        string orderReference,
        decimal amount,
        string currency,
        Card card,
        IEnumerable<Product> products,
        Regular regular,
        Client? client = null,
        string? serviceUrl = null,
        CancellationToken cancellationToken = default)
    {
        var productList = products.ToList();

        var request = new ChargeRequest
        {
            MerchantAccount = _options.MerchantAccount,
            MerchantDomainName = _options.MerchantDomainName,
            MerchantSignature = string.Empty,
            OrderReference = orderReference,
            OrderDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Amount = amount,
            Currency = currency.ToUpperInvariant(),
            CardNumber = card.Number,
            ExpMonth = card.ExpireMonth.ToString("D2"),
            ExpYear = card.ExpireYear.ToString(),
            CardCvv = card.Cvv,
            CardHolder = card.Holder,
            ProductName = productList.Select(p => p.Name).ToArray(),
            ProductPrice = productList.Select(p => p.Price).ToArray(),
            ProductCount = productList.Select(p => p.Count).ToArray(),
            ClientFirstName = client?.FirstName,
            ClientLastName = client?.LastName,
            ClientEmail = client?.Email,
            ClientPhone = client?.Phone,
            ClientCountry = client?.Country,
            ClientIpAddress = client?.IpAddress,
            ServiceUrl = serviceUrl,
            RegularAmount = regular.Amount,
            RegularMode = regular.Modes.Select(m => m.ToString().ToLowerInvariant()).ToArray(),
            RegularOn = regular.DateNext?.ToString("yyyy-MM-dd"),
            RegularCount = regular.Count,
            RegularBehavior = regular.Behavior?.ToString().ToLowerInvariant()
        };

        request.MerchantSignature = _signatureGenerator.GenerateSignature(request.GetSignatureFields());

        return await SendRequestAsync<ChargeRequest, ChargeResponse>(request, cancellationToken);
    }

    public async Task<PurchaseResponse> CreatePurchaseWithRegularAsync(
        string orderReference,
        decimal amount,
        string currency,
        IEnumerable<Product> products,
        Regular regular,
        Client? client = null,
        string? returnUrl = null,
        string? serviceUrl = null,
        string? language = null,
        CancellationToken cancellationToken = default)
    {
        var productList = products.ToList();

        var request = new PurchaseRequest
        {
            MerchantAccount = _options.MerchantAccount,
            MerchantDomainName = _options.MerchantDomainName,
            MerchantSignature = string.Empty,
            OrderReference = orderReference,
            OrderDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Amount = amount,
            Currency = currency.ToUpperInvariant(),
            ProductName = productList.Select(p => p.Name).ToArray(),
            ProductPrice = productList.Select(p => p.Price).ToArray(),
            ProductCount = productList.Select(p => p.Count).ToArray(),
            ClientFirstName = client?.FirstName,
            ClientLastName = client?.LastName,
            ClientEmail = client?.Email,
            ClientPhone = client?.Phone,
            ClientCountry = client?.Country,
            ReturnUrl = returnUrl,
            ServiceUrl = serviceUrl,
            Language = language,
            RegularAmount = regular.Amount,
            RegularMode = regular.Modes.Select(m => m.ToString().ToLowerInvariant()).ToArray(),
            RegularOn = regular.DateNext?.ToString("yyyy-MM-dd"),
            RegularCount = regular.Count,
            RegularBehavior = regular.Behavior?.ToString().ToLowerInvariant()
        };

        request.MerchantSignature = _signatureGenerator.GenerateSignature(request.GetSignatureFields());

        return await SendRequestAsync<PurchaseRequest, PurchaseResponse>(request, cancellationToken);
    }

    private async Task<TResponse> SendRequestAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken)
        where TRequest : ApiRequest
        where TResponse : ApiResponse
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                string.Empty,
                request,
                _jsonOptions,
                cancellationToken);

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(
                    $"HTTP request failed with status {(int)response.StatusCode}: {content}");
            }

            var result = JsonSerializer.Deserialize<TResponse>(content, _jsonOptions);

            if (result is null)
            {
                throw new JsonParseException("Response deserialized to null.", content);
            }

            // Verify response signature
            if (!string.IsNullOrEmpty(result.MerchantSignature))
            {
                var expectedSignature = _signatureGenerator.GenerateSignature(result.GetSignatureFields());

                if (!_signatureGenerator.VerifySignature(expectedSignature, result.MerchantSignature))
                {
                    throw new SignatureException(expectedSignature, result.MerchantSignature);
                }
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
