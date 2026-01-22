// <copyright file="ProductsCursorPaginationTests.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Application.Abstractions;
using Application.Features.AdminAuth.Login;
using Application.Features.Products;
using Application.Features.Products.CreateProduct;
using Application.Features.Products.GetProducts;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.IntegrationTests;

/// <summary>
/// Integration tests for the Products cursor pagination and sorting.
/// </summary>
public sealed class ProductsCursorPaginationTests : IDisposable
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductsCursorPaginationTests"/> class.
    /// </summary>
    public ProductsCursorPaginationTests()
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
    /// Tests cursor pagination with multiple pages.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetProducts_WithCursor_LoadsMultiplePages()
    {
        // Arrange: Seed 35 products
        await this.SeedProductsAsync(35);

        // Remove auth header to test anonymous access
        this.client.DefaultRequestHeaders.Authorization = null;

        // Act: Get first page
        var response1 = await this.client.GetAsync("/products?limit=10");
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        var result1 = await response1.Content.ReadFromJsonAsync<CursorPagedResult<ProductDto>>();
        Assert.NotNull(result1);
        Assert.Equal(10, result1.Items.Count);
        Assert.True(result1.HasMore);
        Assert.NotNull(result1.NextCursor);

        // Act: Get second page using cursor
        var response2 = await this.client.GetAsync($"/products?limit=10&cursor={Uri.EscapeDataString(result1.NextCursor)}");
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
        var result2 = await response2.Content.ReadFromJsonAsync<CursorPagedResult<ProductDto>>();
        Assert.NotNull(result2);
        Assert.Equal(10, result2.Items.Count);
        Assert.True(result2.HasMore);
        Assert.NotNull(result2.NextCursor);

        // Verify no duplicates between pages
        var ids1 = result1.Items.Select(p => p.Id).ToHashSet();
        var ids2 = result2.Items.Select(p => p.Id).ToHashSet();
        Assert.Empty(ids1.Intersect(ids2));

        // Act: Get third page
        var response3 = await this.client.GetAsync($"/products?limit=10&cursor={Uri.EscapeDataString(result2.NextCursor)}");
        Assert.Equal(HttpStatusCode.OK, response3.StatusCode);
        var result3 = await response3.Content.ReadFromJsonAsync<CursorPagedResult<ProductDto>>();
        Assert.NotNull(result3);
        Assert.Equal(10, result3.Items.Count);
        Assert.True(result3.HasMore);
        Assert.NotNull(result3.NextCursor);

        // Act: Get fourth page (last partial page)
        var response4 = await this.client.GetAsync($"/products?limit=10&cursor={Uri.EscapeDataString(result3.NextCursor)}");
        Assert.Equal(HttpStatusCode.OK, response4.StatusCode);
        var result4 = await response4.Content.ReadFromJsonAsync<CursorPagedResult<ProductDto>>();
        Assert.NotNull(result4);
        Assert.Equal(5, result4.Items.Count);
        Assert.False(result4.HasMore);
        Assert.Null(result4.NextCursor);
    }

    /// <summary>
    /// Tests sorting by price ascending.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetProducts_SortByPriceAsc_ReturnsOrderedByPriceAscending()
    {
        // Arrange: Seed products with different prices
        await this.SeedProductsWithPricesAsync(new[] { 100m, 50m, 200m, 75m, 150m });

        this.client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await this.client.GetAsync("/products?sort=priceAsc&limit=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CursorPagedResult<ProductDto>>();
        Assert.NotNull(result);
        Assert.Equal(5, result.Items.Count);

        // Verify order: 50, 75, 100, 150, 200
        var prices = result.Items.SelectMany(p => p.Variants.Select(v => v.Price)).ToList();
        Assert.True(prices.SequenceEqual(new[] { 50m, 75m, 100m, 150m, 200m }));
    }

    /// <summary>
    /// Tests sorting by price descending.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetProducts_SortByPriceDesc_ReturnsOrderedByPriceDescending()
    {
        // Arrange: Seed products with different prices
        await this.SeedProductsWithPricesAsync(new[] { 100m, 50m, 200m, 75m, 150m });

        this.client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await this.client.GetAsync("/products?sort=priceDesc&limit=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CursorPagedResult<ProductDto>>();
        Assert.NotNull(result);
        Assert.Equal(5, result.Items.Count);

        // Verify order: 200, 150, 100, 75, 50
        var prices = result.Items.SelectMany(p => p.Variants.Select(v => v.Price)).ToList();
        Assert.True(prices.SequenceEqual(new[] { 200m, 150m, 100m, 75m, 50m }));
    }

    /// <summary>
    /// Tests cursor pagination with price sorting.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetProducts_CursorWithPriceSort_MaintainsStableOrder()
    {
        // Arrange: Seed 15 products with prices
        var prices = new[] { 50m, 60m, 70m, 80m, 90m, 100m, 110m, 120m, 130m, 140m, 150m, 160m, 170m, 180m, 190m };
        await this.SeedProductsWithPricesAsync(prices);

        this.client.DefaultRequestHeaders.Authorization = null;

        // Act: Get first page sorted by price asc
        var response1 = await this.client.GetAsync("/products?sort=priceAsc&limit=5");
        var result1 = await response1.Content.ReadFromJsonAsync<CursorPagedResult<ProductDto>>();
        Assert.NotNull(result1);
        Assert.Equal(5, result1.Items.Count);

        // Act: Get second page
        var response2 = await this.client.GetAsync($"/products?sort=priceAsc&limit=5&cursor={Uri.EscapeDataString(result1.NextCursor!)}");
        var result2 = await response2.Content.ReadFromJsonAsync<CursorPagedResult<ProductDto>>();
        Assert.NotNull(result2);
        Assert.Equal(5, result2.Items.Count);

        // Assert: No overlap and correct order
        var allIds = result1.Items.Concat(result2.Items).Select(p => p.Id).ToList();
        Assert.Equal(10, allIds.Distinct().Count());

        var allPrices = result1.Items.Concat(result2.Items)
            .SelectMany(p => p.Variants.Select(v => v.Price))
            .ToList();
        Assert.True(allPrices.SequenceEqual(prices.Take(10).OrderBy(p => p)));
    }

    /// <summary>
    /// Tests category filter by slug.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetProducts_FilterByCategory_ReturnsOnlyCategoryProducts()
    {
        // Arrange: Create category and products
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Electronics",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        // Create products in category
        for (int i = 0; i < 5; i++)
        {
            var command = new CreateProductCommand(
                $"Product in Electronics {i}",
                $"product-electronics-{i}",
                null,
                category.Id,
                null,
                null,
                null,
                true,
                false,
                new List<CreateProductVariantDto> { new($"SKU-ELEC-{i}", "Variant", 100m, 10) });

            await this.client.PostAsJsonAsync("/admin/products", command);
        }

        // Create products not in category
        for (int i = 0; i < 3; i++)
        {
            var command = new CreateProductCommand(
                $"Product Other {i}",
                $"product-other-{i}",
                null,
                null,
                null,
                null,
                null,
                true,
                false,
                new List<CreateProductVariantDto> { new($"SKU-OTHER-{i}", "Variant", 100m, 10) });

            await this.client.PostAsJsonAsync("/admin/products", command);
        }

        this.client.DefaultRequestHeaders.Authorization = null;

        // Act: Filter by category slug
        var response = await this.client.GetAsync($"/products?category=Electronics&limit=20");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CursorPagedResult<ProductDto>>();
        Assert.NotNull(result);
        Assert.Equal(5, result.Items.Count);
        Assert.All(result.Items, p => Assert.Equal(category.Id, p.CategoryId));
    }

    /// <summary>
    /// Tests invalid cursor returns empty results gracefully.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetProducts_WithInvalidCursor_ReturnsSuccess()
    {
        // Arrange
        await this.SeedProductsAsync(5);
        this.client.DefaultRequestHeaders.Authorization = null;

        // Act: Use invalid cursor
        var response = await this.client.GetAsync("/products?cursor=invalid-cursor-data&limit=10");

        // Assert: Should still return results (ignores invalid cursor)
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CursorPagedResult<ProductDto>>();
        Assert.NotNull(result);
        Assert.True(result.Items.Count > 0);
    }

    /// <summary>
    /// Disposes the test resources.
    /// </summary>
    public void Dispose()
    {
        this.client.Dispose();
        this.factory.Dispose();
    }

    /// <summary>
    /// Seeds test products.
    /// </summary>
    /// <param name="count">Number of products to seed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task SeedProductsAsync(int count)
    {
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        for (int i = 0; i < count; i++)
        {
            var command = new CreateProductCommand(
                $"Product {i}",
                $"product-{i}",
                $"Description {i}",
                null,
                null,
                null,
                null,
                true,
                false,
                new List<CreateProductVariantDto>
                {
                    new($"SKU-{i:D4}", "Variant", 100m, 10),
                });

            await this.client.PostAsJsonAsync("/admin/products", command);
            await Task.Delay(10); // Ensure different CreatedAt timestamps
        }
    }

    /// <summary>
    /// Seeds test products with specific prices.
    /// </summary>
    /// <param name="prices">The prices for each product.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task SeedProductsWithPricesAsync(decimal[] prices)
    {
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        for (int i = 0; i < prices.Length; i++)
        {
            var command = new CreateProductCommand(
                $"Product Price {prices[i]}",
                $"product-price-{i}",
                null,
                null,
                null,
                null,
                null,
                true,
                false,
                new List<CreateProductVariantDto>
                {
                    new($"SKU-PRICE-{i}", "Variant", prices[i], 10),
                });

            await this.client.PostAsJsonAsync("/admin/products", command);
            await Task.Delay(10);
        }
    }

    /// <summary>
    /// Gets an admin authentication token for testing.
    /// </summary>
    /// <returns>The admin token.</returns>
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
