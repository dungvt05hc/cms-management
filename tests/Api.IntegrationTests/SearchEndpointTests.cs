// <copyright file="SearchEndpointTests.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Application.Abstractions;
using Application.Features.AdminAuth.Login;
using Application.Features.Products.CreateProduct;
using Application.Features.Search;
using Infrastructure.Auth;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.IntegrationTests;

/// <summary>
/// Integration tests for the Search endpoints.
/// </summary>
public sealed class SearchEndpointTests : IDisposable
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchEndpointTests"/> class.
    /// </summary>
    public SearchEndpointTests()
    {
        this.factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove existing database configurations
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

                    // Add in-memory database for testing
                    var dbName = $"TestDb_{Guid.NewGuid()}";
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
    /// Tests that search suggestions return matching products (happy path).
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetSuggestions_WithValidQuery_ReturnsSuggestions()
    {
        // Arrange: Create admin token and add products
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Create test products
        var product1 = new CreateProductCommand(
            "Gaming Laptop Pro",
            "gaming-laptop-pro",
            "High-performance gaming laptop",
            null,
            null,
            null,
            null,
            true,
            false,
            new List<CreateProductVariantDto>
            {
                new("SKU-LAPTOP-001", "16GB RAM", 1999.99m, 10),
            });

        var product2 = new CreateProductCommand(
            "Gaming Mouse",
            "gaming-mouse",
            "RGB gaming mouse",
            null,
            null,
            null,
            null,
            true,
            false,
            new List<CreateProductVariantDto>
            {
                new("SKU-MOUSE-001", "Black", 49.99m, 50),
            });

        var product3 = new CreateProductCommand(
            "Office Laptop",
            "office-laptop",
            "Business laptop",
            null,
            null,
            null,
            null,
            true,
            false,
            new List<CreateProductVariantDto>
            {
                new("SKU-OFFICE-001", "8GB RAM", 899.99m, 15),
            });

        await this.client.PostAsJsonAsync("/admin/products", product1);
        await this.client.PostAsJsonAsync("/admin/products", product2);
        await this.client.PostAsJsonAsync("/admin/products", product3);

        // Remove auth header to test anonymous access
        this.client.DefaultRequestHeaders.Authorization = null;

        // Act: Search for "gaming"
        var response = await this.client.GetAsync("/search/suggest?q=gaming");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var suggestions = await response.Content.ReadFromJsonAsync<List<SearchSuggestionDto>>();
        Assert.NotNull(suggestions);
        Assert.Equal(2, suggestions.Count);
        Assert.Contains(suggestions, s => s.Name == "Gaming Laptop Pro");
        Assert.Contains(suggestions, s => s.Name == "Gaming Mouse");
    }

    /// <summary>
    /// Tests that search suggestions return empty list for short queries.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetSuggestions_WithShortQuery_ReturnsEmpty()
    {
        // Arrange: Create admin token and add a product
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var product = new CreateProductCommand(
            "Test Product",
            "test-product",
            "Test description",
            null,
            null,
            null,
            null,
            true,
            false,
            new List<CreateProductVariantDto>
            {
                new("SKU-TEST-001", "Variant", 99.99m, 5),
            });

        await this.client.PostAsJsonAsync("/admin/products", product);

        // Remove auth header to test anonymous access
        this.client.DefaultRequestHeaders.Authorization = null;

        // Act: Search with single character (too short)
        var response = await this.client.GetAsync("/search/suggest?q=t");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var suggestions = await response.Content.ReadFromJsonAsync<List<SearchSuggestionDto>>();
        Assert.NotNull(suggestions);
        Assert.Empty(suggestions);
    }

    /// <summary>
    /// Tests that search suggestions respect the limit parameter.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetSuggestions_WithLimit_RespectsLimit()
    {
        // Arrange: Create admin token and add multiple products
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Create 5 products with "Laptop" in the name
        for (int i = 1; i <= 5; i++)
        {
            var product = new CreateProductCommand(
                $"Laptop Model {i}",
                $"laptop-model-{i}",
                $"Description {i}",
                null,
                null,
                null,
                null,
                true,
                false,
                new List<CreateProductVariantDto>
                {
                    new($"SKU-LAPTOP-{i:D3}", "Standard", 999.99m, 10),
                });

            await this.client.PostAsJsonAsync("/admin/products", product);
        }

        // Remove auth header to test anonymous access
        this.client.DefaultRequestHeaders.Authorization = null;

        // Act: Search with limit of 3
        var response = await this.client.GetAsync("/search/suggest?q=laptop&limit=3");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var suggestions = await response.Content.ReadFromJsonAsync<List<SearchSuggestionDto>>();
        Assert.NotNull(suggestions);
        Assert.Equal(3, suggestions.Count);
    }

    /// <summary>
    /// Tests that inactive products are excluded from suggestions.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetSuggestions_OnlyReturnsActiveProducts()
    {
        // Arrange: Create admin token and add products
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Create active product
        var activeProduct = new CreateProductCommand(
            "Active Keyboard",
            "active-keyboard",
            "Active product",
            null,
            null,
            null,
            null,
            true, // IsActive
            false,
            new List<CreateProductVariantDto>
            {
                new("SKU-ACTIVE-001", "Standard", 79.99m, 20),
            });

        // Create inactive product
        var inactiveProduct = new CreateProductCommand(
            "Inactive Keyboard",
            "inactive-keyboard",
            "Inactive product",
            null,
            null,
            null,
            null,
            false, // IsActive
            false,
            new List<CreateProductVariantDto>
            {
                new("SKU-INACTIVE-001", "Standard", 79.99m, 0),
            });

        await this.client.PostAsJsonAsync("/admin/products", activeProduct);
        await this.client.PostAsJsonAsync("/admin/products", inactiveProduct);

        // Remove auth header to test anonymous access
        this.client.DefaultRequestHeaders.Authorization = null;

        // Act: Search for "keyboard"
        var response = await this.client.GetAsync("/search/suggest?q=keyboard");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var suggestions = await response.Content.ReadFromJsonAsync<List<SearchSuggestionDto>>();
        Assert.NotNull(suggestions);
        Assert.Single(suggestions);
        Assert.Equal("Active Keyboard", suggestions[0].Name);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.client?.Dispose();
        this.factory?.Dispose();
    }

    private async Task<string> GetAdminTokenAsync()
    {
        var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var dateTime = scope.ServiceProvider.GetRequiredService<IDateTime>();

        var superAdmin = new Domain.Entities.StaffUser
        {
            Id = Guid.NewGuid(),
            Email = "admin@test.com",
            FullName = "Admin",
            PasswordHash = passwordHasher.HashPassword("Admin123!"),
            Role = Domain.Entities.StaffRole.SuperAdmin,
            IsActive = true,
            CreatedAt = dateTime.UtcNow,
            UpdatedAt = dateTime.UtcNow,
        };

        dbContext.StaffUsers.Add(superAdmin);
        await dbContext.SaveChangesAsync();

        var loginCommand = new AdminLoginCommand("admin@test.com", "Admin123!");
        var loginResponse = await this.client.PostAsJsonAsync("/admin/auth/login", loginCommand);
        loginResponse.EnsureSuccessStatusCode();
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AdminLoginResult>();
        return loginResult!.AccessToken;
    }
}
