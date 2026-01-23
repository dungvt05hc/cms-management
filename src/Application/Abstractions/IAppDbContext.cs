// <copyright file="IAppDbContext.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions;

/// <summary>
/// Interface for application database context.
/// </summary>
public interface IAppDbContext
{
    /// <summary>
    /// Gets the Users DbSet.
    /// </summary>
    DbSet<User> Users { get; }

    /// <summary>
    /// Gets the PasswordResetTokens DbSet.
    /// </summary>
    DbSet<PasswordResetToken> PasswordResetTokens { get; }

    /// <summary>
    /// Gets the StaffUsers DbSet.
    /// </summary>
    DbSet<StaffUser> StaffUsers { get; }

    /// <summary>
    /// Gets the Categories DbSet.
    /// </summary>
    DbSet<Category> Categories { get; }

    /// <summary>
    /// Gets the Products DbSet.
    /// </summary>
    DbSet<Product> Products { get; }

    /// <summary>
    /// Gets the ProductVariants DbSet.
    /// </summary>
    DbSet<ProductVariant> ProductVariants { get; }

    /// <summary>
    /// Gets the Carts DbSet.
    /// </summary>
    DbSet<Cart> Carts { get; }

    /// <summary>
    /// Gets the CartItems DbSet.
    /// </summary>
    DbSet<CartItem> CartItems { get; }

    /// <summary>
    /// Gets the Addresses DbSet.
    /// </summary>
    DbSet<Address> Addresses { get; }

    /// <summary>
    /// Gets the ShippingMethods DbSet.
    /// </summary>
    DbSet<ShippingMethod> ShippingMethods { get; }

    /// <summary>
    /// Gets the ShippingCarriers DbSet.
    /// </summary>
    DbSet<ShippingCarrier> ShippingCarriers { get; }

    /// <summary>
    /// Gets the ShippingConfigs DbSet.
    /// </summary>
    DbSet<ShippingConfig> ShippingConfigs { get; }

    /// <summary>
    /// Gets the Vouchers DbSet.
    /// </summary>
    DbSet<Voucher> Vouchers { get; }

    /// <summary>
    /// Gets the Orders DbSet.
    /// </summary>
    DbSet<Order> Orders { get; }

    /// <summary>
    /// Gets the OrderItems DbSet.
    /// </summary>
    DbSet<OrderItem> OrderItems { get; }

    /// <summary>
    /// Gets the ProductGroups DbSet.
    /// </summary>
    DbSet<ProductGroup> ProductGroups { get; }

    /// <summary>
    /// Gets the ProductGroupAttributes DbSet.
    /// </summary>
    DbSet<ProductGroupAttribute> ProductGroupAttributes { get; }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
