// <copyright file="NotificationsEndpointTests.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.IntegrationTests;

/// <summary>
/// Integration tests for the Notifications endpoints.
/// </summary>
public sealed class NotificationsEndpointTests : IDisposable
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationsEndpointTests"/> class.
    /// </summary>
    public NotificationsEndpointTests()
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
    /// Test getting notifications with "all" filter.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetNotifications_WithAllFilter_ReturnsAllNotifications()
    {
        // Arrange
        var userId = await this.CreateTestUserAsync();
        var token = await this.LoginTestUserAsync(userId);

        await this.SeedNotificationsAsync(userId);

        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await this.client.GetAsync("/me/notifications?type=all");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var notifications = await response.Content.ReadFromJsonAsync<List<object>>();
        Assert.NotNull(notifications);
        Assert.Equal(4, notifications.Count);
    }

    /// <summary>
    /// Test getting notifications with "promotions" filter.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetNotifications_WithPromotionsFilter_ReturnsPromotionNotificationsOnly()
    {
        // Arrange
        var userId = await this.CreateTestUserAsync();
        var token = await this.LoginTestUserAsync(userId);

        await this.SeedNotificationsAsync(userId);

        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await this.client.GetAsync("/me/notifications?type=promotions");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var notifications = await response.Content.ReadFromJsonAsync<List<object>>();
        Assert.NotNull(notifications);
        Assert.Single(notifications);
    }

    /// <summary>
    /// Test getting notifications with "orders" filter.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetNotifications_WithOrdersFilter_ReturnsOrderNotificationsOnly()
    {
        // Arrange
        var userId = await this.CreateTestUserAsync();
        var token = await this.LoginTestUserAsync(userId);

        await this.SeedNotificationsAsync(userId);

        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await this.client.GetAsync("/me/notifications?type=orders");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var notifications = await response.Content.ReadFromJsonAsync<List<object>>();
        Assert.NotNull(notifications);
        Assert.Equal(2, notifications.Count);
    }

    /// <summary>
    /// Test getting notifications without authentication.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetNotifications_Unauthenticated_Returns401()
    {
        // Act
        var response = await this.client.GetAsync("/me/notifications");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Disposes the test instance.
    /// </summary>
    public void Dispose()
    {
        this.client.Dispose();
        this.factory.Dispose();
    }

    private async Task<Guid> CreateTestUserAsync()
    {
        using var scope = this.factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<Application.Abstractions.IPasswordHasher>();
        var dateTime = scope.ServiceProvider.GetRequiredService<Application.Abstractions.IDateTime>();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Phone = $"+8490{Random.Shared.Next(1000000, 9999999)}",
            Email = $"test-notif-{Guid.NewGuid()}@example.com",
            FullName = "Test User",
            PasswordHash = passwordHasher.HashPassword("Test123!"),
            IsVerified = true,
            CreatedAt = dateTime.UtcNow,
            UpdatedAt = dateTime.UtcNow,
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user.Id;
    }

    private async Task<string> LoginTestUserAsync(Guid userId)
    {
        using var scope = this.factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await context.Users.FindAsync(userId);

        var loginRequest = new
        {
            PhoneOrEmail = user!.Phone,
            Password = "Test123!",
        };

        var response = await this.client.PostAsJsonAsync("/auth/login", loginRequest);
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

        return result!["accessToken"].ToString()!;
    }

    private async Task SeedNotificationsAsync(Guid userId)
    {
        using var scope = this.factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var notifications = new List<Notification>
        {
            new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = NotificationType.Promotion,
                Title = "Big Sale!",
                Message = "50% off all items",
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddHours(-1),
            },
            new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = NotificationType.Order,
                Title = "Order Shipped",
                Message = "Your order has been shipped",
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddHours(-2),
            },
            new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = NotificationType.Order,
                Title = "Order Delivered",
                Message = "Your order has been delivered",
                IsRead = true,
                CreatedAt = DateTime.UtcNow.AddHours(-3),
            },
            new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = NotificationType.System,
                Title = "Maintenance Notice",
                Message = "System maintenance scheduled",
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddHours(-4),
            },
        };

        context.Notifications.AddRange(notifications);
        await context.SaveChangesAsync();
    }
}
