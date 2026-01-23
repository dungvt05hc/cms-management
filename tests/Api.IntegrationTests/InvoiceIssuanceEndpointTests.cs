// <copyright file="InvoiceIssuanceEndpointTests.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Application.Abstractions;
using Application.Features.AdminAuth.Login;
using Application.Features.Invoices.RunIssuance;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.IntegrationTests;

/// <summary>
/// Integration tests for invoice issuance endpoints.
/// </summary>
public sealed class InvoiceIssuanceEndpointTests : IDisposable
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="InvoiceIssuanceEndpointTests"/> class.
    /// </summary>
    public InvoiceIssuanceEndpointTests()
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

                    var dbName = $"InvoiceIssuanceTestDb_{Guid.NewGuid()}";
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
    /// Test that admin can trigger invoice issuance.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task RunIssuance_WithAdminAuth_ReturnsOk()
    {
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await this.client.PostAsync("/admin/invoices/run-issuance", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<RunIssuanceResult>();
        Assert.NotNull(result);
        Assert.Equal(0, result.ProcessedCount);
    }

    /// <summary>
    /// Test that run issuance requires admin authentication.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task RunIssuance_WithoutAuth_ReturnsUnauthorized()
    {
        var response = await this.client.PostAsync("/admin/invoices/run-issuance", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Test that eligible orders are selected for issuance (Delivered + 10 days).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task RunIssuance_SelectsEligibleOrders()
    {
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var dateTime = scope.ServiceProvider.GetRequiredService<IDateTime>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Phone = "0987654321",
            Email = "user@test.com",
            FullName = "Test User",
            PasswordHash = passwordHasher.HashPassword("password"),
            IsVerified = true,
            CreatedAt = dateTime.UtcNow,
            UpdatedAt = dateTime.UtcNow,
        };
        dbContext.Users.Add(user);

        var eligibleOrder = new Order
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Subtotal = 100m,
            DiscountAmount = 0m,
            ShippingFee = 20m,
            ShippingDiscount = 0m,
            Total = 120m,
            PaymentMethod = PaymentMethod.COD,
            Status = OrderStatus.Delivered,
            ShippingFullName = "Test User",
            ShippingPhone = "0987654321",
            ShippingAddressLine = "123 Main St",
            ShippingWard = "Ward 1",
            ShippingDistrict = "District 1",
            ShippingCity = "HCMC",
            ShippingMethodCode = "STANDARD",
            ShippingCarrierCode = "GHTK",
            InvoiceRequested = true,
            InvoiceTaxCode = "0123456789",
            InvoiceCompanyName = "Test Company",
            InvoiceCompanyAddress = "123 Business St",
            InvoiceEmail = "invoice@test.com",
            InvoiceIssuedAt = null,
            DeliveredAt = dateTime.UtcNow.AddDays(-11),
            CreatedAt = dateTime.UtcNow.AddDays(-15),
            UpdatedAt = dateTime.UtcNow,
        };
        dbContext.Orders.Add(eligibleOrder);

        var notEligibleOrder = new Order
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Subtotal = 100m,
            DiscountAmount = 0m,
            ShippingFee = 20m,
            ShippingDiscount = 0m,
            Total = 120m,
            PaymentMethod = PaymentMethod.COD,
            Status = OrderStatus.Delivered,
            ShippingFullName = "Test User",
            ShippingPhone = "0987654321",
            ShippingAddressLine = "123 Main St",
            ShippingWard = "Ward 1",
            ShippingDistrict = "District 1",
            ShippingCity = "HCMC",
            ShippingMethodCode = "STANDARD",
            ShippingCarrierCode = "GHTK",
            InvoiceRequested = true,
            InvoiceTaxCode = "0123456789",
            InvoiceCompanyName = "Test Company",
            InvoiceCompanyAddress = "123 Business St",
            InvoiceEmail = "invoice@test.com",
            InvoiceIssuedAt = null,
            DeliveredAt = dateTime.UtcNow.AddDays(-5),
            CreatedAt = dateTime.UtcNow.AddDays(-10),
            UpdatedAt = dateTime.UtcNow,
        };
        dbContext.Orders.Add(notEligibleOrder);

        await dbContext.SaveChangesAsync();

        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await this.client.PostAsync("/admin/invoices/run-issuance", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<RunIssuanceResult>();
        Assert.NotNull(result);
        Assert.Equal(1, result.ProcessedCount);

        // Refresh from database to get updated values
        using var verifyScope = this.factory.Services.CreateScope();
        var verifyDbContext = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();

        var updatedEligibleOrder = await verifyDbContext.Orders.FindAsync(eligibleOrder.Id);
        Assert.NotNull(updatedEligibleOrder);
        Assert.NotNull(updatedEligibleOrder.InvoiceIssuedAt);

        var updatedNotEligibleOrder = await verifyDbContext.Orders.FindAsync(notEligibleOrder.Id);
        Assert.NotNull(updatedNotEligibleOrder);
        Assert.Null(updatedNotEligibleOrder.InvoiceIssuedAt);
    }

    /// <summary>
    /// Test that orders without invoice request are not selected.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task RunIssuance_DoesNotSelectOrdersWithoutInvoiceRequest()
    {
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var dateTime = scope.ServiceProvider.GetRequiredService<IDateTime>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Phone = "0987654322",
            Email = "user2@test.com",
            FullName = "Test User 2",
            PasswordHash = passwordHasher.HashPassword("password"),
            IsVerified = true,
            CreatedAt = dateTime.UtcNow,
            UpdatedAt = dateTime.UtcNow,
        };
        dbContext.Users.Add(user);

        var orderWithoutInvoiceRequest = new Order
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Subtotal = 100m,
            DiscountAmount = 0m,
            ShippingFee = 20m,
            ShippingDiscount = 0m,
            Total = 120m,
            PaymentMethod = PaymentMethod.COD,
            Status = OrderStatus.Delivered,
            ShippingFullName = "Test User",
            ShippingPhone = "0987654321",
            ShippingAddressLine = "123 Main St",
            ShippingWard = "Ward 1",
            ShippingDistrict = "District 1",
            ShippingCity = "HCMC",
            ShippingMethodCode = "STANDARD",
            ShippingCarrierCode = "GHTK",
            InvoiceRequested = false,
            DeliveredAt = dateTime.UtcNow.AddDays(-11),
            CreatedAt = dateTime.UtcNow.AddDays(-15),
            UpdatedAt = dateTime.UtcNow,
        };
        dbContext.Orders.Add(orderWithoutInvoiceRequest);

        await dbContext.SaveChangesAsync();

        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await this.client.PostAsync("/admin/invoices/run-issuance", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<RunIssuanceResult>();
        Assert.NotNull(result);
        Assert.Equal(0, result.ProcessedCount);
    }

    /// <summary>
    /// Disposes resources.
    /// </summary>
    public void Dispose()
    {
        this.client?.Dispose();
        this.factory?.Dispose();
    }

    private async Task<string> GetAdminTokenAsync()
    {
        var uniqueEmail = $"admin-{Guid.NewGuid()}@test.com";
        var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var dateTime = scope.ServiceProvider.GetRequiredService<IDateTime>();

        var admin = new StaffUser
        {
            Id = Guid.NewGuid(),
            Email = uniqueEmail,
            FullName = "Admin",
            PasswordHash = passwordHasher.HashPassword("Admin123!"),
            Role = StaffRole.Admin,
            IsActive = true,
            CreatedAt = dateTime.UtcNow,
            UpdatedAt = dateTime.UtcNow,
        };

        dbContext.StaffUsers.Add(admin);
        await dbContext.SaveChangesAsync();

        var loginCommand = new AdminLoginCommand(uniqueEmail, "Admin123!");
        var loginResponse = await this.client.PostAsJsonAsync("/admin/auth/login", loginCommand);
        loginResponse.EnsureSuccessStatusCode();
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AdminLoginResult>();

        return loginResult!.AccessToken;
    }
}
