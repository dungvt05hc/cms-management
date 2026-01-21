// <copyright file="AdminLoginHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.AdminAuth.Login;

/// <summary>
/// Handler for AdminLoginCommand.
/// </summary>
public class AdminLoginHandler
{
    private readonly IAppDbContext dbContext;
    private readonly IPasswordHasher passwordHasher;
    private readonly IJwtTokenGenerator jwtTokenGenerator;
    private readonly ILogger<AdminLoginHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminLoginHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="passwordHasher">The password hasher.</param>
    /// <param name="jwtTokenGenerator">The JWT token generator.</param>
    /// <param name="logger">The logger.</param>
    public AdminLoginHandler(
        IAppDbContext dbContext,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        ILogger<AdminLoginHandler> logger)
    {
        this.dbContext = dbContext;
        this.passwordHasher = passwordHasher;
        this.jwtTokenGenerator = jwtTokenGenerator;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the admin login command.
    /// </summary>
    /// <param name="command">The login command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The login result with JWT token.</returns>
    public async Task<AdminLoginResult> Handle(AdminLoginCommand command, CancellationToken cancellationToken)
    {
        var emailOrPhone = command.EmailOrPhone;
        var sanitizedEmailOrPhone = (emailOrPhone ?? string.Empty)
            .Replace("\r", string.Empty, StringComparison.Ordinal)
            .Replace("\n", string.Empty, StringComparison.Ordinal);

        // Find staff user by email or phone
        var staffUser = await this.dbContext.StaffUsers
            .FirstOrDefaultAsync(u => u.Email == emailOrPhone || u.Phone == emailOrPhone, cancellationToken);

        if (staffUser == null)
        {
            // Do not log user-supplied identifier to avoid exposing private information
            this.logger.LogWarning("Staff login attempt failed: user not found for provided credentials.");
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        if (!staffUser.IsActive)
        {
            var maskedEmail = staffUser.Email.Length > 3
                ? $"{staffUser.Email[..2]}***@{staffUser.Email.Split('@')[1]}"
                : "***";
            this.logger.LogWarning("Staff login attempt failed: account inactive for user with Id {UserId} and role {Role}", staffUser.Id, staffUser.Role);
        // Verify password
        if (!this.passwordHasher.VerifyPassword(command.Password, staffUser.PasswordHash))
        {
            var maskedEmail = staffUser.Email.Length > 3
                ? $"{staffUser.Email[..2]}***@{staffUser.Email.Split('@')[1]}"
                : "***";
            this.logger.LogWarning("Staff login attempt failed: invalid password for user with Id {UserId} and role {Role}", staffUser.Id, staffUser.Role);
        // Generate JWT token with role
        var token = this.jwtTokenGenerator.GenerateStaffToken(
            staffUser.Id,
            staffUser.Email,
            staffUser.Role.ToString());

        var maskedEmailSuccess = staffUser.Email.Length > 3
            ? $"{staffUser.Email[..2]}***@{staffUser.Email.Split('@')[1]}"
            : "***";
        this.logger.LogInformation("Staff user with Id {UserId} logged in successfully with role {Role}", staffUser.Id, staffUser.Role);
}
