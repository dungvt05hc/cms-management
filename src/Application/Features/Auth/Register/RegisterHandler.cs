// <copyright file="RegisterHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.Register;

/// <summary>
/// Handler for the RegisterCommand.
/// </summary>
public class RegisterHandler
{
    private readonly IAppDbContext dbContext;
    private readonly IPasswordHasher passwordHasher;
    private readonly IDateTime dateTime;
    private readonly ILogger<RegisterHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="passwordHasher">The password hasher.</param>
    /// <param name="dateTime">The date time service.</param>
    /// <param name="logger">The logger.</param>
    public RegisterHandler(
        IAppDbContext dbContext,
        IPasswordHasher passwordHasher,
        IDateTime dateTime,
        ILogger<RegisterHandler> logger)
    {
        this.dbContext = dbContext;
        this.passwordHasher = passwordHasher;
        this.dateTime = dateTime;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the registration command.
    /// </summary>
    /// <param name="command">The registration command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The registration result.</returns>
    /// <exception cref="InvalidOperationException">Thrown when phone/email already exists.</exception>
    public async Task<RegisterResult> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        // Check if phone already exists
        var phoneExists = await this.dbContext.Users
            .AnyAsync(u => u.Phone == command.Phone, cancellationToken);

        if (phoneExists)
        {
            this.logger.LogWarning("Registration failed: Phone number already exists");
            throw new InvalidOperationException("Phone number already registered.");
        }

        // Check if email already exists (if provided)
        if (!string.IsNullOrWhiteSpace(command.Email))
        {
            var emailExists = await this.dbContext.Users
                .AnyAsync(u => u.Email == command.Email, cancellationToken);

            if (emailExists)
            {
                this.logger.LogWarning("Registration failed: Email already exists");
                throw new InvalidOperationException("Email already registered.");
            }
        }

        // Create user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Phone = command.Phone,
            Email = command.Email,
            FullName = command.FullName,
            PasswordHash = this.passwordHasher.HashPassword(command.Password),
            IsVerified = false,
            CreatedAt = this.dateTime.UtcNow,
            UpdatedAt = this.dateTime.UtcNow,
        };

        this.dbContext.Users.Add(user);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("User registered successfully with ID: {UserId}", user.Id);

        return new RegisterResult(user.Id);
    }
}
