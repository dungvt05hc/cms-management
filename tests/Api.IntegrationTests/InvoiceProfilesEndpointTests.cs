// <copyright file="InvoiceProfilesEndpointTests.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Application.Abstractions;
using Application.Features.Auth.Login;
using Application.Features.InvoiceProfiles;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.IntegrationTests;

/// <summary>
/// Integration tests for invoice profiles endpoints.
/// </summary>
public sealed class InvoiceProfilesEndpointTests : IDisposable
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="InvoiceProfilesEndpointTests"/> class.
    /// </summary>
    public InvoiceProfilesEndpointTests()
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

                    var dbName = $"InvoiceProfilesTestDb_{Guid.NewGuid()}";
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
    /// Test that authenticated users can get their invoice profiles list (empty initially).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetInvoiceProfiles_WithAuth_ReturnsEmptyList()
    {
        var token = await this.RegisterAndLoginAsync("0987654321", "password123", "Test User");
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await this.client.GetAsync("/me/invoice-profiles");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var profiles = await response.Content.ReadFromJsonAsync<List<InvoiceProfileDto>>();
        Assert.NotNull(profiles);
        Assert.Empty(profiles);
    }

    /// <summary>
    /// Test that invoice profiles endpoint requires authentication.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetInvoiceProfiles_WithoutAuth_ReturnsUnauthorized()
    {
        var response = await this.client.GetAsync("/me/invoice-profiles");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Test creating an invoice profile with valid data.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task CreateInvoiceProfile_ValidData_ReturnsCreated()
    {
        var token = await this.RegisterAndLoginAsync("0987654322", "password123", "Test User");
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new
        {
            TaxCode = "0123456789",
            CompanyName = "Test Company Ltd",
            CompanyAddress = "123 Business St, District 1, HCMC",
            Email = "invoice@testcompany.com",
        };

        var response = await this.client.PostAsJsonAsync("/me/invoice-profiles", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var profile = await response.Content.ReadFromJsonAsync<InvoiceProfileDto>();
        Assert.NotNull(profile);
        Assert.Equal(request.TaxCode, profile.TaxCode);
        Assert.Equal(request.CompanyName, profile.CompanyName);
        Assert.Equal(request.CompanyAddress, profile.CompanyAddress);
        Assert.Equal(request.Email, profile.Email);
    }

    /// <summary>
    /// Test creating an invoice profile with invalid data (missing tax code).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task CreateInvoiceProfile_MissingTaxCode_ReturnsBadRequest()
    {
        var token = await this.RegisterAndLoginAsync("0987654323", "password123", "Test User");
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new
        {
            TaxCode = string.Empty,
            CompanyName = "Test Company Ltd",
            CompanyAddress = "123 Business St, District 1, HCMC",
            Email = "invoice@testcompany.com",
        };

        var response = await this.client.PostAsJsonAsync("/me/invoice-profiles", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// Test creating an invoice profile with invalid email.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task CreateInvoiceProfile_InvalidEmail_ReturnsBadRequest()
    {
        var token = await this.RegisterAndLoginAsync("0987654324", "password123", "Test User");
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new
        {
            TaxCode = "0123456789",
            CompanyName = "Test Company Ltd",
            CompanyAddress = "123 Business St, District 1, HCMC",
            Email = "invalid-email",
        };

        var response = await this.client.PostAsJsonAsync("/me/invoice-profiles", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// Test that created profile appears in the list.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task CreateInvoiceProfile_ThenGetList_ReturnsProfile()
    {
        var token = await this.RegisterAndLoginAsync("0987654325", "password123", "Test User");
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new
        {
            TaxCode = "0123456789",
            CompanyName = "Test Company Ltd",
            CompanyAddress = "123 Business St, District 1, HCMC",
            Email = "invoice@testcompany.com",
        };

        await this.client.PostAsJsonAsync("/me/invoice-profiles", request);

        var response = await this.client.GetAsync("/me/invoice-profiles");
        var profiles = await response.Content.ReadFromJsonAsync<List<InvoiceProfileDto>>();

        Assert.NotNull(profiles);
        Assert.Single(profiles);
        Assert.Equal(request.TaxCode, profiles[0].TaxCode);
    }

    /// <summary>
    /// Disposes resources.
    /// </summary>
    public void Dispose()
    {
        this.client?.Dispose();
        this.factory?.Dispose();
    }

    private async Task<string> RegisterAndLoginAsync(string phone, string password, string fullName)
    {
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<Application.Abstractions.IPasswordHasher>();
        var dateTime = scope.ServiceProvider.GetRequiredService<Application.Abstractions.IDateTime>();

        var user = new Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            Phone = phone,
            Email = $"test-{Guid.NewGuid()}@example.com",
            FullName = fullName,
            PasswordHash = passwordHasher.HashPassword(password),
            IsVerified = true,
            CreatedAt = dateTime.UtcNow,
            UpdatedAt = dateTime.UtcNow,
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var loginRequest = new
        {
            PhoneOrEmail = phone,
            Password = password,
        };

        var loginResponse = await this.client.PostAsJsonAsync("/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResult>();

        return loginResult?.AccessToken ?? throw new InvalidOperationException("Failed to get token");
    }
}
