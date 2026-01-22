// <copyright file="CheckoutSubmitEndpointTests.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Application.Abstractions;
using Application.Features.Auth.Login;
using Application.Features.Checkout;
using Application.Features.Checkout.Submit;
using Application.Features.Payments.PayooCallback;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.IntegrationTests;

/// <summary>
/// Integration tests for checkout submit endpoints.
/// </summary>
public sealed class CheckoutSubmitEndpointTests : IDisposable
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckoutSubmitEndpointTests"/> class.
    /// </summary>
    public CheckoutSubmitEndpointTests()
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

                    var dbName = $"CheckoutTestDb_{Guid.NewGuid()}";
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
    /// Test checkout preview returns correct totals.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task CheckoutPreview_WithValidData_ReturnsCorrectTotals()
    {
        var token = await this.GetAuthTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var product = await this.CreateTestProductAsync(100m);
        await this.client.PostAsJsonAsync("/cart/items", new { productId = product.ProductId, variantId = (Guid?)null, quantity = 2 });

        var address = await this.CreateTestAddressAsync(token);
        var shippingMethod = await this.CreateTestShippingMethodAsync();

        var request = new
        {
            addressId = address.Id,
            shippingMethodCode = shippingMethod.MethodCode,
            shippingCarrierCode = shippingMethod.CarrierCode,
            discountCode = (string?)null,
            shippingCode = (string?)null,
        };

        var response = await this.client.PostAsJsonAsync("/checkout/preview", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var totals = await response.Content.ReadFromJsonAsync<CheckoutTotalsDto>();
        Assert.NotNull(totals);
        Assert.Equal(200m, totals.Subtotal);
        Assert.True(totals.ShippingFee > 0);
        Assert.Equal(0m, totals.DiscountAmount);
    }

    /// <summary>
    /// Test checkout submit with COD creates order immediately.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task CheckoutSubmit_COD_CreatesOrderImmediately()
    {
        var token = await this.GetAuthTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var product = await this.CreateTestProductAsync(150m);
        await this.client.PostAsJsonAsync("/cart/items", new { productId = product.ProductId, variantId = (Guid?)null, quantity = 1 });

        var address = await this.CreateTestAddressAsync(token);
        var shippingMethod = await this.CreateTestShippingMethodAsync();

        var request = new
        {
            addressId = address.Id,
            shippingMethodCode = shippingMethod.MethodCode,
            shippingCarrierCode = shippingMethod.CarrierCode,
            discountCode = (string?)null,
            shippingCode = (string?)null,
            paymentMethod = PaymentMethod.COD,
            notes = "Test order",
        };

        var response = await this.client.PostAsJsonAsync("/checkout/submit", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CheckoutSubmitResult>();
        Assert.NotNull(result);
        Assert.NotNull(result.OrderId);
        Assert.Null(result.PaymentUrl);
        Assert.Null(result.PaymentReference);

        // Verify order exists in database
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var order = await dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == result.OrderId);

        Assert.NotNull(order);
        Assert.Equal(PaymentMethod.COD, order.PaymentMethod);
        Assert.Equal(OrderStatus.Processing, order.Status);
        Assert.Single(order.Items);
        Assert.Equal("Test order", order.Notes);
    }

    /// <summary>
    /// Test checkout submit with Payoo returns payment URL and does NOT create order.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task CheckoutSubmit_Payoo_ReturnsPaymentUrlWithoutCreatingOrder()
    {
        var token = await this.GetAuthTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var product = await this.CreateTestProductAsync(200m);
        await this.client.PostAsJsonAsync("/cart/items", new { productId = product.ProductId, variantId = (Guid?)null, quantity = 1 });

        var address = await this.CreateTestAddressAsync(token);
        var shippingMethod = await this.CreateTestShippingMethodAsync();

        var request = new
        {
            addressId = address.Id,
            shippingMethodCode = shippingMethod.MethodCode,
            shippingCarrierCode = shippingMethod.CarrierCode,
            discountCode = (string?)null,
            shippingCode = (string?)null,
            paymentMethod = PaymentMethod.Payoo,
            notes = "Payoo test",
        };

        var response = await this.client.PostAsJsonAsync("/checkout/submit", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CheckoutSubmitResult>();
        Assert.NotNull(result);
        Assert.Null(result.OrderId);
        Assert.NotNull(result.PaymentUrl);
        Assert.NotNull(result.PaymentReference);

        // Verify NO order exists in database yet
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var orders = await dbContext.Orders.ToListAsync();
        Assert.Empty(orders);
    }

    /// <summary>
    /// Test Payoo callback creates order after successful payment verification.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task PayooCallback_SuccessfulPayment_CreatesOrder()
    {
        var token = await this.GetAuthTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var product = await this.CreateTestProductAsync(300m);
        await this.client.PostAsJsonAsync("/cart/items", new { productId = product.ProductId, variantId = (Guid?)null, quantity = 1 });

        var address = await this.CreateTestAddressAsync(token);
        var shippingMethod = await this.CreateTestShippingMethodAsync();
        var userId = await this.GetUserIdAsync(token);

        // Simulate payment callback
        var callbackRequest = new
        {
            paymentReference = "PAYOO_123456",
            userId,
            addressId = address.Id,
            shippingMethodCode = shippingMethod.MethodCode,
            shippingCarrierCode = shippingMethod.CarrierCode,
            discountCode = (string?)null,
            shippingCode = (string?)null,
            notes = "Callback test",
        };

        var response = await this.client.PostAsJsonAsync("/payments/payoo/callback", callbackRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PayooCallbackResult>();
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.OrderId);

        // Verify order exists
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var order = await dbContext.Orders
            .FirstOrDefaultAsync(o => o.Id == result.OrderId);

        Assert.NotNull(order);
        Assert.Equal(PaymentMethod.Payoo, order.PaymentMethod);
        Assert.Equal("PAYOO_123456", order.PaymentReference);
    }

    /// <summary>
    /// Test Payoo callback is idempotent - calling twice does not create duplicate orders.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task PayooCallback_CalledTwice_IsIdempotent()
    {
        var token = await this.GetAuthTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var product = await this.CreateTestProductAsync(250m);
        await this.client.PostAsJsonAsync("/cart/items", new { productId = product.ProductId, variantId = (Guid?)null, quantity = 1 });

        var address = await this.CreateTestAddressAsync(token);
        var shippingMethod = await this.CreateTestShippingMethodAsync();
        var userId = await this.GetUserIdAsync(token);

        var callbackRequest = new
        {
            paymentReference = "PAYOO_IDEMPOTENT_TEST",
            userId,
            addressId = address.Id,
            shippingMethodCode = shippingMethod.MethodCode,
            shippingCarrierCode = shippingMethod.CarrierCode,
            discountCode = (string?)null,
            shippingCode = (string?)null,
            notes = "Idempotent test",
        };

        // First callback
        var response1 = await this.client.PostAsJsonAsync("/payments/payoo/callback", callbackRequest);
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        var result1 = await response1.Content.ReadFromJsonAsync<PayooCallbackResult>();
        Assert.NotNull(result1);
        Assert.True(result1.Success);
        var orderId1 = result1.OrderId;

        // Second callback with same payment reference
        var response2 = await this.client.PostAsJsonAsync("/payments/payoo/callback", callbackRequest);
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
        var result2 = await response2.Content.ReadFromJsonAsync<PayooCallbackResult>();
        Assert.NotNull(result2);
        Assert.True(result2.Success);
        Assert.Equal(orderId1, result2.OrderId);

        // Verify only one order exists
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var orders = await dbContext.Orders
            .Where(o => o.PaymentReference == "PAYOO_IDEMPOTENT_TEST")
            .ToListAsync();

        Assert.Single(orders);
    }

    /// <summary>
    /// Test checkout without auth returns 401.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task CheckoutSubmit_WithoutAuth_Returns401()
    {
        var request = new
        {
            addressId = Guid.NewGuid(),
            shippingMethodCode = "STD",
            shippingCarrierCode = "GHN",
            paymentMethod = PaymentMethod.COD,
        };

        var response = await this.client.PostAsJsonAsync("/checkout/submit", request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
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

    private async Task<Guid> GetUserIdAsync(string token)
    {
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await dbContext.Users.FirstAsync();
        return user.Id;
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

    private async Task<TestAddress> CreateTestAddressAsync(string token)
    {
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await dbContext.Users.FirstAsync();

        var address = new Address
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            FullName = "Test User",
            Phone = "0912345678",
            AddressLine = "123 Test St",
            Ward = "Test Ward",
            District = "Test District",
            City = "Hanoi",
            IsDefault = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        dbContext.Addresses.Add(address);
        await dbContext.SaveChangesAsync();

        return new TestAddress(address.Id, address.FullName, address.Phone, address.City);
    }

    private async Task<TestShippingMethod> CreateTestShippingMethodAsync()
    {
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var method = new ShippingMethod
        {
            Id = Guid.NewGuid(),
            Code = "STD",
            Name = "Standard",
            Description = "Standard shipping",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var carrier = new ShippingCarrier
        {
            Id = Guid.NewGuid(),
            ShippingMethodId = method.Id,
            Code = "GHN",
            Name = "Giao Hang Nhanh",
            Description = "GHN",
            SupportsCOD = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        dbContext.ShippingMethods.Add(method);
        dbContext.ShippingCarriers.Add(carrier);
        await dbContext.SaveChangesAsync();

        return new TestShippingMethod(method.Code, carrier.Code);
    }

    private record TestProduct(Guid ProductId, string Name, string Slug, Guid VariantId, decimal Price);

    private record TestAddress(Guid Id, string FullName, string Phone, string City);

    private record TestShippingMethod(string MethodCode, string CarrierCode);
}
