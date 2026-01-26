// <copyright file="AdminPromotionsController.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Application.Common;
using Application.Features.Promotions;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

/// <summary>
/// Admin promotions controller for managing discount codes and coupons.
/// </summary>
[ApiController]
[Route("admin/promotions")]
[Authorize(Policy = "AdminOnly")]
public class AdminPromotionsController : ControllerBase
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<AdminPromotionsController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminPromotionsController"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public AdminPromotionsController(
        IAppDbContext dbContext,
        ILogger<AdminPromotionsController> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Get all promotions with optional filtering.
    /// </summary>
    /// <param name="isActive">Filter by active status.</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="pageSize">Page size (default: 20).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of promotions.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PromotionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPromotions(
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = this.dbContext.Promotions.AsQueryable();

        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var promotions = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => MapToDto(p))
            .ToListAsync(cancellationToken);

        return this.Ok(new PagedResult<PromotionDto>
        {
            Items = promotions,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        });
    }

    /// <summary>
    /// Get a promotion by ID.
    /// </summary>
    /// <param name="id">The promotion ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The promotion details.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PromotionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPromotion(Guid id, CancellationToken cancellationToken)
    {
        var promotion = await this.dbContext.Promotions
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (promotion == null)
        {
            return this.NotFound(new { Message = "Promotion not found." });
        }

        return this.Ok(MapToDto(promotion));
    }

    /// <summary>
    /// Create a new promotion.
    /// </summary>
    /// <param name="request">The create promotion request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created promotion.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(PromotionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreatePromotion(
        [FromBody] CreatePromotionRequest request,
        CancellationToken cancellationToken)
    {
        // Validate request
        if (string.IsNullOrWhiteSpace(request.Code))
        {
            return this.BadRequest(new { Message = "Promotion code is required." });
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return this.BadRequest(new { Message = "Promotion name is required." });
        }

        if (request.DiscountValue <= 0)
        {
            return this.BadRequest(new { Message = "Discount value must be greater than 0." });
        }

        if (request.DiscountType == "percentage" && request.DiscountValue > 100)
        {
            return this.BadRequest(new { Message = "Percentage discount cannot exceed 100%." });
        }

        if (request.EndDate <= request.StartDate)
        {
            return this.BadRequest(new { Message = "End date must be after start date." });
        }

        // Check for duplicate code
        var existingCode = await this.dbContext.Promotions
            .AnyAsync(p => p.Code.ToUpper() == request.Code.ToUpper(), cancellationToken);

        if (existingCode)
        {
            return this.Conflict(new { Message = "A promotion with this code already exists." });
        }

        var now = DateTime.UtcNow;
        var promotion = new Promotion
        {
            Id = Guid.NewGuid(),
            Code = request.Code.ToUpper(),
            Name = request.Name,
            Description = request.Description,
            DiscountType = request.DiscountType,
            DiscountValue = request.DiscountValue,
            MinOrderAmount = request.MinOrderAmount,
            MaxDiscountAmount = request.MaxDiscountAmount,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            UsageLimit = request.UsageLimit,
            UsageLimitPerCustomer = request.UsageLimitPerCustomer,
            UsedCount = 0,
            IsActive = request.IsActive,
            CreatedAt = now,
            UpdatedAt = now,
        };

        this.dbContext.Promotions.Add(promotion);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Created promotion {Code} with ID {Id}", promotion.Code, promotion.Id);

        return this.CreatedAtAction(nameof(this.GetPromotion), new { id = promotion.Id }, MapToDto(promotion));
    }

    /// <summary>
    /// Update a promotion.
    /// </summary>
    /// <param name="id">The promotion ID.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated promotion.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(PromotionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePromotion(
        Guid id,
        [FromBody] UpdatePromotionRequest request,
        CancellationToken cancellationToken)
    {
        var promotion = await this.dbContext.Promotions
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (promotion == null)
        {
            return this.NotFound(new { Message = "Promotion not found." });
        }

        // Update fields if provided
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            promotion.Name = request.Name;
        }

        if (request.Description != null)
        {
            promotion.Description = request.Description;
        }

        if (!string.IsNullOrWhiteSpace(request.DiscountType))
        {
            promotion.DiscountType = request.DiscountType;
        }

        if (request.DiscountValue.HasValue)
        {
            if (request.DiscountValue <= 0)
            {
                return this.BadRequest(new { Message = "Discount value must be greater than 0." });
            }

            promotion.DiscountValue = request.DiscountValue.Value;
        }

        if (request.MinOrderAmount.HasValue)
        {
            promotion.MinOrderAmount = request.MinOrderAmount;
        }

        if (request.MaxDiscountAmount.HasValue)
        {
            promotion.MaxDiscountAmount = request.MaxDiscountAmount;
        }

        if (request.StartDate.HasValue)
        {
            promotion.StartDate = request.StartDate.Value;
        }

        if (request.EndDate.HasValue)
        {
            promotion.EndDate = request.EndDate.Value;
        }

        if (request.UsageLimit.HasValue)
        {
            promotion.UsageLimit = request.UsageLimit;
        }

        if (request.UsageLimitPerCustomer.HasValue)
        {
            promotion.UsageLimitPerCustomer = request.UsageLimitPerCustomer;
        }

        if (request.IsActive.HasValue)
        {
            promotion.IsActive = request.IsActive.Value;
        }

        promotion.UpdatedAt = DateTime.UtcNow;

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Updated promotion {Id}", id);

        return this.Ok(MapToDto(promotion));
    }

    /// <summary>
    /// Delete a promotion.
    /// </summary>
    /// <param name="id">The promotion ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePromotion(Guid id, CancellationToken cancellationToken)
    {
        var promotion = await this.dbContext.Promotions
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (promotion == null)
        {
            return this.NotFound(new { Message = "Promotion not found." });
        }

        this.dbContext.Promotions.Remove(promotion);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Deleted promotion {Id}", id);

        return this.NoContent();
    }

    /// <summary>
    /// Toggle promotion active status.
    /// </summary>
    /// <param name="id">The promotion ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated promotion.</returns>
    [HttpPost("{id}/toggle")]
    [ProducesResponseType(typeof(PromotionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TogglePromotion(Guid id, CancellationToken cancellationToken)
    {
        var promotion = await this.dbContext.Promotions
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (promotion == null)
        {
            return this.NotFound(new { Message = "Promotion not found." });
        }

        promotion.IsActive = !promotion.IsActive;
        promotion.UpdatedAt = DateTime.UtcNow;

        await this.dbContext.SaveChangesAsync(cancellationToken);

        return this.Ok(MapToDto(promotion));
    }

    private static PromotionDto MapToDto(Promotion p)
    {
        return new PromotionDto
        {
            Id = p.Id,
            Code = p.Code,
            Name = p.Name,
            Description = p.Description,
            DiscountType = p.DiscountType,
            DiscountValue = p.DiscountValue,
            MinOrderAmount = p.MinOrderAmount,
            MaxDiscountAmount = p.MaxDiscountAmount,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            UsageLimit = p.UsageLimit,
            UsageLimitPerCustomer = p.UsageLimitPerCustomer,
            UsedCount = p.UsedCount,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
        };
    }
}
