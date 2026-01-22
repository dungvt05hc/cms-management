// <copyright file="CheckoutSubmitHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Checkout.Submit;

/// <summary>
/// Handler for checkout submit.
/// </summary>
public class CheckoutSubmitHandler
{
    private readonly IAppDbContext dbContext;
    private readonly IShippingQuoteProvider shippingQuoteProvider;
    private readonly IPaymentGateway paymentGateway;
    private readonly IDateTime dateTime;
    private readonly ILogger<CheckoutSubmitHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckoutSubmitHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="shippingQuoteProvider">The shipping quote provider.</param>
    /// <param name="paymentGateway">The payment gateway.</param>
    /// <param name="dateTime">The date/time service.</param>
    /// <param name="logger">The logger.</param>
    public CheckoutSubmitHandler(
        IAppDbContext dbContext,
        IShippingQuoteProvider shippingQuoteProvider,
        IPaymentGateway paymentGateway,
        IDateTime dateTime,
        ILogger<CheckoutSubmitHandler> logger)
    {
        this.dbContext = dbContext;
        this.shippingQuoteProvider = shippingQuoteProvider;
        this.paymentGateway = paymentGateway;
        this.dateTime = dateTime;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the checkout submit command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The checkout submit result.</returns>
    /// <exception cref="InvalidOperationException">Thrown when cart is empty or address not found.</exception>
    public async Task<CheckoutSubmitResult> Handle(CheckoutSubmitCommand command, CancellationToken cancellationToken)
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

        // Apply discount voucher
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

        // Apply shipping voucher
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

        // Handle based on payment method
        if (command.PaymentMethod == PaymentMethod.COD)
        {
            // COD: Create order immediately
            var order = await this.CreateOrderAsync(
                command,
                address,
                selectedItems,
                subtotal,
                discountAmount,
                shippingFee,
                shippingDiscount,
                total,
                discountVoucherCode,
                shippingVoucherCode,
                null,
                cancellationToken);

            // Clear cart selected items
            foreach (var item in selectedItems)
            {
                this.dbContext.CartItems.Remove(item);
            }

            await this.dbContext.SaveChangesAsync(cancellationToken);

            this.logger.LogInformation(
                "Order {OrderId} created for user {UserId} with COD payment",
                order.Id,
                command.UserId);

            return new CheckoutSubmitResult
            {
                OrderId = order.Id,
            };
        }
        else
        {
            // Payoo: Initiate payment, DO NOT create order yet
            var orderReference = $"ORD_{Guid.NewGuid():N}";
            var paymentResult = await this.paymentGateway.InitiatePaymentAsync(
                total,
                orderReference,
                cancellationToken);

            this.logger.LogInformation(
                "Payment initiated for user {UserId} with amount {Total}",
                command.UserId,
                total);

            return new CheckoutSubmitResult
            {
                PaymentUrl = paymentResult.PaymentUrl,
                PaymentReference = paymentResult.PaymentReference,
            };
        }
    }

    private async Task<Order> CreateOrderAsync(
        CheckoutSubmitCommand command,
        Address address,
        List<CartItem> selectedItems,
        decimal subtotal,
        decimal discountAmount,
        decimal shippingFee,
        decimal shippingDiscount,
        decimal total,
        string? discountVoucherCode,
        string? shippingVoucherCode,
        string? paymentReference,
        CancellationToken cancellationToken)
    {
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
            PaymentMethod = command.PaymentMethod,
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
            PaymentReference = paymentReference,
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
        return order;
    }
}
