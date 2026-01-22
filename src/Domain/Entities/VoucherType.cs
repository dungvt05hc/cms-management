// <copyright file="VoucherType.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Domain.Entities;

/// <summary>
/// Voucher type enumeration.
/// </summary>
public enum VoucherType
{
    /// <summary>
    /// Discount on product subtotal.
    /// </summary>
    Discount = 0,

    /// <summary>
    /// Shipping fee reduction.
    /// </summary>
    Shipping = 1,
}
