// <copyright file="PayooCallbackCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Payments.PayooCallback;

/// <summary>
/// Command to handle Payoo payment callback.
/// </summary>
/// <param name="PaymentReference">The payment reference from Payoo.</param>
/// <param name="UserId">The user ID from the payment session.</param>
/// <param name="AddressId">The shipping address ID.</param>
/// <param name="ShippingMethodCode">The shipping method code.</param>
/// <param name="ShippingCarrierCode">The shipping carrier code.</param>
/// <param name="DiscountCode">Optional discount voucher code.</param>
/// <param name="ShippingCode">Optional shipping voucher code.</param>
/// <param name="Notes">Optional buyer notes.</param>
public record PayooCallbackCommand(
    string PaymentReference,
    Guid UserId,
    Guid AddressId,
    string ShippingMethodCode,
    string ShippingCarrierCode,
    string? DiscountCode,
    string? ShippingCode,
    string? Notes);
