// <copyright file="ShippingEndpointTests.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Application.Abstractions;
using Application.Features.Auth.Login;
using Application.Features.Shipping;
using Application.Features.Shipping.GetShippingQuote;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.IntegrationTests;

/// <summary>
/// Integration tests for shipping endpoints.
/// </summary>
public sealed class ShippingEndpointTests : IDisposable
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShippingEndpointTests"/> class.
    /// </summary>
    public ShippingEndpointTests()
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

                    var dbName = $"ShippingTestDb_{Guid.NewGuid()}";
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
    /// Test that anonymous users can get shipping methods (contains bootstrap data).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetShippingMethods_Anonymous_ReturnsBootstrapMethods()
    {
        var response = await this.client.GetAsync("/shipping/methods");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var methods = await response.Content.ReadFromJsonAsync<List<ShippingMethodDto>>();
        Assert.NotNull(methods);
        Assert.Equal(2, methods.Count); // STANDARD and EXPRESS from bootstrap
        Assert.Contains(methods, m => m.Code == "STANDARD");
        Assert.Contains(methods, m => m.Code == "EXPRESS");
    }

    /// <summary>
    /// Test that anonymous users can get shipping methods with additional custom data.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetShippingMethods_WithAdditionalData_ReturnsAllMethods()
    {
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Add custom method on top of bootstrap data
        var method = new ShippingMethod
        {
            Id = Guid.NewGuid(),
            Code = "CUSTOM",
            Name = "Custom Shipping",
            Description = "Custom delivery",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var carrier = new ShippingCarrier
        {
            Id = Guid.NewGuid(),
            ShippingMethodId = method.Id,
            Code = "CUSTOM_CARRIER",
            Name = "Custom Carrier",
            Description = "Fast delivery",
            SupportsCOD = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        dbContext.ShippingMethods.Add(method);
        dbContext.ShippingCarriers.Add(carrier);
        await dbContext.SaveChangesAsync();

        var response = await this.client.GetAsync("/shipping/methods");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var methods = await response.Content.ReadFromJsonAsync<List<ShippingMethodDto>>();
        Assert.NotNull(methods);
        Assert.Equal(3, methods.Count); // STANDARD, EXPRESS from bootstrap + CUSTOM
        Assert.Contains(methods, m => m.Code == "CUSTOM");
        var customMethod = methods.First(m => m.Code == "CUSTOM");
        Assert.Single(customMethod.Carriers);
        Assert.Equal("CUSTOM_CARRIER", customMethod.Carriers[0].Code);
    }

    /// <summary>
    /// Test that authenticated users can get a shipping quote.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetShippingQuote_ValidRequest_ReturnsQuote()
    {
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Phone = "0901234567",
            Email = "test@example.com",
            FullName = "Test User",
            PasswordHash = passwordHasher.HashPassword("Password123!"),
            IsVerified = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var address = new Address
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            FullName = "Test User",
            Phone = "0901234567",
            AddressLine = "123 Test St",
            Ward = "Ward 1",
            District = "District 1",
            City = "Ho Chi Minh",
            IsDefault = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var method = new ShippingMethod
        {
            Id = Guid.NewGuid(),
            Code = "STANDARD",
            Name = "Standard Shipping",
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
            SupportsCOD = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        dbContext.Users.Add(user);
        dbContext.Addresses.Add(address);
        dbContext.ShippingMethods.Add(method);
        dbContext.ShippingCarriers.Add(carrier);
        await dbContext.SaveChangesAsync();

        var loginResponse = await this.client.PostAsJsonAsync("/auth/login", new LoginQuery(user.Phone, "Password123!"));
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResult>();
        Assert.NotNull(loginResult);

        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.AccessToken);

        var quoteRequest = new
        {
            AddressId = address.Id,
            MethodCode = "STANDARD",
            CarrierCode = "GHN",
            Weight = 1000,
        };

        var response = await this.client.PostAsJsonAsync("/shipping/quote", quoteRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var quote = await response.Content.ReadFromJsonAsync<ShippingQuoteDto>();
        Assert.NotNull(quote);
        Assert.True(quote.Fee > 0);
        Assert.True(quote.EstimatedDeliveryDays > 0);
    }

    /// <summary>
    /// Test that shipping quote fails for invalid method.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetShippingQuote_InvalidMethod_ReturnsBadRequest()
    {
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Phone = "0901234568",
            Email = "test2@example.com",
            FullName = "Test User 2",
            PasswordHash = passwordHasher.HashPassword("Password123!"),
            IsVerified = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var address = new Address
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            FullName = "Test User 2",
            Phone = "0901234568",
            AddressLine = "123 Test St",
            Ward = "Ward 1",
            District = "District 1",
            City = "Ho Chi Minh",
            IsDefault = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        dbContext.Users.Add(user);
        dbContext.Addresses.Add(address);
        await dbContext.SaveChangesAsync();

        var loginResponse = await this.client.PostAsJsonAsync("/auth/login", new LoginQuery(user.Phone, "Password123!"));
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResult>();
        Assert.NotNull(loginResult);

        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.AccessToken);

        var quoteRequest = new
        {
            AddressId = address.Id,
            MethodCode = "INVALID_METHOD",
            CarrierCode = "GHN",
            Weight = 1000,
        };

        var response = await this.client.PostAsJsonAsync("/shipping/quote", quoteRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// Test that shipping quote fails when unauthorized.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetShippingQuote_Unauthorized_ReturnsUnauthorized()
    {
        var quoteRequest = new
        {
            AddressId = Guid.NewGuid(),
            MethodCode = "STANDARD",
            CarrierCode = "GHN",
            Weight = 1000,
        };

        var response = await this.client.PostAsJsonAsync("/shipping/quote", quoteRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Dispose the test resources.
    /// </summary>
    public void Dispose()
    {
        this.client?.Dispose();
        this.factory?.Dispose();
    }
}
