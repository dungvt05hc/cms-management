// <copyright file="ShippingQuoteDto.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Shipping.GetShippingQuote;

/// <summary>
/// DTO for shipping quote.
/// </summary>
/// <param name="Fee">Shipping fee in VND.</param>
/// <param name="EstimatedDeliveryDays">Estimated delivery time in days.</param>
public record ShippingQuoteDto(decimal Fee, int EstimatedDeliveryDays);
