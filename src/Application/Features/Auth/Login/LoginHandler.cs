// <copyright file="LoginHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.Login;

/// <summary>
/// Handler for the LoginQuery.
/// </summary>
public class LoginHandler
{
    private readonly IAppDbContext dbContext;
    private readonly IPasswordHasher passwordHasher;
    private readonly IJwtTokenGenerator jwtTokenGenerator;
    private readonly ILogger<LoginHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="passwordHasher">The password hasher.</param>
    /// <param name="jwtTokenGenerator">The JWT token generator.</param>
    /// <param name="logger">The logger.</param>
    public LoginHandler(
        IAppDbContext dbContext,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        ILogger<LoginHandler> logger)
    {
        this.dbContext = dbContext;
        this.passwordHasher = passwordHasher;
        this.jwtTokenGenerator = jwtTokenGenerator;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the login query.
    /// </summary>
    /// <param name="query">The login query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The login result with JWT token.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when credentials are invalid.</exception>
    public async Task<LoginResult> Handle(LoginQuery query, CancellationToken cancellationToken)
    {
        // Find user by phone or email
        var user = await this.dbContext.Users
            .FirstOrDefaultAsync(
                u => u.Phone == query.PhoneOrEmail || u.Email == query.PhoneOrEmail,
                cancellationToken);

        if (user == null)
        {
            this.logger.LogWarning("Login failed: User not found");
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        // Verify password
        if (!this.passwordHasher.VerifyPassword(query.Password, user.PasswordHash))
        {
            this.logger.LogWarning("Login failed: Invalid password for user {UserId}", user.Id);
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        // Generate JWT token
        var token = this.jwtTokenGenerator.GenerateToken(user.Id, user.Phone);

        this.logger.LogInformation("User {UserId} logged in successfully", user.Id);

        return new LoginResult(token, 3600); // 1 hour expiration
    }
}
