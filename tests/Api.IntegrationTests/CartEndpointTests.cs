// <copyright file="CartEndpointTests.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Application.Abstractions;
using Application.Features.Auth.Login;
using Application.Features.Cart;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.IntegrationTests;

/// <summary>
/// Integration tests for cart endpoints.
/// </summary>
public sealed class CartEndpointTests : IDisposable
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="CartEndpointTests"/> class.
    /// </summary>
    public CartEndpointTests()
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

                    var dbName = $"CartTestDb_{Guid.NewGuid()}";
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
    /// Test that authenticated users can get their cart.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetCart_WithAuth_ReturnsEmptyCart()
    {
        var token = await this.GetAuthTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await this.client.GetAsync("/cart");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var cart = await response.Content.ReadFromJsonAsync<CartDto>();
        Assert.NotNull(cart);
        Assert.Empty(cart.Items);
        Assert.Equal(0m, cart.Subtotal);
    }

    /// <summary>
    /// Test that unauthenticated requests return 401.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetCart_WithoutAuth_Returns401()
    {
        var response = await this.client.GetAsync("/cart");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Test that users can add items to their cart.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task AddCartItem_ValidProduct_ReturnsCreated()
    {
        var token = await this.GetAuthTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var product = await this.CreateTestProductAsync();

        var request = new { productId = product.Id, variantId = (Guid?)null, quantity = 2 };
        var response = await this.client.PostAsJsonAsync("/cart/items", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var cartItem = await response.Content.ReadFromJsonAsync<CartItemDto>();
        Assert.NotNull(cartItem);
        Assert.Equal(product.Id, cartItem.ProductId);
        Assert.Equal(2, cartItem.Quantity);
        Assert.True(cartItem.Selected);
    }

    /// <summary>
    /// Test that adding items to cart without auth returns 401.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task AddCartItem_WithoutAuth_Returns401()
    {
        var request = new { productId = Guid.NewGuid(), variantId = (Guid?)null, quantity = 1 };
        var response = await this.client.PostAsJsonAsync("/cart/items", request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Test that users can update cart item quantity and selection.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task UpdateCartItem_ValidItem_ReturnsUpdated()
    {
        var token = await this.GetAuthTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var product = await this.CreateTestProductAsync();
        var addRequest = new { productId = product.Id, variantId = (Guid?)null, quantity = 2 };
        var addResponse = await this.client.PostAsJsonAsync("/cart/items", addRequest);
        var cartItem = await addResponse.Content.ReadFromJsonAsync<CartItemDto>();
        Assert.NotNull(cartItem);

        var updateRequest = new { quantity = 5, variantId = (Guid?)null, selected = false };
        var response = await this.client.PatchAsJsonAsync($"/cart/items/{cartItem.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<CartItemDto>();
        Assert.NotNull(updated);
        Assert.Equal(5, updated.Quantity);
        Assert.False(updated.Selected);
    }

    /// <summary>
    /// Test that users can delete cart items.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task DeleteCartItem_ValidItem_ReturnsNoContent()
    {
        var token = await this.GetAuthTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var product = await this.CreateTestProductAsync();
        var addRequest = new { productId = product.Id, variantId = (Guid?)null, quantity = 2 };
        var addResponse = await this.client.PostAsJsonAsync("/cart/items", addRequest);
        var cartItem = await addResponse.Content.ReadFromJsonAsync<CartItemDto>();
        Assert.NotNull(cartItem);

        var response = await this.client.DeleteAsync($"/cart/items/{cartItem.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getCart = await this.client.GetAsync("/cart");
        var cart = await getCart.Content.ReadFromJsonAsync<CartDto>();
        Assert.NotNull(cart);
        Assert.Empty(cart.Items);
    }

    /// <summary>
    /// Test that subtotal updates correctly based on selection.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetCart_MultipleItems_CalculatesSubtotalCorrectly()
    {
        var token = await this.GetAuthTokenAsync();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var product1 = await this.CreateTestProductAsync(price: 100m);
        var product2 = await this.CreateTestProductAsync(price: 50m);

        await this.client.PostAsJsonAsync("/cart/items", new { productId = product1.Id, variantId = (Guid?)null, quantity = 2 });
        var add2Response = await this.client.PostAsJsonAsync("/cart/items", new { productId = product2.Id, variantId = (Guid?)null, quantity = 3 });
        var item2 = await add2Response.Content.ReadFromJsonAsync<CartItemDto>();

        var cartResponse = await this.client.GetAsync("/cart");
        var cart = await cartResponse.Content.ReadFromJsonAsync<CartDto>();
        Assert.NotNull(cart);
        Assert.Equal(2, cart.Items.Count);
        Assert.Equal(350m, cart.Subtotal);

        await this.client.PatchAsJsonAsync($"/cart/items/{item2!.Id}", new { quantity = (int?)null, variantId = (Guid?)null, selected = false });

        cartResponse = await this.client.GetAsync("/cart");
        cart = await cartResponse.Content.ReadFromJsonAsync<CartDto>();
        Assert.NotNull(cart);
        Assert.Equal(200m, cart.Subtotal);
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

        var user = new Domain.Entities.User
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

    private async Task<TestProduct> CreateTestProductAsync(decimal price = 100m)
    {
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var product = new Domain.Entities.Product
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

        var variant = new Domain.Entities.ProductVariant
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

    private record TestProduct(Guid Id, string Name, string Slug, Guid VariantId, decimal Price);
}
