// <copyright file="CheckoutVoucherEndpointTests.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Application.Abstractions;
using Application.Features.Auth.Login;
using Application.Features.Checkout;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.IntegrationTests;

/// <summary>
/// Integration tests for checkout voucher endpoints.
/// </summary>
public sealed class CheckoutVoucherEndpointTests : IDisposable
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckoutVoucherEndpointTests"/> class.
    /// </summary>
    public CheckoutVoucherEndpointTests()
    {
        this.factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptorsToRemove = services
                        .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                                    d.ServiceType == typeof(DbContextOptions) ||
                                    d.ServiceType == typeof(AppDbContext) ||
                                    d.ServiceType == typeof(IAppDbContext) ||
                                    (d.ServiceType.IsGenericType &&
                                     d.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>)))
                        .ToList();

                    foreach (var descriptor in descriptorsToRemove)
                    {
                        services.Remove(descriptor);
                    }

                    var efCoreServices = services
                        .Where(d => d.ServiceType.Namespace != null &&
                                    d.ServiceType.Namespace.StartsWith("Microsoft.EntityFrameworkCore"))
                        .ToList();

                    foreach (var descriptor in efCoreServices)
                    {
                        services.Remove(descriptor);
                    }

                    var dbName = $"VoucherTestDb_{Guid.NewGuid()}";
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(dbName);
                    });

                    services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
                });
            });

        this.client = this.factory.CreateClient();
    }

    /// <summary>
    /// Test that applying a discount voucher reduces the total correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task ApplyVoucher_DiscountVoucher_ReducesTotalCorrectly()
    {
        var token = await this.GetAuthTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Add item to cart
        var product = await this.CreateTestProductAsync(100m);
        await this.client.PostAsJsonAsync("/cart/items", new { productId = product.Id, variantId = (Guid?)null, quantity = 2 });

        // Create discount voucher
        var discountVoucher = await this.CreateVoucherAsync("DISCOUNT10", VoucherType.Discount, 10m);

        // Apply voucher
        var request = new { discountCode = "DISCOUNT10", shippingCode = (string?)null };
        var response = await this.client.PostAsJsonAsync("/checkout/apply-voucher", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var totals = await response.Content.ReadFromJsonAsync<CheckoutTotalsDto>();
        Assert.NotNull(totals);
        Assert.Equal(200m, totals.Subtotal); // 2 items x 100
        Assert.Equal(10m, totals.DiscountAmount);
        Assert.Equal(30m, totals.ShippingFee);
        Assert.Equal(0m, totals.ShippingDiscount);
        Assert.Equal(220m, totals.Total); // 200 - 10 + 30 - 0
        Assert.Equal("DISCOUNT10", totals.DiscountVoucherCode);
        Assert.Null(totals.ShippingVoucherCode);
    }

    /// <summary>
    /// Test that applying a shipping voucher reduces shipping fee correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task ApplyVoucher_ShippingVoucher_ReducesShippingFeeCorrectly()
    {
        var token = await this.GetAuthTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Add item to cart
        var product = await this.CreateTestProductAsync(100m);
        await this.client.PostAsJsonAsync("/cart/items", new { productId = product.Id, variantId = (Guid?)null, quantity = 1 });

        // Create shipping voucher
        await this.CreateVoucherAsync("FREESHIP", VoucherType.Shipping, 30m);

        // Apply voucher
        var request = new { discountCode = (string?)null, shippingCode = "FREESHIP" };
        var response = await this.client.PostAsJsonAsync("/checkout/apply-voucher", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var totals = await response.Content.ReadFromJsonAsync<CheckoutTotalsDto>();
        Assert.NotNull(totals);
        Assert.Equal(100m, totals.Subtotal);
        Assert.Equal(0m, totals.DiscountAmount);
        Assert.Equal(30m, totals.ShippingFee);
        Assert.Equal(30m, totals.ShippingDiscount);
        Assert.Equal(100m, totals.Total); // 100 - 0 + 30 - 30
        Assert.Null(totals.DiscountVoucherCode);
        Assert.Equal("FREESHIP", totals.ShippingVoucherCode);
    }

    /// <summary>
    /// Test that shipping discount cannot exceed shipping fee (business rule).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task ApplyVoucher_ShippingVoucherExceedsShippingFee_CapsAtShippingFee()
    {
        var token = await this.GetAuthTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Add item to cart
        var product = await this.CreateTestProductAsync(100m);
        await this.client.PostAsJsonAsync("/cart/items", new { productId = product.Id, variantId = (Guid?)null, quantity = 1 });

        // Create shipping voucher with discount > shipping fee
        await this.CreateVoucherAsync("BIGSHIP", VoucherType.Shipping, 50m);

        // Apply voucher
        var request = new { discountCode = (string?)null, shippingCode = "BIGSHIP" };
        var response = await this.client.PostAsJsonAsync("/checkout/apply-voucher", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var totals = await response.Content.ReadFromJsonAsync<CheckoutTotalsDto>();
        Assert.NotNull(totals);
        Assert.Equal(30m, totals.ShippingFee);
        Assert.Equal(30m, totals.ShippingDiscount); // Capped at shipping fee
        Assert.Equal(100m, totals.Total); // Shipping fee fully discounted
    }

    /// <summary>
    /// Test that both discount and shipping vouchers can be applied together.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task ApplyVoucher_BothVouchers_AppliesBothCorrectly()
    {
        var token = await this.GetAuthTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Add item to cart
        var product = await this.CreateTestProductAsync(100m);
        await this.client.PostAsJsonAsync("/cart/items", new { productId = product.Id, variantId = (Guid?)null, quantity = 2 });

        // Create vouchers
        await this.CreateVoucherAsync("DISCOUNT20", VoucherType.Discount, 20m);
        await this.CreateVoucherAsync("SHIP10", VoucherType.Shipping, 10m);

        // Apply both vouchers
        var request = new { discountCode = "DISCOUNT20", shippingCode = "SHIP10" };
        var response = await this.client.PostAsJsonAsync("/checkout/apply-voucher", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var totals = await response.Content.ReadFromJsonAsync<CheckoutTotalsDto>();
        Assert.NotNull(totals);
        Assert.Equal(200m, totals.Subtotal);
        Assert.Equal(20m, totals.DiscountAmount);
        Assert.Equal(30m, totals.ShippingFee);
        Assert.Equal(10m, totals.ShippingDiscount);
        Assert.Equal(200m, totals.Total); // 200 - 20 + 30 - 10
        Assert.Equal("DISCOUNT20", totals.DiscountVoucherCode);
        Assert.Equal("SHIP10", totals.ShippingVoucherCode);
    }

    /// <summary>
    /// Test that invalid voucher code returns 400.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task ApplyVoucher_InvalidVoucherCode_Returns400()
    {
        var token = await this.GetAuthTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Add item to cart
        var product = await this.CreateTestProductAsync(100m);
        await this.client.PostAsJsonAsync("/cart/items", new { productId = product.Id, variantId = (Guid?)null, quantity = 1 });

        // Apply non-existent voucher
        var request = new { discountCode = "INVALID", shippingCode = (string?)null };
        var response = await this.client.PostAsJsonAsync("/checkout/apply-voucher", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// Test that applying voucher without auth returns 401.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task ApplyVoucher_WithoutAuth_Returns401()
    {
        var request = new { discountCode = "TEST", shippingCode = (string?)null };
        var response = await this.client.PostAsJsonAsync("/checkout/apply-voucher", request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Test that applying voucher with no selected items returns 400.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task ApplyVoucher_NoSelectedItems_Returns400()
    {
        var token = await this.GetAuthTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Add item to cart but don't select it
        var product = await this.CreateTestProductAsync(100m);
        var addResponse = await this.client.PostAsJsonAsync("/cart/items", new { productId = product.Id, variantId = (Guid?)null, quantity = 1 });
        var cartItem = await addResponse.Content.ReadFromJsonAsync<Application.Features.Cart.CartItemDto>();

        // Deselect item
        await this.client.PatchAsJsonAsync($"/cart/items/{cartItem!.Id}", new { selected = false });

        // Try to apply voucher
        await this.CreateVoucherAsync("TEST", VoucherType.Discount, 10m);
        var request = new { discountCode = "TEST", shippingCode = (string?)null };
        var response = await this.client.PostAsJsonAsync("/checkout/apply-voucher", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// Disposes the test client and factory.
    /// </summary>
    public void Dispose()
    {
        this.client.Dispose();
        this.factory.Dispose();
    }

    private async Task<string> GetAuthTokenAsync()
    {
        var uniquePhone = $"0912{Random.Shared.Next(1000000, 9999999)}";
        var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var dateTime = scope.ServiceProvider.GetRequiredService<IDateTime>();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Phone = uniquePhone,
            Email = $"test-{Guid.NewGuid()}@example.com",
            FullName = "Test User",
            PasswordHash = passwordHasher.HashPassword("password123"),
            IsVerified = true,
            CreatedAt = dateTime.UtcNow,
            UpdatedAt = dateTime.UtcNow,
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var loginQuery = new LoginQuery(uniquePhone, "password123");
        var loginResponse = await this.client.PostAsJsonAsync("/auth/login", loginQuery);
        loginResponse.EnsureSuccessStatusCode();
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResult>();

        return loginResult!.AccessToken;
    }

    private async Task<TestProduct> CreateTestProductAsync(decimal price = 100m)
    {
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = $"Test Product {Guid.NewGuid()}",
            Slug = $"test-product-{Guid.NewGuid()}",
            Description = "Test description",
            IsActive = true,
            IsFeatured = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var variant = new ProductVariant
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Sku = $"SKU-{Guid.NewGuid()}",
            VariantName = "Default",
            Price = price,
            StockQuantity = 100,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        product.Variants.Add(variant);
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        return new TestProduct(product.Id, product.Name, product.Slug, variant.Id, price);
    }

    private async Task<TestVoucher> CreateVoucherAsync(string code, VoucherType type, decimal discountAmount, decimal? minimumOrderAmount = null)
    {
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var voucher = new Voucher
        {
            Id = Guid.NewGuid(),
            Code = code,
            Type = type,
            DiscountAmount = discountAmount,
            IsActive = true,
            MinimumOrderAmount = minimumOrderAmount,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        dbContext.Vouchers.Add(voucher);
        await dbContext.SaveChangesAsync();

        return new TestVoucher(voucher.Id, code, type, discountAmount);
    }

    private record TestProduct(Guid Id, string Name, string Slug, Guid VariantId, decimal Price);

    private record TestVoucher(Guid Id, string Code, VoucherType Type, decimal DiscountAmount);
}
