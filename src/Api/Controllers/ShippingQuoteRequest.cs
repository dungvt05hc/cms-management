// <copyright file="ShippingQuoteRequest.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Api.Controllers;

/// <summary>
/// Request for shipping quote.
/// </summary>
/// <param name="AddressId">The delivery address ID.</param>
/// <param name="MethodCode">The shipping method code.</param>
/// <param name="CarrierCode">The carrier code.</param>
/// <param name="Weight">The total weight in grams.</param>
public record ShippingQuoteRequest(
    Guid AddressId,
    string MethodCode,
    string CarrierCode,
    int Weight);
