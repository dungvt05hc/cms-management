// <copyright file="CategoriesEndpointTests.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Application.Abstractions;
using Application.Features.AdminAuth.Login;
using Application.Features.Categories.CreateCategory;
using Application.Features.Categories.GetCategoryTree;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.IntegrationTests;

/// <summary>
/// Integration tests for the Categories endpoints.
/// </summary>
public sealed class CategoriesEndpointTests : IDisposable
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="CategoriesEndpointTests"/> class.
    /// </summary>
    public CategoriesEndpointTests()
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
    /// Tests admin category CRUD flow and public tree endpoint.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task CategoriesFlow_AdminCRUD_PublicTree_ReturnsSuccess()
    {
        // Arrange: Bootstrap super admin manually for test
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

        // Act 1: Admin login
        var loginCommand = new AdminLoginCommand("admin@test.com", "Admin123!");
        var loginResponse = await this.client.PostAsJsonAsync("/admin/auth/login", loginCommand);

        loginResponse.EnsureSuccessStatusCode();
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AdminLoginResult>();
        Assert.NotNull(loginResult);
        Assert.NotNull(loginResult.AccessToken);

        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.AccessToken);

        // Act 2: Create root category
        var createRootCommand = new CreateCategoryCommand("Electronics", null);
        var createRootResponse = await this.client.PostAsJsonAsync("/admin/categories", createRootCommand);

        Assert.Equal(HttpStatusCode.Created, createRootResponse.StatusCode);
        var rootCategory = await createRootResponse.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(rootCategory);
        Assert.Equal("Electronics", rootCategory.Name);
        Assert.Null(rootCategory.ParentId);

        // Act 3: Create child category
        var createChildCommand = new CreateCategoryCommand("Laptops", rootCategory.Id);
        var createChildResponse = await this.client.PostAsJsonAsync("/admin/categories", createChildCommand);

        Assert.Equal(HttpStatusCode.Created, createChildResponse.StatusCode);
        var childCategory = await createChildResponse.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(childCategory);
        Assert.Equal("Laptops", childCategory.Name);
        Assert.Equal(rootCategory.Id, childCategory.ParentId);

        // Act 4: Create grandchild category
        var createGrandchildCommand = new CreateCategoryCommand("Gaming Laptops", childCategory.Id);
        var createGrandchildResponse = await this.client.PostAsJsonAsync("/admin/categories", createGrandchildCommand);

        Assert.Equal(HttpStatusCode.Created, createGrandchildResponse.StatusCode);
        var grandchildCategory = await createGrandchildResponse.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(grandchildCategory);
        Assert.Equal("Gaming Laptops", grandchildCategory.Name);
        Assert.Equal(childCategory.Id, grandchildCategory.ParentId);

        // Act 5: Get public category tree
        var treeResponse = await this.client.GetAsync("/categories/tree");

        Assert.Equal(HttpStatusCode.OK, treeResponse.StatusCode);
        var tree = await treeResponse.Content.ReadFromJsonAsync<List<CategoryTreeNode>>();
        Assert.NotNull(tree);
        Assert.Single(tree);
        Assert.Equal("Electronics", tree[0].Name);
        Assert.Single(tree[0].Children);
        Assert.Equal("Laptops", tree[0].Children[0].Name);
        Assert.Single(tree[0].Children[0].Children);
        Assert.Equal("Gaming Laptops", tree[0].Children[0].Children[0].Name);

        // Act 6: Update category
        var updateResponse = await this.client.PutAsJsonAsync(
            $"/admin/categories/{childCategory.Id}",
            new { Name = "Notebooks", ParentId = rootCategory.Id });

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        // Act 7: Delete leaf category
        var deleteResponse = await this.client.DeleteAsync($"/admin/categories/{grandchildCategory.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Act 8: Verify updated tree
        var updatedTreeResponse = await this.client.GetAsync("/categories/tree");
        var updatedTree = await updatedTreeResponse.Content.ReadFromJsonAsync<List<CategoryTreeNode>>();
        Assert.NotNull(updatedTree);
        Assert.Single(updatedTree);
        Assert.Equal("Notebooks", updatedTree[0].Children[0].Name);
        Assert.Empty(updatedTree[0].Children[0].Children);
    }

    /// <summary>
    /// Tests cycle detection when updating a category.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task UpdateCategory_CreatesCycle_ReturnsConflict()
    {
        // Arrange: Bootstrap super admin and login
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
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult!.AccessToken);

        // Act 1: Create parent -> child -> grandchild
        var parentResponse = await this.client.PostAsJsonAsync("/admin/categories", new CreateCategoryCommand("Parent", null));
        parentResponse.EnsureSuccessStatusCode();
        var parent = await parentResponse.Content.ReadFromJsonAsync<CategoryDto>();

        var childResponse = await this.client.PostAsJsonAsync("/admin/categories", new CreateCategoryCommand("Child", parent!.Id));
        childResponse.EnsureSuccessStatusCode();
        var child = await childResponse.Content.ReadFromJsonAsync<CategoryDto>();

        var grandchildResponse = await this.client.PostAsJsonAsync("/admin/categories", new CreateCategoryCommand("Grandchild", child!.Id));
        grandchildResponse.EnsureSuccessStatusCode();
        var grandchild = await grandchildResponse.Content.ReadFromJsonAsync<CategoryDto>();

        // Act 2: Try to set parent's parent to grandchild (creates cycle)
        var updateResponse = await this.client.PutAsJsonAsync(
            $"/admin/categories/{parent.Id}",
            new { Name = "Parent", ParentId = grandchild!.Id });

        // Assert: Should return conflict due to cycle detection
        Assert.Equal(HttpStatusCode.Conflict, updateResponse.StatusCode);
    }

    /// <summary>
    /// Tests that unauthorized users cannot create categories.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task CreateCategory_Unauthorized_ReturnsUnauthorized()
    {
        // Act: Try to create category without token
        var createCommand = new CreateCategoryCommand("Unauthorized", null);
        var response = await this.client.PostAsJsonAsync("/admin/categories", createCommand);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Tests that category tree is accessible anonymously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetCategoryTree_Anonymous_ReturnsSuccess()
    {
        // Act: Get category tree without authentication
        var response = await this.client.GetAsync("/categories/tree");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tree = await response.Content.ReadFromJsonAsync<List<CategoryTreeNode>>();
        Assert.NotNull(tree);
    }

    /// <summary>
    /// Disposes the test resources.
    /// </summary>
    public void Dispose()
    {
        this.client.Dispose();
        this.factory.Dispose();
    }
}
