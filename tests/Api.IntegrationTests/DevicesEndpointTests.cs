// <copyright file="DevicesEndpointTests.cs" company="CMS Management">
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
/// Integration tests for the Devices endpoints.
/// </summary>
public sealed class DevicesEndpointTests : IDisposable
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="DevicesEndpointTests"/> class.
    /// </summary>
    public DevicesEndpointTests()
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
    /// Test registering a device token.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task RegisterDevice_ValidToken_ReturnsCreated()
    {
        // Arrange
        var userId = await this.CreateTestUserAsync();
        var token = await this.LoginTestUserAsync(userId);

        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new
        {
            Token = "test-fcm-token-12345",
            DeviceType = "web",
        };

        // Act
        var response = await this.client.PostAsJsonAsync("/me/devices", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        Assert.NotNull(result);
        Assert.True(result.ContainsKey("id"));
    }

    /// <summary>
    /// Test registering duplicate device token (deduplication).
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task RegisterDevice_DuplicateToken_ReturnsExistingDevice()
    {
        // Arrange
        var userId = await this.CreateTestUserAsync();
        var token = await this.LoginTestUserAsync(userId);

        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new
        {
            Token = "test-fcm-token-duplicate",
            DeviceType = "web",
        };

        // Act 1: Register device
        var response1 = await this.client.PostAsJsonAsync("/me/devices", request);
        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);
        var result1 = await response1.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var deviceId1 = result1!["id"].ToString();

        // Act 2: Register same device again
        var response2 = await this.client.PostAsJsonAsync("/me/devices", request);
        Assert.Equal(HttpStatusCode.Created, response2.StatusCode);
        var result2 = await response2.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var deviceId2 = result2!["id"].ToString();

        // Assert: Should return same device ID (deduplication)
        Assert.Equal(deviceId1, deviceId2);
    }

    /// <summary>
    /// Test getting device tokens list.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetDevices_ReturnsListWithMaskedTokens()
    {
        // Arrange
        var userId = await this.CreateTestUserAsync();
        var token = await this.LoginTestUserAsync(userId);

        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Register a device first
        var request = new
        {
            Token = "test-fcm-token-12345678",
            DeviceType = "web",
        };

        await this.client.PostAsJsonAsync("/me/devices", request);

        // Act
        var response = await this.client.GetAsync("/me/devices");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var devices = await response.Content.ReadFromJsonAsync<List<Dictionary<string, object>>>();
        Assert.NotNull(devices);
        Assert.Single(devices);

        // Verify token is masked
        var deviceToken = devices[0]["token"].ToString();
        Assert.StartsWith("test", deviceToken);
        Assert.Contains("...", deviceToken);
    }

    /// <summary>
    /// Test deleting a device token.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task DeleteDevice_ValidId_ReturnsNoContent()
    {
        // Arrange
        var userId = await this.CreateTestUserAsync();
        var token = await this.LoginTestUserAsync(userId);

        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Register a device first
        var request = new
        {
            Token = "test-fcm-token-to-delete",
            DeviceType = "web",
        };

        var registerResponse = await this.client.PostAsJsonAsync("/me/devices", request);
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        var deviceId = registerResult!["id"].ToString();

        // Act
        var response = await this.client.DeleteAsync($"/me/devices/{deviceId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify device is deleted
        var getResponse = await this.client.GetAsync("/me/devices");
        var devices = await getResponse.Content.ReadFromJsonAsync<List<object>>();
        Assert.Empty(devices!);
    }

    /// <summary>
    /// Test registering device without authentication.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task RegisterDevice_Unauthenticated_Returns401()
    {
        // Arrange
        var request = new
        {
            Token = "test-token",
        };

        // Act
        var response = await this.client.PostAsJsonAsync("/me/devices", request);

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
            Email = $"test-device-{Guid.NewGuid()}@example.com",
            FullName = "Test Device User",
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
}
