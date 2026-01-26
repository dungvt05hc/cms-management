// <copyright file="AppDbContext.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

/// <summary>
/// Application database context.
/// </summary>
public class AppDbContext : DbContext, IAppDbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the Users DbSet.
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// Gets or sets the PasswordResetTokens DbSet.
    /// </summary>
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; } = null!;

    /// <summary>
    /// Gets or sets the StaffUsers DbSet.
    /// </summary>
    public DbSet<StaffUser> StaffUsers { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Categories DbSet.
    /// </summary>
    public DbSet<Category> Categories { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Products DbSet.
    /// </summary>
    public DbSet<Product> Products { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ProductVariants DbSet.
    /// </summary>
    public DbSet<ProductVariant> ProductVariants { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Carts DbSet.
    /// </summary>
    public DbSet<Cart> Carts { get; set; } = null!;

    /// <summary>
    /// Gets or sets the CartItems DbSet.
    /// </summary>
    public DbSet<CartItem> CartItems { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Addresses DbSet.
    /// </summary>
    public DbSet<Address> Addresses { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ShippingMethods DbSet.
    /// </summary>
    public DbSet<ShippingMethod> ShippingMethods { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ShippingCarriers DbSet.
    /// </summary>
    public DbSet<ShippingCarrier> ShippingCarriers { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ShippingConfigs DbSet.
    /// </summary>
    public DbSet<ShippingConfig> ShippingConfigs { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Vouchers DbSet.
    /// </summary>
    public DbSet<Voucher> Vouchers { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Orders DbSet.
    /// </summary>
    public DbSet<Order> Orders { get; set; } = null!;

    /// <summary>
    /// Gets or sets the OrderItems DbSet.
    /// </summary>
    public DbSet<OrderItem> OrderItems { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ProductGroups DbSet.
    /// </summary>
    public DbSet<ProductGroup> ProductGroups { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ProductGroupAttributes DbSet.
    /// </summary>
    public DbSet<ProductGroupAttribute> ProductGroupAttributes { get; set; } = null!;

    /// <summary>
    /// Gets or sets the InvoiceProfiles DbSet.
    /// </summary>
    public DbSet<InvoiceProfile> InvoiceProfiles { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Notifications DbSet.
    /// </summary>
    public DbSet<Notification> Notifications { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DeviceTokens DbSet.
    /// </summary>
    public DbSet<DeviceToken> DeviceTokens { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Promotions DbSet.
    /// </summary>
    public DbSet<Promotion> Promotions { get; set; } = null!;

    /// <summary>
    /// Configures the database schema.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Phone).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();

            entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(256);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(256);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasIndex(e => e.UserId);

            entity.Property(e => e.Token).IsRequired().HasMaxLength(256);
            entity.Property(e => e.ExpiresAt).IsRequired();
            entity.Property(e => e.IsUsed).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StaffUser>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Phone).IsUnique();

            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(256);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Role).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ParentId);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(256);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasOne(e => e.Parent)
                .WithMany(e => e.Children)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.CategoryId);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(512);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(512);
            entity.Property(e => e.Description).HasMaxLength(4000);
            entity.Property(e => e.Images).HasMaxLength(2000);
            entity.Property(e => e.Videos).HasMaxLength(2000);
            entity.Property(e => e.Specifications).HasMaxLength(4000);
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.IsFeatured).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Sku).IsUnique();
            entity.HasIndex(e => e.ProductId);

            entity.Property(e => e.Sku).IsRequired().HasMaxLength(100);
            entity.Property(e => e.VariantName).HasMaxLength(256);
            entity.Property(e => e.Price).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.StockQuantity).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasOne(e => e.Product)
                .WithMany(e => e.Variants)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);

            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CartId);
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => e.VariantId);

            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.Selected).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasOne(e => e.Cart)
                .WithMany(e => e.Items)
                .HasForeignKey(e => e.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Variant)
                .WithMany()
                .HasForeignKey(e => e.VariantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);

            entity.Property(e => e.FullName).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.AddressLine).IsRequired().HasMaxLength(512);
            entity.Property(e => e.Ward).IsRequired().HasMaxLength(256);
            entity.Property(e => e.District).IsRequired().HasMaxLength(256);
            entity.Property(e => e.City).IsRequired().HasMaxLength(256);
            entity.Property(e => e.IsDefault).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ShippingMethod>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();

            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });

        modelBuilder.Entity<ShippingCarrier>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ShippingMethodId, e.Code }).IsUnique();
            entity.HasIndex(e => e.ShippingMethodId);

            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.SupportsCOD).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasOne(e => e.ShippingMethod)
                .WithMany(e => e.Carriers)
                .HasForeignKey(e => e.ShippingMethodId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ShippingConfig>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Key).IsUnique();

            entity.Property(e => e.Key).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Value).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();

            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.DiscountAmount).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.MinimumOrderAmount).HasPrecision(18, 2);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.PaymentReference);

            entity.Property(e => e.Subtotal).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.DiscountAmount).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.ShippingFee).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.ShippingDiscount).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.Total).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.PaymentMethod).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.ShippingFullName).IsRequired().HasMaxLength(256);
            entity.Property(e => e.ShippingPhone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.ShippingAddressLine).IsRequired().HasMaxLength(512);
            entity.Property(e => e.ShippingWard).IsRequired().HasMaxLength(256);
            entity.Property(e => e.ShippingDistrict).IsRequired().HasMaxLength(256);
            entity.Property(e => e.ShippingCity).IsRequired().HasMaxLength(256);
            entity.Property(e => e.ShippingMethodCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ShippingCarrierCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.DiscountVoucherCode).HasMaxLength(50);
            entity.Property(e => e.ShippingVoucherCode).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.PaymentReference).HasMaxLength(256);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.InvoiceRequested).IsRequired();
            entity.Property(e => e.InvoiceTaxCode).HasMaxLength(50);
            entity.Property(e => e.InvoiceCompanyName).HasMaxLength(512);
            entity.Property(e => e.InvoiceCompanyAddress).HasMaxLength(512);
            entity.Property(e => e.InvoiceEmail).HasMaxLength(256);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => e.VariantId);

            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(512);
            entity.Property(e => e.VariantName).HasMaxLength(256);
            entity.Property(e => e.Sku).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UnitPrice).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.TotalPrice).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasOne(e => e.Order)
                .WithMany(e => e.Items)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Variant)
                .WithMany()
                .HasForeignKey(e => e.VariantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProductGroup>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CategoryId);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(256);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProductGroupAttribute>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ProductGroupId, e.Key }).IsUnique();
            entity.HasIndex(e => e.ProductGroupId);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasOne(e => e.ProductGroup)
                .WithMany(e => e.Attributes)
                .HasForeignKey(e => e.ProductGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InvoiceProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);

            entity.Property(e => e.TaxCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(512);
            entity.Property(e => e.CompanyAddress).IsRequired().HasMaxLength(512);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.UserId, e.Type });

            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Title).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Data).HasMaxLength(4000);
            entity.Property(e => e.IsRead).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DeviceToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.UserId, e.Token }).IsUnique();

            entity.Property(e => e.Token).IsRequired().HasMaxLength(512);
            entity.Property(e => e.DeviceType).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();

            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.DiscountType).IsRequired().HasMaxLength(20);
            entity.Property(e => e.DiscountValue).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.MinOrderAmount).HasPrecision(18, 2);
            entity.Property(e => e.MaxDiscountAmount).HasPrecision(18, 2);
            entity.Property(e => e.StartDate).IsRequired();
            entity.Property(e => e.EndDate).IsRequired();
            entity.Property(e => e.UsedCount).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });
    }
}
