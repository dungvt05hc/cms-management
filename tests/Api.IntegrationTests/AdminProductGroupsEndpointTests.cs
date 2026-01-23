// <copyright file="AdminProductGroupsEndpointTests.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Application.Abstractions;
using Application.Features.AdminAuth.Login;
using Application.Features.Categories.CreateCategory;
using Application.Features.ProductGroups.CreateProductGroup;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.IntegrationTests;

/// <summary>
/// Integration tests for the Admin Product Groups endpoints.
/// </summary>
public sealed class AdminProductGroupsEndpointTests : IDisposable
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminProductGroupsEndpointTests"/> class.
    /// </summary>
    public AdminProductGroupsEndpointTests()
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
    /// Tests creating a product group with valid category returns success.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task CreateProductGroup_WithValidCategory_ReturnsCreated()
    {
        // Arrange: Bootstrap super admin and create category
        var (token, categoryId) = await this.SetupAdminAndCategory();

        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var command = new CreateProductGroupCommand(
            categoryId,
            "Laptops",
            new List<ProductGroupAttributeDto>
            {
                new ProductGroupAttributeDto("Screen Size", "screen-size", "text"),
                new ProductGroupAttributeDto("RAM", "ram", "number"),
            });

        // Act
        var response = await this.client.PostAsJsonAsync("/admin/product-groups", command);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ProductGroupDto>();
        Assert.NotNull(result);
        Assert.Equal("Laptops", result.Name);
        Assert.Equal(categoryId, result.CategoryId);
        Assert.Equal(2, result.Attributes.Count);
        Assert.Contains(result.Attributes, a => a.Key == "screen-size");
        Assert.Contains(result.Attributes, a => a.Key == "ram");
    }

    /// <summary>
    /// Tests creating a product group with invalid category returns bad request.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task CreateProductGroup_WithInvalidCategory_ReturnsBadRequest()
    {
        // Arrange: Bootstrap super admin only
        var token = await this.SetupAdmin();

        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var command = new CreateProductGroupCommand(
            Guid.NewGuid(),
            "Laptops",
            new List<ProductGroupAttributeDto>
            {
                new ProductGroupAttributeDto("Screen Size", "screen-size", "text"),
            });

        // Act
        var response = await this.client.PostAsJsonAsync("/admin/product-groups", command);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// Tests getting product groups filtered by category.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetProductGroups_FilteredByCategory_ReturnsCorrectGroups()
    {
        // Arrange: Setup admin and create categories + product groups
        var token = await this.SetupAdmin();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Create two categories
        var categoryResponse1 = await this.client.PostAsJsonAsync("/admin/categories", new CreateCategoryCommand("Electronics", null));
        var category1 = await categoryResponse1.Content.ReadFromJsonAsync<CategoryDto>();

        var categoryResponse2 = await this.client.PostAsJsonAsync("/admin/categories", new CreateCategoryCommand("Books", null));
        var category2 = await categoryResponse2.Content.ReadFromJsonAsync<CategoryDto>();

        // Create product groups for each category
        await this.client.PostAsJsonAsync("/admin/product-groups", new CreateProductGroupCommand(
            category1!.Id,
            "Laptops",
            new List<ProductGroupAttributeDto> { new ProductGroupAttributeDto("RAM", "ram", "number") }));

        await this.client.PostAsJsonAsync("/admin/product-groups", new CreateProductGroupCommand(
            category2!.Id,
            "Fiction",
            new List<ProductGroupAttributeDto> { new ProductGroupAttributeDto("Author", "author", "text") }));

        // Act
        var response = await this.client.GetAsync($"/admin/product-groups?categoryId={category1.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<List<ProductGroupDto>>();
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Laptops", result[0].Name);
        Assert.Equal(category1.Id, result[0].CategoryId);
    }

    /// <summary>
    /// Tests updating a product group.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task UpdateProductGroup_WithValidData_ReturnsSuccess()
    {
        // Arrange: Setup admin and create category + product group
        var (token, categoryId) = await this.SetupAdminAndCategory();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createCommand = new CreateProductGroupCommand(
            categoryId,
            "Laptops",
            new List<ProductGroupAttributeDto> { new ProductGroupAttributeDto("RAM", "ram", "number") });

        var createResponse = await this.client.PostAsJsonAsync("/admin/product-groups", createCommand);
        var created = await createResponse.Content.ReadFromJsonAsync<ProductGroupDto>();

        // Act
        var updateRequest = new
        {
            CategoryId = categoryId,
            Name = "Gaming Laptops",
            Attributes = new List<ProductGroupAttributeDto>
            {
                new ProductGroupAttributeDto("GPU", "gpu", "text"),
                new ProductGroupAttributeDto("RAM", "ram", "number"),
            },
        };

        var updateResponse = await this.client.PutAsJsonAsync($"/admin/product-groups/{created!.Id}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        // Verify the update
        var getResponse = await this.client.GetAsync($"/admin/product-groups?categoryId={categoryId}");
        var result = await getResponse.Content.ReadFromJsonAsync<List<ProductGroupDto>>();
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Gaming Laptops", result[0].Name);
        Assert.Equal(2, result[0].Attributes.Count);
    }

    /// <summary>
    /// Tests deleting a product group.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task DeleteProductGroup_ExistingGroup_ReturnsNoContent()
    {
        // Arrange: Setup admin and create category + product group
        var (token, categoryId) = await this.SetupAdminAndCategory();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createCommand = new CreateProductGroupCommand(
            categoryId,
            "Laptops",
            new List<ProductGroupAttributeDto> { new ProductGroupAttributeDto("RAM", "ram", "number") });

        var createResponse = await this.client.PostAsJsonAsync("/admin/product-groups", createCommand);
        var created = await createResponse.Content.ReadFromJsonAsync<ProductGroupDto>();

        // Act
        var deleteResponse = await this.client.DeleteAsync($"/admin/product-groups/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Verify deletion
        var getResponse = await this.client.GetAsync($"/admin/product-groups?categoryId={categoryId}");
        var result = await getResponse.Content.ReadFromJsonAsync<List<ProductGroupDto>>();
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    /// <summary>
    /// Tests creating a product group with duplicate attribute keys returns bad request.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task CreateProductGroup_WithDuplicateKeys_ReturnsBadRequest()
    {
        // Arrange
        var (token, categoryId) = await this.SetupAdminAndCategory();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var command = new CreateProductGroupCommand(
            categoryId,
            "Laptops",
            new List<ProductGroupAttributeDto>
            {
                new ProductGroupAttributeDto("Screen Size", "screen", "text"),
                new ProductGroupAttributeDto("Screen Resolution", "screen", "text"),
            });

        // Act
        var response = await this.client.PostAsJsonAsync("/admin/product-groups", command);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// Tests that unauthorized users cannot create product groups.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task CreateProductGroup_Unauthorized_ReturnsUnauthorized()
    {
        // Act
        var command = new CreateProductGroupCommand(
            Guid.NewGuid(),
            "Laptops",
            new List<ProductGroupAttributeDto> { new ProductGroupAttributeDto("RAM", "ram", "number") });

        var response = await this.client.PostAsJsonAsync("/admin/product-groups", command);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Disposes the test resources.
    /// </summary>
    public void Dispose()
    {
        this.client.Dispose();
        this.factory.Dispose();
    }

    private async Task<string> SetupAdmin()
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
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AdminLoginResult>();

        return loginResult!.AccessToken;
    }

    private async Task<(string Token, Guid CategoryId)> SetupAdminAndCategory()
    {
        var token = await this.SetupAdmin();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var categoryCommand = new CreateCategoryCommand("Electronics", null);
        var categoryResponse = await this.client.PostAsJsonAsync("/admin/categories", categoryCommand);
        var category = await categoryResponse.Content.ReadFromJsonAsync<CategoryDto>();

        return (token, category!.Id);
    }
}
