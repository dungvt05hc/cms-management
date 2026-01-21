// <copyright file="AuthEndpointTests.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

using Application.Abstractions;
using Application.Features.Auth.ForgotPassword;
using Application.Features.Auth.Login;
using Application.Features.Auth.Register;
using Application.Features.Auth.ResetPassword;
using Application.Features.Auth.VerifyOtp;
using Application.Features.Users.GetProfile;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Api.IntegrationTests;

/// <summary>
/// Integration tests for the Auth endpoints.
/// </summary>
public sealed class AuthEndpointTests : IDisposable
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthEndpointTests"/> class.
    /// </summary>
    public AuthEndpointTests()
    {
        this.factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Find and remove all EF Core and database-related service descriptors
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

                    // Remove any EF Core internal services
                    var efCoreServices = services
                        .Where(d => d.ServiceType.Namespace != null &&
                                    d.ServiceType.Namespace.StartsWith("Microsoft.EntityFrameworkCore"))
                        .ToList();

                    foreach (var descriptor in efCoreServices)
                    {
                        services.Remove(descriptor);
                    }

                    // Add in-memory database for testing with unique name per test class instance
                    var dbName = $"TestDb_{Guid.NewGuid()}";
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(dbName);
                    });

                    // Re-register IAppDbContext
                    services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
                });
            });

        this.client = this.factory.CreateClient();
    }

    /// <summary>
    /// Tests the full auth flow: register -&gt; verify OTP -&gt; login -&gt; get profile.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task AuthFlow_RegisterVerifyLoginGetProfile_ReturnsSuccess()
    {
        // Arrange
        var phoneNumber = $"+8490{Random.Shared.Next(1000000, 9999999)}";
        var registerCommand = new RegisterCommand(
            Phone: phoneNumber,
            Email: $"test-{Guid.NewGuid()}@example.com",
            FullName: "Test User",
            Password: "SecurePassword123");

        // Act 1: Register
        var registerResponse = await this.client.PostAsJsonAsync("/auth/register", registerCommand);

        // Assert 1: Registration should succeed
        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<RegisterResult>();
        Assert.NotNull(registerResult);
        Assert.NotEqual(Guid.Empty, registerResult.RegistrationId);

        // Act 2: Verify OTP (stub accepts "123456")
        var verifyOtpCommand = new VerifyOtpCommand(
            PhoneOrEmail: registerCommand.Phone,
            Otp: "123456");
        var verifyOtpResponse = await this.client.PostAsJsonAsync("/auth/otp/verify", verifyOtpCommand);

        // Assert 2: OTP verification should succeed
        Assert.Equal(HttpStatusCode.OK, verifyOtpResponse.StatusCode);

        // Act 3: Login
        var loginQuery = new LoginQuery(
            PhoneOrEmail: registerCommand.Phone,
            Password: registerCommand.Password);
        var loginResponse = await this.client.PostAsJsonAsync("/auth/login", loginQuery);

        // Assert 3: Login should succeed
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResult>();
        Assert.NotNull(loginResult);
        Assert.NotNull(loginResult.AccessToken);
        Assert.True(loginResult.ExpiresIn > 0);

        // Act 4: Get profile using JWT token
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.AccessToken);
        var profileResponse = await this.client.GetAsync("/me");

        // Assert 4: Profile retrieval should succeed
        Assert.Equal(HttpStatusCode.OK, profileResponse.StatusCode);
        var profile = await profileResponse.Content.ReadFromJsonAsync<ProfileDto>();
        Assert.NotNull(profile);
        Assert.Equal(registerCommand.Phone, profile.Phone);
        Assert.Equal(registerCommand.Email, profile.Email);
        Assert.Equal(registerCommand.FullName, profile.FullName);
        Assert.True(profile.IsVerified);
    }

    /// <summary>
    /// Tests that registering with the same phone twice returns Conflict.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task Register_DuplicatePhone_ReturnsConflict()
    {
        // Arrange
        var phoneNumber = $"+8490{Random.Shared.Next(1000000, 9999999)}";
        var registerCommand1 = new RegisterCommand(
            Phone: phoneNumber,
            Email: $"test1-{Guid.NewGuid()}@example.com",
            FullName: "Test User 1",
            Password: "SecurePassword123");

        var registerCommand2 = new RegisterCommand(
            Phone: phoneNumber, // Same phone
            Email: $"test2-{Guid.NewGuid()}@example.com",
            FullName: "Test User 2",
            Password: "SecurePassword456");

        // Act
        var response1 = await this.client.PostAsJsonAsync("/auth/register", registerCommand1);
        var response2 = await this.client.PostAsJsonAsync("/auth/register", registerCommand2);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, response2.StatusCode);
    }

    /// <summary>
    /// Tests that registering with invalid phone format returns BadRequest.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task Register_InvalidPhoneFormat_ReturnsBadRequest()
    {
        // Arrange
        var registerCommand = new RegisterCommand(
            Phone: "invalid-phone",
            Email: "test@example.com",
            FullName: "Test User",
            Password: "SecurePassword123");

        // Act
        var response = await this.client.PostAsJsonAsync("/auth/register", registerCommand);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// Tests that registering with a weak password returns BadRequest.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task Register_WeakPassword_ReturnsBadRequest()
    {
        // Arrange
        var phoneNumber = $"+8490{Random.Shared.Next(1000000, 9999999)}";
        var registerCommand = new RegisterCommand(
            Phone: phoneNumber,
            Email: $"test-{Guid.NewGuid()}@example.com",
            FullName: "Test User",
            Password: "short");

        // Act
        var response = await this.client.PostAsJsonAsync("/auth/register", registerCommand);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// Tests that verifying OTP with invalid code returns BadRequest.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task VerifyOtp_InvalidCode_ReturnsBadRequest()
    {
        // Arrange
        var phoneNumber = $"+8490{Random.Shared.Next(1000000, 9999999)}";
        var registerCommand = new RegisterCommand(
            Phone: phoneNumber,
            Email: $"test-{Guid.NewGuid()}@example.com",
            FullName: "Test User",
            Password: "SecurePassword123");
        await this.client.PostAsJsonAsync("/auth/register", registerCommand);

        var verifyOtpCommand = new VerifyOtpCommand(
            PhoneOrEmail: registerCommand.Phone,
            Otp: "999999"); // Invalid OTP

        // Act
        var response = await this.client.PostAsJsonAsync("/auth/otp/verify", verifyOtpCommand);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// Tests that login with invalid credentials returns Unauthorized.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var phoneNumber = $"+8490{Random.Shared.Next(1000000, 9999999)}";
        var registerCommand = new RegisterCommand(
            Phone: phoneNumber,
            Email: $"test-{Guid.NewGuid()}@example.com",
            FullName: "Test User",
            Password: "SecurePassword123");
        await this.client.PostAsJsonAsync("/auth/register", registerCommand);

        var loginQuery = new LoginQuery(
            PhoneOrEmail: registerCommand.Phone,
            Password: "WrongPassword");

        // Act
        var response = await this.client.PostAsJsonAsync("/auth/login", loginQuery);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Tests that login with non-existent user returns Unauthorized.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task Login_NonExistentUser_ReturnsUnauthorized()
    {
        // Arrange
        var phoneNumber = $"+8490{Random.Shared.Next(1000000, 9999999)}";
        var loginQuery = new LoginQuery(
            PhoneOrEmail: phoneNumber,
            Password: "AnyPassword123");

        // Act
        var response = await this.client.PostAsJsonAsync("/auth/login", loginQuery);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Tests that accessing /me without authentication returns Unauthorized.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetMe_NoAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await this.client.GetAsync("/me");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Tests that accessing /me with invalid token returns Unauthorized.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetMe_InvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid-token");

        // Act
        var response = await this.client.GetAsync("/me");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Tests that login with email works correctly.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task Login_WithEmail_ReturnsSuccess()
    {
        // Arrange
        var phoneNumber = $"+8490{Random.Shared.Next(1000000, 9999999)}";
        var email = $"email-login-{Guid.NewGuid()}@example.com";
        var registerCommand = new RegisterCommand(
            Phone: phoneNumber,
            Email: email,
            FullName: "Test User Email",
            Password: "SecurePassword123");

        await this.client.PostAsJsonAsync("/auth/register", registerCommand);

        // Verify OTP
        var verifyOtpCommand = new VerifyOtpCommand(
            PhoneOrEmail: registerCommand.Phone,
            Otp: "123456");
        await this.client.PostAsJsonAsync("/auth/otp/verify", verifyOtpCommand);

        // Act: Login with email
        var loginQuery = new LoginQuery(
            PhoneOrEmail: email,
            Password: registerCommand.Password);
        var response = await this.client.PostAsJsonAsync("/auth/login", loginQuery);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var loginResult = await response.Content.ReadFromJsonAsync<LoginResult>();
        Assert.NotNull(loginResult);
        Assert.NotNull(loginResult.AccessToken);
    }

    /// <summary>
    /// Tests the forgot password flow: request reset -&gt; get token -&gt; reset password -&gt; login with new password.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ForgotPassword_ValidFlow_ReturnsSuccess()
    {
        // Arrange - Create and verify a user
        var phoneNumber = $"+8490{Random.Shared.Next(1000000, 9999999)}";
        var email = $"forgot-password-{Guid.NewGuid()}@example.com";
        var originalPassword = "OriginalPassword123";
        var newPassword = "NewPassword456";

        var registerCommand = new RegisterCommand(
            Phone: phoneNumber,
            Email: email,
            FullName: "Test User",
            Password: originalPassword);

        await this.client.PostAsJsonAsync("/auth/register", registerCommand);

        var verifyOtpCommand = new VerifyOtpCommand(
            PhoneOrEmail: phoneNumber,
            Otp: "123456");
        await this.client.PostAsJsonAsync("/auth/otp/verify", verifyOtpCommand);

        // Act 1: Request password reset
        var forgotPasswordCommand = new ForgotPasswordCommand(Email: email);
        var forgotPasswordResponse = await this.client.PostAsJsonAsync("/auth/password/forgot", forgotPasswordCommand);

        // Assert 1: Forgot password request should succeed
        Assert.Equal(HttpStatusCode.OK, forgotPasswordResponse.StatusCode);

        // Get the token from the database (simulating email)
        var dbContext = this.factory.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();
        var resetToken = await dbContext.PasswordResetTokens
            .Where(rt => !rt.IsUsed)
            .OrderByDescending(rt => rt.CreatedAt)
            .FirstOrDefaultAsync();
        Assert.NotNull(resetToken);

        // Act 2: Reset password with token
        var resetPasswordCommand = new ResetPasswordCommand(
            Token: resetToken.Token,
            NewPassword: newPassword);
        var resetPasswordResponse = await this.client.PostAsJsonAsync("/auth/password/reset", resetPasswordCommand);

        // Assert 2: Reset password should succeed
        Assert.Equal(HttpStatusCode.OK, resetPasswordResponse.StatusCode);

        // Act 3: Try to login with old password (should fail)
        var loginWithOldPassword = new LoginQuery(
            PhoneOrEmail: email,
            Password: originalPassword);
        var loginOldResponse = await this.client.PostAsJsonAsync("/auth/login", loginWithOldPassword);

        // Assert 3: Login with old password should fail
        Assert.Equal(HttpStatusCode.Unauthorized, loginOldResponse.StatusCode);

        // Act 4: Login with new password (should succeed)
        var loginWithNewPassword = new LoginQuery(
            PhoneOrEmail: email,
            Password: newPassword);
        var loginNewResponse = await this.client.PostAsJsonAsync("/auth/login", loginWithNewPassword);

        // Assert 4: Login with new password should succeed
        Assert.Equal(HttpStatusCode.OK, loginNewResponse.StatusCode);
        var loginResult = await loginNewResponse.Content.ReadFromJsonAsync<LoginResult>();
        Assert.NotNull(loginResult);
        Assert.NotNull(loginResult.AccessToken);
    }

    /// <summary>
    /// Tests that requesting password reset for non-existent email returns success (security).
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ForgotPassword_NonExistentEmail_ReturnsSuccess()
    {
        // Arrange
        var forgotPasswordCommand = new ForgotPasswordCommand(Email: "nonexistent@example.com");

        // Act
        var response = await this.client.PostAsJsonAsync("/auth/password/forgot", forgotPasswordCommand);

        // Assert - Should return success to prevent email enumeration
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    /// <summary>
    /// Tests that invalid email format returns BadRequest.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ForgotPassword_InvalidEmailFormat_ReturnsBadRequest()
    {
        // Arrange
        var forgotPasswordCommand = new ForgotPasswordCommand(Email: "invalid-email");

        // Act
        var response = await this.client.PostAsJsonAsync("/auth/password/forgot", forgotPasswordCommand);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// Tests that using an invalid token returns BadRequest.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ResetPassword_InvalidToken_ReturnsBadRequest()
    {
        // Arrange
        var resetPasswordCommand = new ResetPasswordCommand(
            Token: "invalid-token",
            NewPassword: "NewPassword123");

        // Act
        var response = await this.client.PostAsJsonAsync("/auth/password/reset", resetPasswordCommand);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// Tests that using a token twice returns BadRequest.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ResetPassword_TokenUsedTwice_ReturnsBadRequest()
    {
        // Arrange - Create and verify a user
        var phoneNumber = $"+8490{Random.Shared.Next(1000000, 9999999)}";
        var email = $"token-reuse-{Guid.NewGuid()}@example.com";

        var registerCommand = new RegisterCommand(
            Phone: phoneNumber,
            Email: email,
            FullName: "Test User",
            Password: "OriginalPassword123");

        await this.client.PostAsJsonAsync("/auth/register", registerCommand);

        var verifyOtpCommand = new VerifyOtpCommand(
            PhoneOrEmail: phoneNumber,
            Otp: "123456");
        await this.client.PostAsJsonAsync("/auth/otp/verify", verifyOtpCommand);

        // Request password reset
        var forgotPasswordCommand = new ForgotPasswordCommand(Email: email);
        await this.client.PostAsJsonAsync("/auth/password/forgot", forgotPasswordCommand);

        // Get the token from the database
        var dbContext = this.factory.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();
        var resetToken = await dbContext.PasswordResetTokens
            .Where(rt => !rt.IsUsed)
            .OrderByDescending(rt => rt.CreatedAt)
            .FirstOrDefaultAsync();
        Assert.NotNull(resetToken);

        // Act 1: Use token first time
        var resetPasswordCommand = new ResetPasswordCommand(
            Token: resetToken.Token,
            NewPassword: "NewPassword123");
        var firstResponse = await this.client.PostAsJsonAsync("/auth/password/reset", resetPasswordCommand);

        // Assert 1: First use should succeed
        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

        // Act 2: Try to use the same token again
        var secondResponse = await this.client.PostAsJsonAsync("/auth/password/reset", resetPasswordCommand);

        // Assert 2: Second use should fail
        Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
    }

    /// <summary>
    /// Tests that an expired token returns BadRequest.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ResetPassword_ExpiredToken_ReturnsBadRequest()
    {
        // Arrange - Create and verify a user
        var phoneNumber = $"+8490{Random.Shared.Next(1000000, 9999999)}";
        var email = $"expired-token-{Guid.NewGuid()}@example.com";

        var registerCommand = new RegisterCommand(
            Phone: phoneNumber,
            Email: email,
            FullName: "Test User",
            Password: "OriginalPassword123");

        await this.client.PostAsJsonAsync("/auth/register", registerCommand);

        var verifyOtpCommand = new VerifyOtpCommand(
            PhoneOrEmail: phoneNumber,
            Otp: "123456");
        await this.client.PostAsJsonAsync("/auth/otp/verify", verifyOtpCommand);

        // Request password reset
        var forgotPasswordCommand = new ForgotPasswordCommand(Email: email);
        await this.client.PostAsJsonAsync("/auth/password/forgot", forgotPasswordCommand);

        // Get the token from the database and manually expire it
        var dbContext = this.factory.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();
        var resetToken = await dbContext.PasswordResetTokens
            .Where(rt => !rt.IsUsed)
            .OrderByDescending(rt => rt.CreatedAt)
            .FirstOrDefaultAsync();
        Assert.NotNull(resetToken);

        // Expire the token by setting ExpiresAt to past
        resetToken.ExpiresAt = DateTime.UtcNow.AddHours(-1);
        await dbContext.SaveChangesAsync();

        // Act: Try to use expired token
        var resetPasswordCommand = new ResetPasswordCommand(
            Token: resetToken.Token,
            NewPassword: "NewPassword123");
        var response = await this.client.PostAsJsonAsync("/auth/password/reset", resetPasswordCommand);

        // Assert: Should fail with BadRequest
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <summary>
    /// Tests that weak password in reset returns BadRequest.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task ResetPassword_WeakPassword_ReturnsBadRequest()
    {
        // Arrange
        var resetPasswordCommand = new ResetPasswordCommand(
            Token: "some-token",
            NewPassword: "weak");

        // Act
        var response = await this.client.PostAsJsonAsync("/auth/password/reset", resetPasswordCommand);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.client?.Dispose();
        this.factory?.Dispose();
    }
}
