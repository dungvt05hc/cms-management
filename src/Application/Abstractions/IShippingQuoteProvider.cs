// <copyright file="IShippingQuoteProvider.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Abstractions;

/// <summary>
/// Interface for shipping quote provider.
/// </summary>
public interface IShippingQuoteProvider
{
    /// <summary>
    /// Gets a shipping quote for the given parameters.
    /// </summary>
    /// <param name="methodCode">The shipping method code.</param>
    /// <param name="carrierCode">The shipping carrier code.</param>
    /// <param name="fromCity">The origin city.</param>
    /// <param name="toCity">The destination city.</param>
    /// <param name="toDistrict">The destination district.</param>
    /// <param name="toWard">The destination ward.</param>
    /// <param name="weight">The total weight in grams.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Shipping quote with fee and ETA.</returns>
    Task<ShippingQuote> GetQuoteAsync(
        string methodCode,
        string carrierCode,
        string fromCity,
        string toCity,
        string toDistrict,
        string toWard,
        int weight,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a shipping quote.
/// </summary>
/// <param name="Fee">Shipping fee in VND.</param>
/// <param name="EstimatedDeliveryDays">Estimated delivery time in days.</param>
public record ShippingQuote(decimal Fee, int EstimatedDeliveryDays);
