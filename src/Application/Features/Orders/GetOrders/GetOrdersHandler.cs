// <copyright file="GetOrdersHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.GetOrders;

/// <summary>
/// Handler for getting user's orders.
/// </summary>
public class GetOrdersHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<GetOrdersHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetOrdersHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public GetOrdersHandler(IAppDbContext dbContext, ILogger<GetOrdersHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the get orders query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of orders.</returns>
    public async Task<List<OrderDto>> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
    {
        var ordersQuery = this.dbContext.Orders
            .Include(o => o.Items)
            .Where(o => o.UserId == query.UserId);

        if (query.Status.HasValue)
        {
            ordersQuery = ordersQuery.Where(o => o.Status == query.Status.Value);
        }

        var orders = await ordersQuery
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

        return orders.Select(o => new OrderDto
        {
            Id = o.Id,
            Status = o.Status,
            PaymentMethod = o.PaymentMethod,
            Subtotal = o.Subtotal,
            DiscountAmount = o.DiscountAmount,
            ShippingFee = o.ShippingFee,
            ShippingDiscount = o.ShippingDiscount,
            Total = o.Total,
            ShippingFullName = o.ShippingFullName,
            ShippingPhone = o.ShippingPhone,
            ShippingAddressLine = o.ShippingAddressLine,
            ShippingWard = o.ShippingWard,
            ShippingDistrict = o.ShippingDistrict,
            ShippingCity = o.ShippingCity,
            ShippingMethodCode = o.ShippingMethodCode,
            ShippingCarrierCode = o.ShippingCarrierCode,
            DiscountVoucherCode = o.DiscountVoucherCode,
            ShippingVoucherCode = o.ShippingVoucherCode,
            Notes = o.Notes,
            Items = o.Items.Select(i => new OrderItemDto
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
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt,
        }).ToList();
    }
}
