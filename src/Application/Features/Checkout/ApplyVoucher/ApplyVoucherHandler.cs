// <copyright file="ApplyVoucherHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Checkout.ApplyVoucher;

/// <summary>
/// Handler for applying vouchers and calculating totals.
/// </summary>
public class ApplyVoucherHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<ApplyVoucherHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplyVoucherHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public ApplyVoucherHandler(IAppDbContext dbContext, ILogger<ApplyVoucherHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the apply voucher command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The checkout totals with applied vouchers.</returns>
    /// <exception cref="InvalidOperationException">Thrown when voucher is invalid or business rules are violated.</exception>
    public async Task<CheckoutTotalsDto> Handle(ApplyVoucherCommand command, CancellationToken cancellationToken)
    {
        // Get user's cart
        var cart = await this.dbContext.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .ThenInclude(p => p!.Variants)
            .Include(c => c.Items)
            .ThenInclude(i => i.Variant)
            .FirstOrDefaultAsync(c => c.UserId == command.UserId, cancellationToken);

        if (cart == null)
        {
            throw new InvalidOperationException("Cart not found.");
        }

        // Calculate subtotal from selected items
        var selectedItems = cart.Items.Where(i => i.Selected).ToList();
        if (!selectedItems.Any())
        {
            throw new InvalidOperationException("No items selected for checkout.");
        }

        var subtotal = selectedItems.Sum(item =>
        {
            var price = item.Variant?.Price ?? item.Product?.Variants.FirstOrDefault()?.Price ?? 0;
            return price * item.Quantity;
        });

        // For MVP, use a fixed shipping fee (in a real system, this would be calculated based on address/weight)
        decimal shippingFee = 30.0m;

        decimal discountAmount = 0;
        string? discountVoucherCode = null;

        decimal shippingDiscount = 0;
        string? shippingVoucherCode = null;

        // Apply discount voucher if provided
        if (!string.IsNullOrWhiteSpace(command.DiscountCode))
        {
            var discountVoucher = await this.dbContext.Vouchers
                .FirstOrDefaultAsync(
                    v => v.Code == command.DiscountCode && v.IsActive && v.Type == VoucherType.Discount,
                    cancellationToken);

            if (discountVoucher == null)
            {
                throw new InvalidOperationException("Invalid discount voucher code.");
            }

            // Check minimum order amount
            if (discountVoucher.MinimumOrderAmount.HasValue && subtotal < discountVoucher.MinimumOrderAmount.Value)
            {
                throw new InvalidOperationException($"Minimum order amount of ${discountVoucher.MinimumOrderAmount.Value:F2} is required for this discount voucher.");
            }

            discountAmount = discountVoucher.DiscountAmount;
            discountVoucherCode = discountVoucher.Code;

            this.logger.LogInformation(
                "Applied discount voucher {VoucherCode} with amount {DiscountAmount} for user {UserId}",
                discountVoucherCode,
                discountAmount,
                command.UserId);
        }

        // Apply shipping voucher if provided
        if (!string.IsNullOrWhiteSpace(command.ShippingCode))
        {
            var shippingVoucher = await this.dbContext.Vouchers
                .FirstOrDefaultAsync(
                    v => v.Code == command.ShippingCode && v.IsActive && v.Type == VoucherType.Shipping,
                    cancellationToken);

            if (shippingVoucher == null)
            {
                throw new InvalidOperationException("Invalid shipping voucher code.");
            }

            // Check minimum order amount
            if (shippingVoucher.MinimumOrderAmount.HasValue && subtotal < shippingVoucher.MinimumOrderAmount.Value)
            {
                throw new InvalidOperationException($"Minimum order amount of ${shippingVoucher.MinimumOrderAmount.Value:F2} is required for this shipping voucher.");
            }

            // Business rule: Shipping promotion cannot exceed shipping fee
            shippingDiscount = Math.Min(shippingVoucher.DiscountAmount, shippingFee);
            shippingVoucherCode = shippingVoucher.Code;

            this.logger.LogInformation(
                "Applied shipping voucher {VoucherCode} with discount {ShippingDiscount} (max {ShippingFee}) for user {UserId}",
                shippingVoucherCode,
                shippingDiscount,
                shippingFee,
                command.UserId);
        }

        // Calculate total
        var total = subtotal - discountAmount + shippingFee - shippingDiscount;

        // Ensure total is not negative
        if (total < 0)
        {
            total = 0;
        }

        return new CheckoutTotalsDto
        {
            Subtotal = subtotal,
            DiscountAmount = discountAmount,
            ShippingFee = shippingFee,
            ShippingDiscount = shippingDiscount,
            Total = total,
            DiscountVoucherCode = discountVoucherCode,
            ShippingVoucherCode = shippingVoucherCode,
        };
    }
}
