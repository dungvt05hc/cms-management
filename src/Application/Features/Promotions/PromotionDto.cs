// <copyright file="PromotionDto.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Promotions;

/// <summary>
/// Data transfer object for Promotion.
/// </summary>
public class PromotionDto
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the promotion code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the promotion name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the discount type (percentage or fixed).
    /// </summary>
    public string DiscountType { get; set; } = "percentage";

    /// <summary>
    /// Gets or sets the discount value.
    /// </summary>
    public decimal DiscountValue { get; set; }

    /// <summary>
    /// Gets or sets the minimum order amount.
    /// </summary>
    public decimal? MinOrderAmount { get; set; }

    /// <summary>
    /// Gets or sets the maximum discount amount.
    /// </summary>
    public decimal? MaxDiscountAmount { get; set; }

    /// <summary>
    /// Gets or sets the start date.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date.
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Gets or sets the usage limit.
    /// </summary>
    public int? UsageLimit { get; set; }

    /// <summary>
    /// Gets or sets the usage limit per customer.
    /// </summary>
    public int? UsageLimitPerCustomer { get; set; }

    /// <summary>
    /// Gets or sets the used count.
    /// </summary>
    public int UsedCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the promotion is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the created date.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the updated date.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets a value indicating whether the promotion is currently valid.
    /// </summary>
    public bool IsValid => this.IsActive && DateTime.UtcNow >= this.StartDate && DateTime.UtcNow <= this.EndDate
        && (this.UsageLimit == null || this.UsedCount < this.UsageLimit);
}
