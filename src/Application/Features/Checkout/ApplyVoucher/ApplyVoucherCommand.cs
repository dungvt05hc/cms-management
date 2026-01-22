// <copyright file="ApplyVoucherCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Checkout.ApplyVoucher;

/// <summary>
/// Command to apply discount and/or shipping vouchers to calculate totals.
/// </summary>
/// <param name="UserId">The authenticated user ID.</param>
/// <param name="DiscountCode">Optional discount voucher code.</param>
/// <param name="ShippingCode">Optional shipping voucher code.</param>
public record ApplyVoucherCommand(
    Guid UserId,
    string? DiscountCode,
    string? ShippingCode);
