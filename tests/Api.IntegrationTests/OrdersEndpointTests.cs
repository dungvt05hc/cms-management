// <copyright file="OrdersEndpointTests.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Application.Abstractions;
using Application.Features.Auth.Login;
using Application.Features.Orders;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.IntegrationTests;

/// <summary>
/// Integration tests for orders endpoints.
/// </summary>
public sealed class OrdersEndpointTests : IDisposable
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrdersEndpointTests"/> class.
    /// </summary>
    public OrdersEndpointTests()
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

                    var dbName = $"OrdersTestDb_{Guid.NewGuid()}";
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
    /// Test that GET /me/orders returns user's orders only.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetOrders_ReturnsUserOrdersOnly()
    {
        var token = await this.GetAuthTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Create test order
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var otherUserId = Guid.NewGuid();

        var userOrder = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = OrderStatus.Processing,
            PaymentMethod = PaymentMethod.COD,
            Subtotal = 100,
            Total = 100,
            ShippingFullName = "Test User",
            ShippingPhone = "1234567890",
            ShippingAddressLine = "123 Test St",
            ShippingWard = "Ward 1",
            ShippingDistrict = "District 1",
            ShippingCity = "City",
            ShippingMethodCode = "STANDARD",
            ShippingCarrierCode = "GHTK",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var otherOrder = new Order
        {
            Id = Guid.NewGuid(),
            UserId = otherUserId,
            Status = OrderStatus.Processing,
            PaymentMethod = PaymentMethod.COD,
            Subtotal = 200,
            Total = 200,
            ShippingFullName = "Other User",
            ShippingPhone = "0987654321",
            ShippingAddressLine = "456 Test St",
            ShippingWard = "Ward 2",
            ShippingDistrict = "District 2",
            ShippingCity = "City",
            ShippingMethodCode = "EXPRESS",
            ShippingCarrierCode = "GHN",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        dbContext.Orders.Add(userOrder);
        dbContext.Orders.Add(otherOrder);
        await dbContext.SaveChangesAsync();

        // Get orders
        var response = await this.client.GetAsync("/me/orders");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var orders = await response.Content.ReadFromJsonAsync<List<OrderDto>>();
        Assert.NotNull(orders);
        Assert.Single(orders);
        Assert.Equal(userOrder.Id, orders[0].Id);
    }

    /// <summary>
    /// Test that GET /me/orders?status= filters by status.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetOrders_WithStatusFilter_ReturnsFilteredOrders()
    {
        var token = await this.GetAuthTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        var processingOrder = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = OrderStatus.Processing,
            PaymentMethod = PaymentMethod.COD,
            Subtotal = 100,
            Total = 100,
            ShippingFullName = "Test User",
            ShippingPhone = "1234567890",
            ShippingAddressLine = "123 Test St",
            ShippingWard = "Ward 1",
            ShippingDistrict = "District 1",
            ShippingCity = "City",
            ShippingMethodCode = "STANDARD",
            ShippingCarrierCode = "GHTK",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var shippingOrder = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = OrderStatus.Shipping,
            PaymentMethod = PaymentMethod.COD,
            Subtotal = 200,
            Total = 200,
            ShippingFullName = "Test User",
            ShippingPhone = "1234567890",
            ShippingAddressLine = "123 Test St",
            ShippingWard = "Ward 1",
            ShippingDistrict = "District 1",
            ShippingCity = "City",
            ShippingMethodCode = "STANDARD",
            ShippingCarrierCode = "GHTK",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        dbContext.Orders.Add(processingOrder);
        dbContext.Orders.Add(shippingOrder);
        await dbContext.SaveChangesAsync();

        // Get processing orders only
        var response = await this.client.GetAsync("/me/orders?status=0");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var orders = await response.Content.ReadFromJsonAsync<List<OrderDto>>();
        Assert.NotNull(orders);
        Assert.Single(orders);
        Assert.Equal(OrderStatus.Processing, orders[0].Status);
    }

    /// <summary>
    /// Test that GET /me/orders/{id} returns order detail.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetOrderById_ReturnsOrderDetail()
    {
        var token = await this.GetAuthTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = OrderStatus.Processing,
            PaymentMethod = PaymentMethod.COD,
            Subtotal = 100,
            Total = 100,
            ShippingFullName = "Test User",
            ShippingPhone = "1234567890",
            ShippingAddressLine = "123 Test St",
            ShippingWard = "Ward 1",
            ShippingDistrict = "District 1",
            ShippingCity = "City",
            ShippingMethodCode = "STANDARD",
            ShippingCarrierCode = "GHTK",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();

        var response = await this.client.GetAsync($"/me/orders/{order.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var orderDto = await response.Content.ReadFromJsonAsync<OrderDto>();
        Assert.NotNull(orderDto);
        Assert.Equal(order.Id, orderDto.Id);
        Assert.Equal(order.Total, orderDto.Total);
    }

    /// <summary>
    /// Test that GET /me/orders/{id} returns 404 for order that doesn't belong to user.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetOrderById_OtherUserOrder_Returns404()
    {
        var token = await this.GetAuthTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var otherUserId = Guid.NewGuid();

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = otherUserId,
            Status = OrderStatus.Processing,
            PaymentMethod = PaymentMethod.COD,
            Subtotal = 100,
            Total = 100,
            ShippingFullName = "Other User",
            ShippingPhone = "0987654321",
            ShippingAddressLine = "456 Test St",
            ShippingWard = "Ward 2",
            ShippingDistrict = "District 2",
            ShippingCity = "City",
            ShippingMethodCode = "EXPRESS",
            ShippingCarrierCode = "GHN",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();

        var response = await this.client.GetAsync($"/me/orders/{order.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    /// <summary>
    /// Test that POST /me/orders/{id}/cancel succeeds when status is Processing.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task CancelOrder_ProcessingStatus_Succeeds()
    {
        var token = await this.GetAuthTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = OrderStatus.Processing,
            PaymentMethod = PaymentMethod.COD,
            Subtotal = 100,
            Total = 100,
            ShippingFullName = "Test User",
            ShippingPhone = "1234567890",
            ShippingAddressLine = "123 Test St",
            ShippingWard = "Ward 1",
            ShippingDistrict = "District 1",
            ShippingCity = "City",
            ShippingMethodCode = "STANDARD",
            ShippingCarrierCode = "GHTK",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();

        var request = new { reasonCode = "CHANGED_MIND", note = "Test cancellation" };
        var response = await this.client.PostAsJsonAsync($"/me/orders/{order.Id}/cancel", request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify status changed - need to create new scope to see changes
        using var verifyScope = this.factory.Services.CreateScope();
        var verifyContext = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var updatedOrder = await verifyContext.Orders.FindAsync(order.Id);
        Assert.NotNull(updatedOrder);
        Assert.Equal(OrderStatus.Cancelled, updatedOrder.Status);
    }

    /// <summary>
    /// Test that POST /me/orders/{id}/cancel fails when status is not Processing.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task CancelOrder_NonProcessingStatus_FailsWithBadRequest()
    {
        var token = await this.GetAuthTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = OrderStatus.Shipping,
            PaymentMethod = PaymentMethod.COD,
            Subtotal = 100,
            Total = 100,
            ShippingFullName = "Test User",
            ShippingPhone = "1234567890",
            ShippingAddressLine = "123 Test St",
            ShippingWard = "Ward 1",
            ShippingDistrict = "District 1",
            ShippingCity = "City",
            ShippingMethodCode = "STANDARD",
            ShippingCarrierCode = "GHTK",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();

        var request = new { reasonCode = "CHANGED_MIND", note = "Test cancellation" };
        var response = await this.client.PostAsJsonAsync($"/me/orders/{order.Id}/cancel", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        // Verify status unchanged
        var updatedOrder = await dbContext.Orders.FindAsync(order.Id);
        Assert.NotNull(updatedOrder);
        Assert.Equal(OrderStatus.Shipping, updatedOrder.Status);
    }

    /// <summary>
    /// Test that POST /me/orders/{id}/confirm-received succeeds when status is Delivered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task ConfirmOrder_DeliveredStatus_Succeeds()
    {
        var token = await this.GetAuthTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = OrderStatus.Delivered,
            PaymentMethod = PaymentMethod.COD,
            Subtotal = 100,
            Total = 100,
            ShippingFullName = "Test User",
            ShippingPhone = "1234567890",
            ShippingAddressLine = "123 Test St",
            ShippingWard = "Ward 1",
            ShippingDistrict = "District 1",
            ShippingCity = "City",
            ShippingMethodCode = "STANDARD",
            ShippingCarrierCode = "GHTK",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow.AddDays(-2),
        };

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();

        var response = await this.client.PostAsJsonAsync($"/me/orders/{order.Id}/confirm-received", new { });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify UpdatedAt changed - need to create new scope to see changes
        using var verifyScope = this.factory.Services.CreateScope();
        var verifyContext = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();
        var updatedOrder = await verifyContext.Orders.FindAsync(order.Id);
        Assert.NotNull(updatedOrder);
        Assert.True(updatedOrder.UpdatedAt > order.UpdatedAt);
    }

    /// <summary>
    /// Test that POST /me/orders/{id}/confirm-received fails when status is not Delivered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task ConfirmOrder_NonDeliveredStatus_FailsWithBadRequest()
    {
        var token = await this.GetAuthTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = OrderStatus.Processing,
            PaymentMethod = PaymentMethod.COD,
            Subtotal = 100,
            Total = 100,
            ShippingFullName = "Test User",
            ShippingPhone = "1234567890",
            ShippingAddressLine = "123 Test St",
            ShippingWard = "Ward 1",
            ShippingDistrict = "District 1",
            ShippingCity = "City",
            ShippingMethodCode = "STANDARD",
            ShippingCarrierCode = "GHTK",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();

        var response = await this.client.PostAsJsonAsync($"/me/orders/{order.Id}/confirm-received", new { });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.client.Dispose();
        this.factory.Dispose();
    }

    private async Task<string> GetAuthTokenAsync()
    {
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var dateTime = scope.ServiceProvider.GetRequiredService<IDateTime>();

        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var existingUser = await dbContext.Users.FindAsync(userId);
        if (existingUser == null)
        {
            var user = new User
            {
                Id = userId,
                Phone = "0912345678",
                Email = "test@example.com",
                FullName = "Test User",
                PasswordHash = passwordHasher.HashPassword("Password123!"),
                IsVerified = true,
                CreatedAt = dateTime.UtcNow,
                UpdatedAt = dateTime.UtcNow,
            };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
        }

        var loginRequest = new { phoneOrEmail = "0912345678", password = "Password123!" };
        var response = await this.client.PostAsJsonAsync("/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<LoginResult>();
        return result?.AccessToken ?? throw new InvalidOperationException("Failed to get auth token");
    }
}
