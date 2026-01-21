// <copyright file="CreateStaffHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Staff.CreateStaff;

/// <summary>
/// Handler for CreateStaffCommand.
/// </summary>
public class CreateStaffHandler
{
    private readonly IAppDbContext dbContext;
    private readonly IPasswordHasher passwordHasher;
    private readonly IDateTime dateTime;
    private readonly ILogger<CreateStaffHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateStaffHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="passwordHasher">The password hasher.</param>
    /// <param name="dateTime">The date time service.</param>
    /// <param name="logger">The logger.</param>
    public CreateStaffHandler(
        IAppDbContext dbContext,
        IPasswordHasher passwordHasher,
        IDateTime dateTime,
        ILogger<CreateStaffHandler> logger)
    {
        this.dbContext = dbContext;
        this.passwordHasher = passwordHasher;
        this.dateTime = dateTime;
        this.logger = logger;
    }

    private static string SanitizeForLogging(string value)
    {
        return value?
            .Replace("\r", string.Empty, StringComparison.Ordinal)
            .Replace("\n", string.Empty, StringComparison.Ordinal);
    }

    /// <summary>
    /// Handles the create staff command.
    /// </summary>
    /// <param name="command">The create staff command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result with staff user details.</returns>
    public async Task<CreateStaffResult> Handle(CreateStaffCommand command, CancellationToken cancellationToken)
    {
        // Check if email already exists
        var existingByEmail = await this.dbContext.StaffUsers
            .AnyAsync(u => u.Email == command.Email, cancellationToken);

        if (existingByEmail)
        {
            var maskedEmail = command.Email.Length > 3
                ? $"{command.Email[..2]}***@{command.Email.Split('@')[1]}"
                : "***";
            this.logger.LogWarning("Staff creation failed: email {MaskedEmail} already exists", SanitizeForLogging(maskedEmail));
            throw new InvalidOperationException("Email already exists.");
        }

        // Check if phone already exists (if provided)
        if (!string.IsNullOrWhiteSpace(command.Phone))
        {
            var existingByPhone = await this.dbContext.StaffUsers
                .AnyAsync(u => u.Phone == command.Phone, cancellationToken);

            if (existingByPhone)
            {
                var maskedPhone = command.Phone.Length > 3
                    ? $"{command.Phone[..3]}***{command.Phone[^2..]}"
                    : "***";
                this.logger.LogWarning("Staff creation failed: phone {MaskedPhone} already exists", SanitizeForLogging(maskedPhone));
                throw new InvalidOperationException("Phone number already exists.");
            }
        }

        // Parse role
        if (!Enum.TryParse<StaffRole>(command.Role, out var role))
        {
            throw new InvalidOperationException("Invalid role specified.");
        }

        // Create staff user
        var staffUser = new StaffUser
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            Phone = command.Phone,
            FullName = command.FullName,
            PasswordHash = this.passwordHasher.HashPassword(command.Password),
            Role = role,
            IsActive = true,
            CreatedAt = this.dateTime.UtcNow,
            UpdatedAt = this.dateTime.UtcNow,
        };

        this.dbContext.StaffUsers.Add(staffUser);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        var maskedEmailSuccess = staffUser.Email.Length > 3
            ? $"{staffUser.Email[..2]}***@{staffUser.Email.Split('@')[1]}"
            : "***";
        this.logger.LogInformation("Staff user created: {MaskedEmail} with role {Role}", SanitizeForLogging(maskedEmailSuccess), staffUser.Role);

        return new CreateStaffResult(staffUser.Id, staffUser.Email, staffUser.FullName, staffUser.Role.ToString());
    }
}
