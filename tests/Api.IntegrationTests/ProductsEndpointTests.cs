// <copyright file="ProductsEndpointTests.cs" company="CMS Management">
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
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.IntegrationTests;

/// <summary>
/// Integration tests for the public Products endpoints.
/// </summary>
public sealed class ProductsEndpointTests : IDisposable
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductsEndpointTests"/> class.
    /// </summary>
    public ProductsEndpointTests()
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
    /// Tests that anonymous users can get products including featured filter.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetProducts_WithFeaturedFilter_ReturnsOnlyFeaturedProducts()
    {
        // Arrange: Create admin token and add products
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Create a featured product
        var featuredCommand = new CreateProductCommand(
            "Featured Product",
            "featured-product",
            "Featured product description",
            null,
            null,
            null,
            null,
            true,
            true,
            new List<CreateProductVariantDto>
            {
                new("SKU-FEAT-001", "Variant 1", 199.99m, 50),
            });

        await this.client.PostAsJsonAsync("/admin/products", featuredCommand);

        // Create a non-featured product
        var regularCommand = new CreateProductCommand(
            "Regular Product",
            "regular-product",
            "Regular product description",
            null,
            null,
            null,
            null,
            true,
            false,
            new List<CreateProductVariantDto>
            {
                new("SKU-REG-001", "Variant 1", 99.99m, 20),
            });

        await this.client.PostAsJsonAsync("/admin/products", regularCommand);

        // Remove auth header to test anonymous access
        this.client.DefaultRequestHeaders.Authorization = null;

        // Act: Get featured products
        var response = await this.client.GetAsync("/products?featured=true&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("Featured Product", result.Items[0].Name);
        Assert.True(result.Items[0].IsFeatured);
    }

    /// <summary>
    /// Tests that anonymous users can get all products when featured filter is not specified.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetProducts_WithoutFeaturedFilter_ReturnsAllProducts()
    {
        // Arrange: Create admin token and add products
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Create a featured product
        var featuredCommand = new CreateProductCommand(
            "Featured Product 2",
            "featured-product-2",
            null,
            null,
            null,
            null,
            null,
            true,
            true,
            new List<CreateProductVariantDto>
            {
                new("SKU-FEAT-002", "Variant 1", 199.99m, 50),
            });

        await this.client.PostAsJsonAsync("/admin/products", featuredCommand);

        // Create a non-featured product
        var regularCommand = new CreateProductCommand(
            "Regular Product 2",
            "regular-product-2",
            null,
            null,
            null,
            null,
            null,
            true,
            false,
            new List<CreateProductVariantDto>
            {
                new("SKU-REG-002", "Variant 1", 99.99m, 20),
            });

        await this.client.PostAsJsonAsync("/admin/products", regularCommand);

        // Remove auth header to test anonymous access
        this.client.DefaultRequestHeaders.Authorization = null;

        // Act: Get all products (no featured filter)
        var response = await this.client.GetAsync("/products?pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
    }

    /// <summary>
    /// Tests that featured products endpoint works without authentication.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetFeaturedProducts_Anonymous_ReturnsSuccess()
    {
        // Act: Get featured products without authentication
        var response = await this.client.GetAsync("/products?featured=true");

        // Assert: Should work without authentication
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    /// <summary>
    /// Tests that getting a product by slug returns the product details.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetProductBySlug_ValidSlug_ReturnsProduct()
    {
        // Arrange: Create admin token and add a product
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createCommand = new CreateProductCommand(
            "Test Product By Slug",
            "test-product-by-slug",
            "Test product description",
            null,
            null,
            null,
            null,
            true,
            false,
            new List<CreateProductVariantDto>
            {
                new("SKU-SLUG-001", "Variant 1", 99.99m, 10),
                new("SKU-SLUG-002", "Variant 2", 149.99m, 5),
            });

        await this.client.PostAsJsonAsync("/admin/products", createCommand);

        // Remove auth header to test anonymous access
        this.client.DefaultRequestHeaders.Authorization = null;

        // Act: Get product by slug
        var response = await this.client.GetAsync("/products/test-product-by-slug");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(result);
        Assert.Equal("Test Product By Slug", result.Name);
        Assert.Equal("test-product-by-slug", result.Slug);
        Assert.Equal(2, result.Variants.Count);
    }

    /// <summary>
    /// Tests that getting a product by invalid slug returns 404.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetProductBySlug_InvalidSlug_Returns404()
    {
        // Act: Get product by invalid slug
        var response = await this.client.GetAsync("/products/non-existent-slug");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    /// <summary>
    /// Tests that getting product suggestions returns products from the same category.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetProductSuggestions_ValidSlug_ReturnsSuggestions()
    {
        // Arrange: Create admin token, category, and multiple products in the same category
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Create a category first
        var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var dateTime = scope.ServiceProvider.GetRequiredService<IDateTime>();

        var category = new Domain.Entities.Category
        {
            Id = Guid.NewGuid(),
            Name = "Electronics",
            CreatedAt = dateTime.UtcNow,
            UpdatedAt = dateTime.UtcNow,
        };

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        // Create main product
        var mainProduct = new CreateProductCommand(
            "Main Product",
            "main-product",
            "Main product description",
            category.Id,
            null,
            null,
            null,
            true,
            false,
            new List<CreateProductVariantDto>
            {
                new("SKU-MAIN-001", "Variant 1", 199.99m, 10),
            });

        await this.client.PostAsJsonAsync("/admin/products", mainProduct);

        // Create suggested products in the same category
        for (int i = 1; i <= 3; i++)
        {
            var suggestedProduct = new CreateProductCommand(
                $"Suggested Product {i}",
                $"suggested-product-{i}",
                $"Suggested product {i} description",
                category.Id,
                null,
                null,
                null,
                true,
                false,
                new List<CreateProductVariantDto>
                {
                    new($"SKU-SUGG-{i:D3}", "Variant 1", 99.99m + i, 10),
                });

            await this.client.PostAsJsonAsync("/admin/products", suggestedProduct);
        }

        // Remove auth header to test anonymous access
        this.client.DefaultRequestHeaders.Authorization = null;

        // Act: Get product suggestions
        var response = await this.client.GetAsync("/products/main-product/suggestions?limit=4");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.All(result, p => Assert.Equal(category.Id, p.CategoryId));
        Assert.DoesNotContain(result, p => p.Slug == "main-product");
    }

    /// <summary>
    /// Tests that getting suggestions for product with no category returns empty list.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetProductSuggestions_ProductWithNoCategory_ReturnsEmpty()
    {
        // Arrange: Create admin token and product without category
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createCommand = new CreateProductCommand(
            "Product Without Category",
            "product-without-category",
            "Product without category description",
            null,
            null,
            null,
            null,
            true,
            false,
            new List<CreateProductVariantDto>
            {
                new("SKU-NO-CAT-001", "Variant 1", 99.99m, 10),
            });

        await this.client.PostAsJsonAsync("/admin/products", createCommand);

        // Remove auth header to test anonymous access
        this.client.DefaultRequestHeaders.Authorization = null;

        // Act: Get product suggestions
        var response = await this.client.GetAsync("/products/product-without-category/suggestions");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        Assert.NotNull(result);
        Assert.Empty(result);
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
