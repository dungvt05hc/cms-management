// <copyright file="CheckoutSubmitCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Domain.Entities;

namespace Application.Features.Checkout.Submit;

/// <summary>
/// Command to submit checkout and create order or initiate payment.
/// </summary>
/// <param name="UserId">The authenticated user ID.</param>
/// <param name="AddressId">The shipping address ID.</param>
/// <param name="ShippingMethodCode">The shipping method code.</param>
/// <param name="ShippingCarrierCode">The shipping carrier code.</param>
/// <param name="DiscountCode">Optional discount voucher code.</param>
/// <param name="ShippingCode">Optional shipping voucher code.</param>
/// <param name="PaymentMethod">The payment method (COD or Payoo).</param>
/// <param name="Notes">Optional buyer notes.</param>
public record CheckoutSubmitCommand(
    Guid UserId,
    Guid AddressId,
    string ShippingMethodCode,
    string ShippingCarrierCode,
    string? DiscountCode,
    string? ShippingCode,
    PaymentMethod PaymentMethod,
    string? Notes);
