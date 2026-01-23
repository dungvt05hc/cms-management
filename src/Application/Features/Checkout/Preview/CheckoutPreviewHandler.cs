// <copyright file="CheckoutPreviewHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Checkout.Preview;

/// <summary>
/// Handler for checkout preview.
/// </summary>
public class CheckoutPreviewHandler
{
    private readonly IAppDbContext dbContext;
    private readonly IShippingQuoteProvider shippingQuoteProvider;
    private readonly ILogger<CheckoutPreviewHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckoutPreviewHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="shippingQuoteProvider">The shipping quote provider.</param>
    /// <param name="logger">The logger.</param>
    public CheckoutPreviewHandler(
        IAppDbContext dbContext,
        IShippingQuoteProvider shippingQuoteProvider,
        ILogger<CheckoutPreviewHandler> logger)
    {
        this.dbContext = dbContext;
        this.shippingQuoteProvider = shippingQuoteProvider;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the checkout preview command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The checkout totals.</returns>
    /// <exception cref="InvalidOperationException">Thrown when cart is empty or address not found.</exception>
    public async Task<CheckoutTotalsDto> Handle(CheckoutPreviewCommand command, CancellationToken cancellationToken)
    {
        // Verify address belongs to user
        var address = await this.dbContext.Addresses
            .FirstOrDefaultAsync(a => a.Id == command.AddressId && a.UserId == command.UserId, cancellationToken);

        if (address == null)
        {
            throw new InvalidOperationException("Address not found or does not belong to user.");
        }

        // Get user's cart
        var cart = await this.dbContext.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .ThenInclude(p => p!.Variants)
            .Include(c => c.Items)
            .ThenInclude(i => i.Variant)
            .FirstOrDefaultAsync(c => c.UserId == command.UserId, cancellationToken);

        if (cart == null || !cart.Items.Any(i => i.Selected))
        {
            throw new InvalidOperationException("No items selected for checkout.");
        }

        // Calculate subtotal from selected items
        var selectedItems = cart.Items.Where(i => i.Selected).ToList();
        var subtotal = selectedItems.Sum(item =>
        {
            var price = item.Variant?.Price ?? item.Product?.Variants.FirstOrDefault()?.Price ?? 0;
            return price * item.Quantity;
        });

        // Get shipping quote
        var shippingQuote = await this.shippingQuoteProvider.GetQuoteAsync(
            command.ShippingMethodCode,
            command.ShippingCarrierCode,
            "Hanoi",
            address.City,
            address.District,
            address.Ward,
            1000,
            cancellationToken);

        var shippingFee = shippingQuote.Fee;

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

            if (discountVoucher.MinimumOrderAmount.HasValue && subtotal < discountVoucher.MinimumOrderAmount.Value)
            {
                throw new InvalidOperationException($"Minimum order amount of ${discountVoucher.MinimumOrderAmount.Value:F2} is required for this discount voucher.");
            }

            discountAmount = discountVoucher.DiscountAmount;
            discountVoucherCode = discountVoucher.Code;
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

            if (shippingVoucher.MinimumOrderAmount.HasValue && subtotal < shippingVoucher.MinimumOrderAmount.Value)
            {
                throw new InvalidOperationException($"Minimum order amount of ${shippingVoucher.MinimumOrderAmount.Value:F2} is required for this shipping voucher.");
            }

            // Business rule: Shipping promotion cannot exceed shipping fee
            shippingDiscount = Math.Min(shippingVoucher.DiscountAmount, shippingFee);
            shippingVoucherCode = shippingVoucher.Code;
        }

        // Calculate total
        var total = subtotal - discountAmount + shippingFee - shippingDiscount;
        if (total < 0)
        {
            total = 0;
        }

        this.logger.LogInformation(
            "Checkout preview for user {UserId}: Subtotal={Subtotal}, Discount={DiscountAmount}, Shipping={ShippingFee}, ShippingDiscount={ShippingDiscount}, Total={Total}",
            command.UserId,
            subtotal,
            discountAmount,
            shippingFee,
            shippingDiscount,
            total);

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
