// <copyright file="StubShippingQuoteProvider.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Stub implementation of shipping quote provider for development/testing.
/// </summary>
public class StubShippingQuoteProvider : IShippingQuoteProvider
{
    private readonly ILogger<StubShippingQuoteProvider> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StubShippingQuoteProvider"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public StubShippingQuoteProvider(ILogger<StubShippingQuoteProvider> logger)
    {
        this.logger = logger;
    }

    /// <inheritdoc/>
    public Task<ShippingQuote> GetQuoteAsync(
        string methodCode,
        string carrierCode,
        string fromCity,
        string toCity,
        string toDistrict,
        string toWard,
        int weight,
        CancellationToken cancellationToken = default)
    {
        this.logger.LogInformation(
            "Stub shipping quote requested: MethodCode={MethodCode}, CarrierCode={CarrierCode}, FromCity={FromCity}, ToCity={ToCity}",
            SanitizeForLogging(methodCode),
            SanitizeForLogging(carrierCode),
            SanitizeForLogging(fromCity),
            SanitizeForLogging(toCity));

        var fee = this.CalculateDeterministicFee(methodCode, carrierCode, weight);
        var eta = this.CalculateDeterministicEta(methodCode, carrierCode);

        return Task.FromResult(new ShippingQuote(fee, eta));
    }

    private static string SanitizeForLogging(string value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        return value
            .Replace("\r", string.Empty, StringComparison.Ordinal)
            .Replace("\n", string.Empty, StringComparison.Ordinal);
    }

    private decimal CalculateDeterministicFee(string methodCode, string carrierCode, int weight)
    {
        var baseFee = methodCode.ToUpperInvariant() switch
        {
            "STANDARD" => 25000m,
            "EXPRESS" => 45000m,
            _ => 30000m,
        };

        var carrierMultiplier = carrierCode.ToUpperInvariant() switch
        {
            "GHN" => 1.0m,
            "GHTK" => 0.95m,
            "VNPOST" => 0.85m,
            _ => 1.0m,
        };

        var weightFee = weight > 1000 ? (weight - 1000) / 500 * 5000m : 0m;

        return (baseFee * carrierMultiplier) + weightFee;
    }

    private int CalculateDeterministicEta(string methodCode, string carrierCode)
    {
        var baseEta = methodCode.ToUpperInvariant() switch
        {
            "STANDARD" => 5,
            "EXPRESS" => 2,
            _ => 3,
        };

        var carrierAdjustment = carrierCode.ToUpperInvariant() switch
        {
            "GHN" => 0,
            "GHTK" => 1,
            "VNPOST" => 2,
            _ => 0,
        };

        return baseEta + carrierAdjustment;
    }
}
