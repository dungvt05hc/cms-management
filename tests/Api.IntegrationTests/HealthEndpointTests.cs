// <copyright file="HealthEndpointTests.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Http.Json;

using Application.Health;

using Microsoft.AspNetCore.Mvc.Testing;

namespace Api.IntegrationTests;

/// <summary>
/// Integration tests for the Health endpoint.
/// </summary>
public sealed class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="HealthEndpointTests"/> class.
    /// </summary>
    /// <param name="factory">The web application factory.</param>
    public HealthEndpointTests(WebApplicationFactory<Program> factory)
    {
        this.factory = factory;
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
}
