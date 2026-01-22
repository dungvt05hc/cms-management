// <copyright file="AddressesEndpointTests.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Application.Abstractions;
using Application.Features.Addresses;
using Application.Features.Auth.Login;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.IntegrationTests;

/// <summary>
/// Integration tests for addresses endpoints.
/// </summary>
public sealed class AddressesEndpointTests : IDisposable
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddressesEndpointTests"/> class.
    /// </summary>
    public AddressesEndpointTests()
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

                    var dbName = $"AddressesTestDb_{Guid.NewGuid()}";
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
    /// Test that authenticated users can get their addresses list (empty initially).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetAddresses_WithAuth_ReturnsEmptyList()
    {
        var token = await this.RegisterAndLoginAsync("0987654321", "password123", "Test User");
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await this.client.GetAsync("/me/addresses");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var addresses = await response.Content.ReadFromJsonAsync<List<AddressDto>>();
        Assert.NotNull(addresses);
        Assert.Empty(addresses);
    }

    /// <summary>
    /// Test that addresses endpoint requires authentication.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetAddresses_WithoutAuth_ReturnsUnauthorized()
    {
        var response = await this.client.GetAsync("/me/addresses");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Test creating an address.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task CreateAddress_ValidRequest_ReturnsCreated()
    {
        var token = await this.RegisterAndLoginAsync("0987654322", "password123", "Test User");
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new
        {
            FullName = "John Doe",
            Phone = "0987654321",
            AddressLine = "123 Main St",
            Ward = "Ward 1",
            District = "District 1",
            City = "Ho Chi Minh City",
            IsDefault = true,
        };

        var response = await this.client.PostAsJsonAsync("/me/addresses", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var address = await response.Content.ReadFromJsonAsync<AddressDto>();
        Assert.NotNull(address);
        Assert.Equal("John Doe", address.FullName);
        Assert.Equal("0987654321", address.Phone);
        Assert.True(address.IsDefault);
    }

    /// <summary>
    /// Test creating more than 5 addresses fails.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task CreateAddress_MoreThanFive_ReturnsBadRequest()
    {
        var token = await this.RegisterAndLoginAsync("0987654323", "password123", "Test User");
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        for (int i = 0; i < 5; i++)
        {
            var request = new
            {
                FullName = $"User {i}",
                Phone = $"098765432{i}",
                AddressLine = $"{i} Main St",
                Ward = "Ward 1",
                District = "District 1",
                City = "Ho Chi Minh City",
                IsDefault = i == 0,
            };

            var response = await this.client.PostAsJsonAsync("/me/addresses", request);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        var sixthRequest = new
        {
            FullName = "User 6",
            Phone = "0987654326",
            AddressLine = "6 Main St",
            Ward = "Ward 1",
            District = "District 1",
            City = "Ho Chi Minh City",
            IsDefault = false,
        };

        var sixthResponse = await this.client.PostAsJsonAsync("/me/addresses", sixthRequest);
        Assert.Equal(HttpStatusCode.BadRequest, sixthResponse.StatusCode);
    }

    /// <summary>
    /// Test updating an address.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task UpdateAddress_ValidRequest_ReturnsOk()
    {
        var token = await this.RegisterAndLoginAsync("0987654324", "password123", "Test User");
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createRequest = new
        {
            FullName = "John Doe",
            Phone = "0987654321",
            AddressLine = "123 Main St",
            Ward = "Ward 1",
            District = "District 1",
            City = "Ho Chi Minh City",
            IsDefault = true,
        };

        var createResponse = await this.client.PostAsJsonAsync("/me/addresses", createRequest);
        var createdAddress = await createResponse.Content.ReadFromJsonAsync<AddressDto>();
        Assert.NotNull(createdAddress);

        var updateRequest = new
        {
            FullName = "Jane Doe",
            Phone = "0987654322",
            AddressLine = "456 New St",
            Ward = "Ward 2",
            District = "District 2",
            City = "Hanoi",
        };

        var updateResponse = await this.client.PutAsJsonAsync($"/me/addresses/{createdAddress.Id}", updateRequest);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updatedAddress = await updateResponse.Content.ReadFromJsonAsync<AddressDto>();
        Assert.NotNull(updatedAddress);
        Assert.Equal("Jane Doe", updatedAddress.FullName);
        Assert.Equal("456 New St", updatedAddress.AddressLine);
    }

    /// <summary>
    /// Test deleting an address.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task DeleteAddress_ValidRequest_ReturnsNoContent()
    {
        var token = await this.RegisterAndLoginAsync("0987654325", "password123", "Test User");
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createRequest = new
        {
            FullName = "John Doe",
            Phone = "0987654321",
            AddressLine = "123 Main St",
            Ward = "Ward 1",
            District = "District 1",
            City = "Ho Chi Minh City",
            IsDefault = true,
        };

        var createResponse = await this.client.PostAsJsonAsync("/me/addresses", createRequest);
        var createdAddress = await createResponse.Content.ReadFromJsonAsync<AddressDto>();
        Assert.NotNull(createdAddress);

        var deleteResponse = await this.client.DeleteAsync($"/me/addresses/{createdAddress.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await this.client.GetAsync("/me/addresses");
        var addresses = await getResponse.Content.ReadFromJsonAsync<List<AddressDto>>();
        Assert.NotNull(addresses);
        Assert.Empty(addresses);
    }

    /// <summary>
    /// Test setting default address.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task SetDefaultAddress_ValidRequest_ReturnsOk()
    {
        var token = await this.RegisterAndLoginAsync("0987654326", "password123", "Test User");
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request1 = new
        {
            FullName = "Address 1",
            Phone = "0987654321",
            AddressLine = "123 Main St",
            Ward = "Ward 1",
            District = "District 1",
            City = "Ho Chi Minh City",
            IsDefault = true,
        };

        var response1 = await this.client.PostAsJsonAsync("/me/addresses", request1);
        var address1 = await response1.Content.ReadFromJsonAsync<AddressDto>();
        Assert.NotNull(address1);

        var request2 = new
        {
            FullName = "Address 2",
            Phone = "0987654322",
            AddressLine = "456 Second St",
            Ward = "Ward 2",
            District = "District 2",
            City = "Hanoi",
            IsDefault = false,
        };

        var response2 = await this.client.PostAsJsonAsync("/me/addresses", request2);
        var address2 = await response2.Content.ReadFromJsonAsync<AddressDto>();
        Assert.NotNull(address2);

        var setDefaultResponse = await this.client.PutAsync($"/me/addresses/{address2.Id}/default", null);

        Assert.Equal(HttpStatusCode.OK, setDefaultResponse.StatusCode);
        var updatedAddress = await setDefaultResponse.Content.ReadFromJsonAsync<AddressDto>();
        Assert.NotNull(updatedAddress);
        Assert.True(updatedAddress.IsDefault);

        var listResponse = await this.client.GetAsync("/me/addresses");
        var addresses = await listResponse.Content.ReadFromJsonAsync<List<AddressDto>>();
        Assert.NotNull(addresses);
        Assert.Equal(2, addresses.Count);
        Assert.Single(addresses, a => a.IsDefault);
        Assert.Equal(address2.Id, addresses.First(a => a.IsDefault).Id);
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
