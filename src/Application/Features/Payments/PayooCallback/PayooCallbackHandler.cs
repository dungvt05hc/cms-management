// <copyright file="PayooCallbackHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Payments.PayooCallback;

/// <summary>
/// Handler for Payoo payment callback (idempotent).
/// </summary>
public class PayooCallbackHandler
{
    private readonly IAppDbContext dbContext;
    private readonly IShippingQuoteProvider shippingQuoteProvider;
    private readonly IPaymentGateway paymentGateway;
    private readonly IDateTime dateTime;
    private readonly ILogger<PayooCallbackHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PayooCallbackHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="shippingQuoteProvider">The shipping quote provider.</param>
    /// <param name="paymentGateway">The payment gateway.</param>
    /// <param name="dateTime">The date/time service.</param>
    /// <param name="logger">The logger.</param>
    public PayooCallbackHandler(
        IAppDbContext dbContext,
        IShippingQuoteProvider shippingQuoteProvider,
        IPaymentGateway paymentGateway,
        IDateTime dateTime,
        ILogger<PayooCallbackHandler> logger)
    {
        this.dbContext = dbContext;
        this.shippingQuoteProvider = shippingQuoteProvider;
        this.paymentGateway = paymentGateway;
        this.dateTime = dateTime;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the Payoo callback command (idempotent).
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The callback result.</returns>
    public async Task<PayooCallbackResult> Handle(PayooCallbackCommand command, CancellationToken cancellationToken)
    {
        // Idempotency check: if order already exists for this payment reference, return success
        var existingOrder = await this.dbContext.Orders
            .FirstOrDefaultAsync(o => o.PaymentReference == command.PaymentReference, cancellationToken);

        if (existingOrder != null)
        {
            this.logger.LogInformation(
                "Payment callback for reference {PaymentReference} already processed, order {OrderId} exists",
                command.PaymentReference,
                existingOrder.Id);

            return new PayooCallbackResult
            {
                Success = true,
                OrderId = existingOrder.Id,
            };
        }

        // Verify payment with gateway
        var paymentVerified = await this.paymentGateway.VerifyPaymentAsync(command.PaymentReference, cancellationToken);

        if (!paymentVerified)
        {
            this.logger.LogWarning(
                "Payment verification failed for reference {PaymentReference}",
                command.PaymentReference);

            return new PayooCallbackResult
            {
                Success = false,
                ErrorMessage = "Payment verification failed.",
            };
        }

        // Get address
        var address = await this.dbContext.Addresses
            .FirstOrDefaultAsync(a => a.Id == command.AddressId && a.UserId == command.UserId, cancellationToken);

        if (address == null)
        {
            this.logger.LogError(
                "Address {AddressId} not found for user {UserId} during payment callback",
                command.AddressId,
                command.UserId);

            return new PayooCallbackResult
            {
                Success = false,
                ErrorMessage = "Address not found.",
            };
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
            this.logger.LogError(
                "No items selected in cart for user {UserId} during payment callback",
                command.UserId);

            return new PayooCallbackResult
            {
                Success = false,
                ErrorMessage = "No items in cart.",
            };
        }

        // Calculate totals
        var selectedItems = cart.Items.Where(i => i.Selected).ToList();
        var subtotal = selectedItems.Sum(item =>
        {
            var price = item.Variant?.Price ?? item.Product?.Variants.FirstOrDefault()?.Price ?? 0;
            return price * item.Quantity;
        });

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

        // Apply vouchers
        if (!string.IsNullOrWhiteSpace(command.DiscountCode))
        {
            var discountVoucher = await this.dbContext.Vouchers
                .FirstOrDefaultAsync(
                    v => v.Code == command.DiscountCode && v.IsActive && v.Type == VoucherType.Discount,
                    cancellationToken);

            if (discountVoucher != null &&
                (!discountVoucher.MinimumOrderAmount.HasValue || subtotal >= discountVoucher.MinimumOrderAmount.Value))
            {
                discountAmount = discountVoucher.DiscountAmount;
                discountVoucherCode = discountVoucher.Code;
            }
        }

        if (!string.IsNullOrWhiteSpace(command.ShippingCode))
        {
            var shippingVoucher = await this.dbContext.Vouchers
                .FirstOrDefaultAsync(
                    v => v.Code == command.ShippingCode && v.IsActive && v.Type == VoucherType.Shipping,
                    cancellationToken);

            if (shippingVoucher != null &&
                (!shippingVoucher.MinimumOrderAmount.HasValue || subtotal >= shippingVoucher.MinimumOrderAmount.Value))
            {
                shippingDiscount = Math.Min(shippingVoucher.DiscountAmount, shippingFee);
                shippingVoucherCode = shippingVoucher.Code;
            }
        }

        var total = subtotal - discountAmount + shippingFee - shippingDiscount;
        if (total < 0)
        {
            total = 0;
        }

        // Create order
        var now = this.dateTime.UtcNow;
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            Subtotal = subtotal,
            DiscountAmount = discountAmount,
            ShippingFee = shippingFee,
            ShippingDiscount = shippingDiscount,
            Total = total,
            PaymentMethod = PaymentMethod.Payoo,
            Status = OrderStatus.Processing,
            ShippingFullName = address.FullName,
            ShippingPhone = address.Phone,
            ShippingAddressLine = address.AddressLine,
            ShippingWard = address.Ward,
            ShippingDistrict = address.District,
            ShippingCity = address.City,
            ShippingMethodCode = command.ShippingMethodCode,
            ShippingCarrierCode = command.ShippingCarrierCode,
            DiscountVoucherCode = discountVoucherCode,
            ShippingVoucherCode = shippingVoucherCode,
            Notes = command.Notes,
            PaymentReference = command.PaymentReference,
            CreatedAt = now,
            UpdatedAt = now,
        };

        foreach (var cartItem in selectedItems)
        {
            var price = cartItem.Variant?.Price ?? cartItem.Product?.Variants.FirstOrDefault()?.Price ?? 0;
            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = cartItem.ProductId,
                VariantId = cartItem.VariantId,
                ProductName = cartItem.Product?.Name ?? string.Empty,
                VariantName = cartItem.Variant?.VariantName,
                Sku = cartItem.Variant?.Sku ?? cartItem.Product?.Variants.FirstOrDefault()?.Sku ?? string.Empty,
                UnitPrice = price,
                Quantity = cartItem.Quantity,
                TotalPrice = price * cartItem.Quantity,
                CreatedAt = now,
                UpdatedAt = now,
            };

            order.Items.Add(orderItem);
        }

        this.dbContext.Orders.Add(order);

        // Clear cart selected items
        foreach (var item in selectedItems)
        {
            this.dbContext.CartItems.Remove(item);
        }

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation(
            "Order {OrderId} created for user {UserId} after successful Payoo payment verification",
            order.Id,
            command.UserId);

        return new PayooCallbackResult
        {
            Success = true,
            OrderId = order.Id,
        };
    }
}
