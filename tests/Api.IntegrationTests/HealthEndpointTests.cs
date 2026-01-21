// <copyright file="HealthEndpointTests.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Http.Json;

using Application.Abstractions;
using Application.Health;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.IntegrationTests;

/// <summary>
/// Integration tests for the Health endpoint.
/// </summary>
public sealed class HealthEndpointTests : IDisposable
{
    private readonly WebApplicationFactory<Program> factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="HealthEndpointTests"/> class.
    /// </summary>
    public HealthEndpointTests()
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
    }

    /// <summary>
    /// Tests that GET /health returns HTTP 200 OK.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetHealth_ReturnsOkStatus()
    {
        // Arrange
        var client = this.factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    /// <summary>
    /// Tests that GET /health returns a valid health response with correct structure.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetHealth_ReturnsValidHealthResponse()
    {
        // Arrange
        var client = this.factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");
        var healthResponse = await response.Content.ReadFromJsonAsync<HealthResponse>();

        // Assert
        Assert.NotNull(healthResponse);
        Assert.Equal("ok", healthResponse.Status);
        Assert.NotEqual(default, healthResponse.UtcNow);
        Assert.Equal(DateTimeKind.Utc, healthResponse.UtcNow.Kind);
    }

    /// <summary>
    /// Tests that GET /health returns a timestamp close to the current time.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetHealth_ReturnsRecentTimestamp()
    {
        // Arrange
        var client = this.factory.CreateClient();
        var beforeRequest = DateTime.UtcNow;

        // Act
        var response = await client.GetAsync("/health");
        var afterRequest = DateTime.UtcNow;
        var healthResponse = await response.Content.ReadFromJsonAsync<HealthResponse>();

        // Assert
        Assert.NotNull(healthResponse);
        Assert.True(healthResponse.UtcNow >= beforeRequest);
        Assert.True(healthResponse.UtcNow <= afterRequest);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.factory?.Dispose();
    }
}
