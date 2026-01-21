// <copyright file="AdminProductsEndpointTests.cs" company="CMS Management">
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
using Application.Features.Products.UpdateProduct;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.IntegrationTests;

/// <summary>
/// Integration tests for the Admin Products endpoints.
/// </summary>
public sealed class AdminProductsEndpointTests : IDisposable
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminProductsEndpointTests"/> class.
    /// </summary>
    public AdminProductsEndpointTests()
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

                    // Add in-memory database
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                    });

                    services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
                });
            });

        this.client = this.factory.CreateClient();
    }

    /// <summary>
    /// Test: Create product with variants returns 201.
    /// </summary>
    /// <returns>A task representing the async operation.</returns>
    [Fact]
    public async Task CreateProduct_WithVariants_Returns201()
    {
        // Arrange
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var command = new CreateProductCommand(
            "Test Product",
            "test-product",
            "Test description",
            null,
            null,
            null,
            null,
            true,
            new List<CreateProductVariantDto>
            {
                new("SKU-001", "Variant 1", 100.00m, 10),
                new("SKU-002", "Variant 2", 150.00m, 5),
            });

        // Act
        var response = await this.client.PostAsJsonAsync("/admin/products", command);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(product);
        Assert.Equal("Test Product", product.Name);
        Assert.Equal("test-product", product.Slug);
        Assert.Equal(2, product.Variants.Count);
        Assert.Contains(product.Variants, v => v.Sku == "SKU-001");
        Assert.Contains(product.Variants, v => v.Sku == "SKU-002");
    }

    /// <summary>
    /// Test: Create product with duplicate slug returns 409.
    /// </summary>
    /// <returns>A task representing the async operation.</returns>
    [Fact]
    public async Task CreateProduct_WithDuplicateSlug_Returns409()
    {
        // Arrange
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var command1 = new CreateProductCommand(
            "Product 1",
            "duplicate-slug",
            null,
            null,
            null,
            null,
            null,
            true,
            new List<CreateProductVariantDto>
            {
                new("SKU-100", "Variant 1", 100.00m, 10),
            });

        await this.client.PostAsJsonAsync("/admin/products", command1);

        var command2 = new CreateProductCommand(
            "Product 2",
            "duplicate-slug",
            null,
            null,
            null,
            null,
            null,
            true,
            new List<CreateProductVariantDto>
            {
                new("SKU-200", "Variant 2", 150.00m, 5),
            });

        // Act
        var response = await this.client.PostAsJsonAsync("/admin/products", command2);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    /// <summary>
    /// Test: Create product with duplicate SKU returns 409.
    /// </summary>
    /// <returns>A task representing the async operation.</returns>
    [Fact]
    public async Task CreateProduct_WithDuplicateSku_Returns409()
    {
        // Arrange
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var command1 = new CreateProductCommand(
            "Product 1",
            "product-1",
            null,
            null,
            null,
            null,
            null,
            true,
            new List<CreateProductVariantDto>
            {
                new("DUPLICATE-SKU", "Variant 1", 100.00m, 10),
            });

        await this.client.PostAsJsonAsync("/admin/products", command1);

        var command2 = new CreateProductCommand(
            "Product 2",
            "product-2",
            null,
            null,
            null,
            null,
            null,
            true,
            new List<CreateProductVariantDto>
            {
                new("DUPLICATE-SKU", "Variant 2", 150.00m, 5),
            });

        // Act
        var response = await this.client.PostAsJsonAsync("/admin/products", command2);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    /// <summary>
    /// Test: Get products returns paginated list.
    /// </summary>
    /// <returns>A task representing the async operation.</returns>
    [Fact]
    public async Task GetProducts_ReturnsPaginatedList()
    {
        // Arrange
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Create a product first
        var command = new CreateProductCommand(
            "Test Product",
            "test-product-list",
            null,
            null,
            null,
            null,
            null,
            true,
            new List<CreateProductVariantDto>
            {
                new("SKU-LIST-001", "Variant 1", 100.00m, 10),
            });

        await this.client.PostAsJsonAsync("/admin/products", command);

        // Act
        var response = await this.client.GetAsync("/admin/products?page=1&pageSize=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();
        Assert.NotNull(result);
        Assert.True(result.TotalCount > 0);
        Assert.NotEmpty(result.Items);
    }

    /// <summary>
    /// Test: Get product by ID returns product.
    /// </summary>
    /// <returns>A task representing the async operation.</returns>
    [Fact]
    public async Task GetProductById_ReturnsProduct()
    {
        // Arrange
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var command = new CreateProductCommand(
            "Test Product",
            "test-product-get",
            null,
            null,
            null,
            null,
            null,
            true,
            new List<CreateProductVariantDto>
            {
                new("SKU-GET-001", "Variant 1", 100.00m, 10),
            });

        var createResponse = await this.client.PostAsJsonAsync("/admin/products", command);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        // Act
        var response = await this.client.GetAsync($"/admin/products/{createdProduct!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(product);
        Assert.Equal(createdProduct.Id, product.Id);
        Assert.Equal("Test Product", product.Name);
    }

    /// <summary>
    /// Test: Update product returns 200.
    /// </summary>
    /// <returns>A task representing the async operation.</returns>
    [Fact]
    public async Task UpdateProduct_Returns200()
    {
        // Arrange
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var command = new CreateProductCommand(
            "Test Product",
            "test-product-update",
            null,
            null,
            null,
            null,
            null,
            true,
            new List<CreateProductVariantDto>
            {
                new("SKU-UPDATE-001", "Variant 1", 100.00m, 10),
            });

        var createResponse = await this.client.PostAsJsonAsync("/admin/products", command);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        var updateCommand = new
        {
            Name = "Updated Product",
            Slug = "updated-product",
            Description = "Updated description",
            CategoryId = (Guid?)null,
            Images = (string?)null,
            Videos = (string?)null,
            Specifications = (string?)null,
            IsActive = true,
            Variants = new List<UpdateProductVariantDto>
            {
                new(createdProduct!.Variants[0].Id, "SKU-UPDATE-001", "Updated Variant 1", 120.00m, 15),
            },
        };

        // Act
        var response = await this.client.PutAsJsonAsync($"/admin/products/{createdProduct.Id}", updateCommand);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    /// <summary>
    /// Test: Delete product returns 204.
    /// </summary>
    /// <returns>A task representing the async operation.</returns>
    [Fact]
    public async Task DeleteProduct_Returns204()
    {
        // Arrange
        var token = await this.GetAdminTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var command = new CreateProductCommand(
            "Test Product",
            "test-product-delete",
            null,
            null,
            null,
            null,
            null,
            true,
            new List<CreateProductVariantDto>
            {
                new("SKU-DELETE-001", "Variant 1", 100.00m, 10),
            });

        var createResponse = await this.client.PostAsJsonAsync("/admin/products", command);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        // Act
        var response = await this.client.DeleteAsync($"/admin/products/{createdProduct!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify product was deleted
        var getResponse = await this.client.GetAsync($"/admin/products/{createdProduct.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    /// <summary>
    /// Disposes the test resources.
    /// </summary>
    public void Dispose()
    {
        this.client.Dispose();
        this.factory.Dispose();
    }

    private async Task<string> GetAdminTokenAsync()
    {
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var dateTime = scope.ServiceProvider.GetRequiredService<IDateTime>();

        // Use a unique email for each test to avoid collisions
        var uniqueEmail = $"admin-{Guid.NewGuid()}@test.com";

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
