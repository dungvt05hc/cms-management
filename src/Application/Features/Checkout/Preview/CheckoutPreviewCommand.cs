// <copyright file="CheckoutPreviewCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Checkout.Preview;

/// <summary>
/// Command to preview checkout totals.
/// </summary>
/// <param name="UserId">The authenticated user ID.</param>
/// <param name="AddressId">The shipping address ID.</param>
/// <param name="ShippingMethodCode">The shipping method code.</param>
/// <param name="ShippingCarrierCode">The shipping carrier code.</param>
/// <param name="DiscountCode">Optional discount voucher code.</param>
/// <param name="ShippingCode">Optional shipping voucher code.</param>
public record CheckoutPreviewCommand(
    Guid UserId,
    Guid AddressId,
    string ShippingMethodCode,
    string ShippingCarrierCode,
    string? DiscountCode,
    string? ShippingCode);
