// <copyright file="GetShippingQuoteQuery.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Shipping.GetShippingQuote;

/// <summary>
/// Query to get a shipping quote.
/// </summary>
/// <param name="UserId">The user ID.</param>
/// <param name="AddressId">The address ID.</param>
/// <param name="MethodCode">The shipping method code.</param>
/// <param name="CarrierCode">The carrier code.</param>
/// <param name="Weight">The total weight in grams.</param>
public record GetShippingQuoteQuery(
    Guid UserId,
    Guid AddressId,
    string MethodCode,
    string CarrierCode,
    int Weight);
