// <copyright file="SuperAdminBootstrapService.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Service to bootstrap super admin account on first run.
/// </summary>
public class SuperAdminBootstrapService
{
    private readonly IAppDbContext dbContext;
    private readonly IPasswordHasher passwordHasher;
    private readonly IDateTime dateTime;
    private readonly IConfiguration configuration;
    private readonly ILogger<SuperAdminBootstrapService> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SuperAdminBootstrapService"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="passwordHasher">The password hasher.</param>
    /// <param name="dateTime">The date time service.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="logger">The logger.</param>
    public SuperAdminBootstrapService(
        IAppDbContext dbContext,
        IPasswordHasher passwordHasher,
        IDateTime dateTime,
        IConfiguration configuration,
        ILogger<SuperAdminBootstrapService> logger)
    {
        this.dbContext = dbContext;
        this.passwordHasher = passwordHasher;
        this.dateTime = dateTime;
        this.configuration = configuration;
        this.logger = logger;
    }

    /// <summary>
    /// Bootstraps the super admin account if it doesn't exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task BootstrapAsync(CancellationToken cancellationToken = default)
    {
        // Check if any super admin exists
        var superAdminExists = await this.dbContext.StaffUsers
            .AnyAsync(u => u.Role == StaffRole.SuperAdmin, cancellationToken);

        if (superAdminExists)
        {
            this.logger.LogInformation("Super admin already exists, skipping bootstrap");
            return;
        }

        // Get super admin credentials from configuration
        var email = this.configuration["SuperAdmin:Email"];
        var password = this.configuration["SuperAdmin:Password"];
        var fullName = this.configuration["SuperAdmin:FullName"] ?? "Super Admin";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            this.logger.LogWarning("Super admin credentials not configured in environment variables");
            return;
        }

        // Create super admin account
        var superAdmin = new StaffUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            Phone = null,
            FullName = fullName,
            PasswordHash = this.passwordHasher.HashPassword(password),
            Role = StaffRole.SuperAdmin,
            IsActive = true,
            CreatedAt = this.dateTime.UtcNow,
            UpdatedAt = this.dateTime.UtcNow,
        };

        this.dbContext.StaffUsers.Add(superAdmin);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Super admin account created successfully.");
    }
}
