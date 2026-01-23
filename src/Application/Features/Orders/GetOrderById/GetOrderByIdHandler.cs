// <copyright file="GetOrderByIdHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.GetOrderById;

/// <summary>
/// Handler for getting order by ID.
/// </summary>
public class GetOrderByIdHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<GetOrderByIdHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetOrderByIdHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public GetOrderByIdHandler(IAppDbContext dbContext, ILogger<GetOrderByIdHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the get order by ID query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The order details or null if not found.</returns>
    public async Task<OrderDto?> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
    {
        var order = await this.dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == query.OrderId && o.UserId == query.UserId, cancellationToken);

        if (order == null)
        {
            return null;
        }

        return new OrderDto
        {
            Id = order.Id,
            Status = order.Status,
            PaymentMethod = order.PaymentMethod,
            Subtotal = order.Subtotal,
            DiscountAmount = order.DiscountAmount,
            ShippingFee = order.ShippingFee,
            ShippingDiscount = order.ShippingDiscount,
            Total = order.Total,
            ShippingFullName = order.ShippingFullName,
            ShippingPhone = order.ShippingPhone,
            ShippingAddressLine = order.ShippingAddressLine,
            ShippingWard = order.ShippingWard,
            ShippingDistrict = order.ShippingDistrict,
            ShippingCity = order.ShippingCity,
            ShippingMethodCode = order.ShippingMethodCode,
            ShippingCarrierCode = order.ShippingCarrierCode,
            DiscountVoucherCode = order.DiscountVoucherCode,
            ShippingVoucherCode = order.ShippingVoucherCode,
            Notes = order.Notes,
            Items = order.Items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                VariantId = i.VariantId,
                ProductName = i.ProductName,
                VariantName = i.VariantName,
                Sku = i.Sku,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                TotalPrice = i.TotalPrice,
            }).ToList(),
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
        };
    }
}
