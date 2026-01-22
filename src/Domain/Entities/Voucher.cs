// <copyright file="Voucher.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Domain.Entities;

/// <summary>
/// Represents a voucher that can be applied to orders for discounts or shipping promotions.
/// </summary>
public class Voucher
{
    /// <summary>
    /// Gets or sets the unique identifier for the voucher.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the voucher code (unique, user-entered).
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the voucher type (Discount or Shipping).
    /// </summary>
    public VoucherType Type { get; set; }

    /// <summary>
    /// Gets or sets the discount amount (fixed amount for MVP).
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the voucher is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the minimum order amount required to use this voucher (optional).
    /// </summary>
    public decimal? MinimumOrderAmount { get; set; }

    /// <summary>
    /// Gets or sets the date and time the voucher was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time the voucher was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
