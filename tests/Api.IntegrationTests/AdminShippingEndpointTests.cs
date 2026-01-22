// <copyright file="AdminShippingEndpointTests.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Application.Abstractions;
using Application.Features.AdminAuth.Login;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.IntegrationTests;

/// <summary>
/// Integration tests for admin shipping endpoints.
/// </summary>
public sealed class AdminShippingEndpointTests : IDisposable
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminShippingEndpointTests"/> class.
    /// </summary>
    public AdminShippingEndpointTests()
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

                    var dbName = $"AdminShippingTestDb_{Guid.NewGuid()}";
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
    /// Test that admin can update shipping configuration.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task UpdateShippingConfig_AsAdmin_ReturnsSuccess()
    {
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var admin = new StaffUser
        {
            Id = Guid.NewGuid(),
            Email = "admin@test.com",
            Phone = "0901234569",
            FullName = "Admin User",
            PasswordHash = passwordHasher.HashPassword("Admin123!"),
            Role = StaffRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        dbContext.StaffUsers.Add(admin);
        await dbContext.SaveChangesAsync();

        var loginResponse = await this.client.PostAsJsonAsync("/admin/auth/login", new AdminLoginCommand(admin.Email, "Admin123!"));
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AdminLoginResult>();
        Assert.NotNull(loginResult);

        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.AccessToken);

        var configRequest = new
        {
            Configs = new Dictionary<string, string>
            {
                { "max_distance_km", "100" },
                { "default_carrier", "GHN" },
            },
        };

        var response = await this.client.PutAsJsonAsync("/admin/shipping/config", configRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var configs = await dbContext.ShippingConfigs.ToListAsync();
        Assert.Equal(2, configs.Count);
        Assert.Contains(configs, c => c.Key == "max_distance_km" && c.Value == "100");
        Assert.Contains(configs, c => c.Key == "default_carrier" && c.Value == "GHN");
    }

    /// <summary>
    /// Test that non-admin cannot update shipping configuration.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task UpdateShippingConfig_Unauthorized_ReturnsForbidden()
    {
        var configRequest = new
        {
            Configs = new Dictionary<string, string>
            {
                { "max_distance_km", "100" },
            },
        };

        var response = await this.client.PutAsJsonAsync("/admin/shipping/config", configRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Dispose the test resources.
    /// </summary>
    public void Dispose()
    {
        this.client?.Dispose();
        this.factory?.Dispose();
    }
}
