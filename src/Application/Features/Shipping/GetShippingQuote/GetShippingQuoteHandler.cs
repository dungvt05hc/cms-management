// <copyright file="GetShippingQuoteHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Shipping.GetShippingQuote;

/// <summary>
/// Handler for GetShippingQuoteQuery.
/// </summary>
public class GetShippingQuoteHandler
{
    private readonly IAppDbContext dbContext;
    private readonly IShippingQuoteProvider shippingQuoteProvider;
    private readonly ILogger<GetShippingQuoteHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetShippingQuoteHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="shippingQuoteProvider">The shipping quote provider.</param>
    /// <param name="logger">The logger.</param>
    public GetShippingQuoteHandler(
        IAppDbContext dbContext,
        IShippingQuoteProvider shippingQuoteProvider,
        ILogger<GetShippingQuoteHandler> logger)
    {
        this.dbContext = dbContext;
        this.shippingQuoteProvider = shippingQuoteProvider;
        this.logger = logger;
    }

    /// <summary>
    /// Handles getting a shipping quote.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Shipping quote.</returns>
    public async Task<ShippingQuoteDto> Handle(
        GetShippingQuoteQuery query,
        CancellationToken cancellationToken)
    {
        var sanitizedMethodCode = (query.MethodCode ?? string.Empty)
            .Replace(Environment.NewLine, string.Empty)
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty);
        var sanitizedCarrierCode = (query.CarrierCode ?? string.Empty)
            .Replace(Environment.NewLine, string.Empty)
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty);

        this.logger.LogInformation(
            "Getting shipping quote: MethodCode={MethodCode}, CarrierCode={CarrierCode}, AddressId={AddressId}",
            sanitizedMethodCode,
            sanitizedCarrierCode,
            query.AddressId);

        // Verify method exists and is active
        var method = await this.dbContext.ShippingMethods
            .Include(m => m.Carriers)
            .FirstOrDefaultAsync(m => m.Code == query.MethodCode && m.IsActive, cancellationToken);

        if (method == null)
        {
            throw new InvalidOperationException($"Shipping method '{query.MethodCode}' not found or inactive.");
        }

        // Verify carrier exists for this method and is active
        var carrier = method.Carriers.FirstOrDefault(c => c.Code == query.CarrierCode && c.IsActive);
        if (carrier == null)
        {
            throw new InvalidOperationException($"Carrier '{query.CarrierCode}' not found or inactive for method '{query.MethodCode}'.");
        }

        // Verify address belongs to user
        var address = await this.dbContext.Addresses
            .FirstOrDefaultAsync(a => a.Id == query.AddressId && a.UserId == query.UserId, cancellationToken);

        if (address == null)
        {
            throw new InvalidOperationException("Address not found or does not belong to user.");
        }

        // Get quote from provider
        var quote = await this.shippingQuoteProvider.GetQuoteAsync(
            query.MethodCode,
            query.CarrierCode,
            "Default City",
            address.City,
            address.District,
            address.Ward,
            query.Weight,
            cancellationToken);

        this.logger.LogInformation(
            "Shipping quote calculated: Fee={Fee}, ETA={ETA} days",
            quote.Fee,
            quote.EstimatedDeliveryDays);

        return new ShippingQuoteDto(quote.Fee, quote.EstimatedDeliveryDays);
    }
}
