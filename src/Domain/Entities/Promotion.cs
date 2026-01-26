// <copyright file="Promotion.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Domain.Entities;

/// <summary>
/// Represents a promotion or discount coupon.
/// </summary>
public class Promotion
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the promotion code (e.g., "SUMMER20").
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the promotion name/title.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the discount type: "percentage" or "fixed".
    /// </summary>
    public string DiscountType { get; set; } = "percentage";

    /// <summary>
    /// Gets or sets the discount value (percentage 0-100 or fixed amount).
    /// </summary>
    public decimal DiscountValue { get; set; }

    /// <summary>
    /// Gets or sets the minimum order amount required to use this promotion.
    /// </summary>
    public decimal? MinOrderAmount { get; set; }

    /// <summary>
    /// Gets or sets the maximum discount amount (for percentage discounts).
    /// </summary>
    public decimal? MaxDiscountAmount { get; set; }

    /// <summary>
    /// Gets or sets the start date of the promotion.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of the promotion.
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of times this promotion can be used.
    /// </summary>
    public int? UsageLimit { get; set; }

    /// <summary>
    /// Gets or sets the maximum usage per customer.
    /// </summary>
    public int? UsageLimitPerCustomer { get; set; }

    /// <summary>
    /// Gets or sets the current usage count.
    /// </summary>
    public int UsedCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the promotion is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the date and time the promotion was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time the promotion was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
