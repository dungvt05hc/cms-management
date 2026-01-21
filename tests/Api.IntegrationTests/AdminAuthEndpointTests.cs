// <copyright file="AdminAuthEndpointTests.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using Application.Abstractions;
using Application.Features.AdminAuth.Login;
using Application.Features.Staff.CreateStaff;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.IntegrationTests;

/// <summary>
/// Integration tests for the Admin Auth endpoints.
/// </summary>
public sealed class AdminAuthEndpointTests : IDisposable
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminAuthEndpointTests"/> class.
    /// </summary>
    public AdminAuthEndpointTests()
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
    /// Tests the full admin flow: bootstrap super admin -> login -> create staff -> staff login -> access admin endpoint.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task AdminAuthFlow_BootstrapSuperAdmin_LoginCreateStaff_ReturnsSuccess()
    {
        // Arrange: Bootstrap super admin manually for test
        var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var dateTime = scope.ServiceProvider.GetRequiredService<IDateTime>();

        var superAdmin = new Domain.Entities.StaffUser
        {
            Id = Guid.NewGuid(),
            Email = "superadmin@test.com",
            FullName = "Super Admin",
            PasswordHash = passwordHasher.HashPassword("SuperAdmin123!"),
            Role = Domain.Entities.StaffRole.SuperAdmin,
            IsActive = true,
            CreatedAt = dateTime.UtcNow,
            UpdatedAt = dateTime.UtcNow,
        };

        dbContext.StaffUsers.Add(superAdmin);
        await dbContext.SaveChangesAsync();

        // Act 1: Super admin login
        var loginCommand = new AdminLoginCommand("superadmin@test.com", "SuperAdmin123!");
        var loginResponse = await this.client.PostAsJsonAsync("/admin/auth/login", loginCommand);

        // Assert 1: Login should succeed
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AdminLoginResult>();
        Assert.NotNull(loginResult);
        Assert.NotNull(loginResult.AccessToken);
        Assert.Equal("SuperAdmin", loginResult.Role);

        // Act 2: Create staff user
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.AccessToken);
        var createStaffCommand = new CreateStaffCommand(
            Email: "staff@test.com",
            Phone: null,
            FullName: "Test Staff",
            Password: "StaffPassword123!",
            Role: "Admin");

        var createStaffResponse = await this.client.PostAsJsonAsync("/admin/staff", createStaffCommand);

        // Assert 2: Staff creation should succeed
        Assert.Equal(HttpStatusCode.Created, createStaffResponse.StatusCode);
        var createStaffResult = await createStaffResponse.Content.ReadFromJsonAsync<CreateStaffResult>();
        Assert.NotNull(createStaffResult);
        Assert.NotEqual(Guid.Empty, createStaffResult.StaffId);
        Assert.Equal("staff@test.com", createStaffResult.Email);
        Assert.Equal("Admin", createStaffResult.Role);

        // Act 3: Staff login
        this.client.DefaultRequestHeaders.Authorization = null;
        var staffLoginCommand = new AdminLoginCommand("staff@test.com", "StaffPassword123!");
        var staffLoginResponse = await this.client.PostAsJsonAsync("/admin/auth/login", staffLoginCommand);

        // Assert 3: Staff login should succeed
        Assert.Equal(HttpStatusCode.OK, staffLoginResponse.StatusCode);
        var staffLoginResult = await staffLoginResponse.Content.ReadFromJsonAsync<AdminLoginResult>();
        Assert.NotNull(staffLoginResult);
        Assert.NotNull(staffLoginResult.AccessToken);
        Assert.Equal("Admin", staffLoginResult.Role);

        // Act 4: Staff can access admin endpoint (create another staff)
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", staffLoginResult.AccessToken);
        var createStaffCommand2 = new CreateStaffCommand(
            Email: "staff2@test.com",
            Phone: null,
            FullName: "Test Staff 2",
            Password: "StaffPassword456!",
            Role: "Staff");

        var createStaffResponse2 = await this.client.PostAsJsonAsync("/admin/staff", createStaffCommand2);

        // Assert 4: Staff should be able to create another staff
        Assert.Equal(HttpStatusCode.Created, createStaffResponse2.StatusCode);
    }

    /// <summary>
    /// Tests that buyer (regular user) cannot access admin endpoints.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task AdminEndpoint_BuyerAccess_ReturnsForbidden()
    {
        // Arrange: Create a buyer user and get their token
        var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var jwtGenerator = scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();
        var dateTime = scope.ServiceProvider.GetRequiredService<IDateTime>();

        var buyer = new Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            Phone = "+84901234567",
            Email = "buyer@test.com",
            FullName = "Test Buyer",
            PasswordHash = passwordHasher.HashPassword("BuyerPassword123!"),
            IsVerified = true,
            CreatedAt = dateTime.UtcNow,
            UpdatedAt = dateTime.UtcNow,
        };

        dbContext.Users.Add(buyer);
        await dbContext.SaveChangesAsync();

        // Generate buyer token
        var buyerToken = jwtGenerator.GenerateToken(buyer.Id, buyer.Phone);

        // Act: Try to access admin endpoint with buyer token
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", buyerToken);
        var createStaffCommand = new CreateStaffCommand(
            Email: "staff@test.com",
            Phone: null,
            FullName: "Test Staff",
            Password: "StaffPassword123!",
            Role: "Admin");

        var response = await this.client.PostAsJsonAsync("/admin/staff", createStaffCommand);

        // Assert: Should return Forbidden (403)
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    /// <summary>
    /// Tests that accessing admin endpoints without authentication returns Unauthorized.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task AdminEndpoint_NoAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var createStaffCommand = new CreateStaffCommand(
            Email: "staff@test.com",
            Phone: null,
            FullName: "Test Staff",
            Password: "StaffPassword123!",
            Role: "Admin");

        // Act: Try to access admin endpoint without token
        var response = await this.client.PostAsJsonAsync("/admin/staff", createStaffCommand);

        // Assert: Should return Unauthorized (401)
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Tests that admin login with invalid credentials returns Unauthorized.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task AdminLogin_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange: Bootstrap super admin
        var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var dateTime = scope.ServiceProvider.GetRequiredService<IDateTime>();

        var superAdmin = new Domain.Entities.StaffUser
        {
            Id = Guid.NewGuid(),
            Email = "superadmin@test.com",
            FullName = "Super Admin",
            PasswordHash = passwordHasher.HashPassword("SuperAdmin123!"),
            Role = Domain.Entities.StaffRole.SuperAdmin,
            IsActive = true,
            CreatedAt = dateTime.UtcNow,
            UpdatedAt = dateTime.UtcNow,
        };

        dbContext.StaffUsers.Add(superAdmin);
        await dbContext.SaveChangesAsync();

        // Act: Login with wrong password
        var loginCommand = new AdminLoginCommand("superadmin@test.com", "WrongPassword");
        var response = await this.client.PostAsJsonAsync("/admin/auth/login", loginCommand);

        // Assert: Should return Unauthorized
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Tests that creating staff with duplicate email returns Conflict.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task CreateStaff_DuplicateEmail_ReturnsConflict()
    {
        // Arrange: Bootstrap super admin and login
        var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var dateTime = scope.ServiceProvider.GetRequiredService<IDateTime>();

        var superAdmin = new Domain.Entities.StaffUser
        {
            Id = Guid.NewGuid(),
            Email = "superadmin@test.com",
            FullName = "Super Admin",
            PasswordHash = passwordHasher.HashPassword("SuperAdmin123!"),
            Role = Domain.Entities.StaffRole.SuperAdmin,
            IsActive = true,
            CreatedAt = dateTime.UtcNow,
            UpdatedAt = dateTime.UtcNow,
        };

        dbContext.StaffUsers.Add(superAdmin);
        await dbContext.SaveChangesAsync();

        var loginCommand = new AdminLoginCommand("superadmin@test.com", "SuperAdmin123!");
        var loginResponse = await this.client.PostAsJsonAsync("/admin/auth/login", loginCommand);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AdminLoginResult>();

        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult!.AccessToken);

        // Act: Create two staff with same email
        var createStaffCommand1 = new CreateStaffCommand(
            Email: "duplicate@test.com",
            Phone: null,
            FullName: "Staff 1",
            Password: "Password123!",
            Role: "Admin");

        var createStaffCommand2 = new CreateStaffCommand(
            Email: "duplicate@test.com",
            Phone: null,
            FullName: "Staff 2",
            Password: "Password456!",
            Role: "Admin");

        var response1 = await this.client.PostAsJsonAsync("/admin/staff", createStaffCommand1);
        var response2 = await this.client.PostAsJsonAsync("/admin/staff", createStaffCommand2);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, response2.StatusCode);
    }

    /// <summary>
    /// Tests that inactive staff cannot login.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task AdminLogin_InactiveAccount_ReturnsUnauthorized()
    {
        // Arrange: Create inactive staff
        var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var dateTime = scope.ServiceProvider.GetRequiredService<IDateTime>();

        var inactiveStaff = new Domain.Entities.StaffUser
        {
            Id = Guid.NewGuid(),
            Email = "inactive@test.com",
            FullName = "Inactive Staff",
            PasswordHash = passwordHasher.HashPassword("Password123!"),
            Role = Domain.Entities.StaffRole.Staff,
            IsActive = false, // Inactive
            CreatedAt = dateTime.UtcNow,
            UpdatedAt = dateTime.UtcNow,
        };

        dbContext.StaffUsers.Add(inactiveStaff);
        await dbContext.SaveChangesAsync();

        // Act: Try to login with inactive account
        var loginCommand = new AdminLoginCommand("inactive@test.com", "Password123!");
        var response = await this.client.PostAsJsonAsync("/admin/auth/login", loginCommand);

        // Assert: Should return Unauthorized
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.client?.Dispose();
        this.factory?.Dispose();
    }
}
