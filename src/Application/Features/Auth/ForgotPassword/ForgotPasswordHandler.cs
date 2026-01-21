// <copyright file="ForgotPasswordHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Security.Cryptography;

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.ForgotPassword;

/// <summary>
/// Handler for ForgotPasswordCommand.
/// </summary>
public class ForgotPasswordHandler
{
    private readonly IAppDbContext dbContext;
    private readonly IEmailSender emailSender;
    private readonly IDateTime dateTime;
    private readonly ILogger<ForgotPasswordHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForgotPasswordHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="emailSender">The email sender.</param>
    /// <param name="dateTime">The date time service.</param>
    /// <param name="logger">The logger.</param>
    public ForgotPasswordHandler(
        IAppDbContext dbContext,
        IEmailSender emailSender,
        IDateTime dateTime,
        ILogger<ForgotPasswordHandler> logger)
    {
        this.dbContext = dbContext;
        this.emailSender = emailSender;
        this.dateTime = dateTime;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the forgot password command.
    /// </summary>
    /// <param name="command">The forgot password command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        // Mask email for logging
        var maskedEmail = MaskEmail(command.Email);

        // Find user by email
        var user = await this.dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

        if (user == null)
        {
            // For security, don't reveal if email exists - just log and return success
            this.logger.LogInformation("Password reset requested for non-existent email: {Email}", maskedEmail);
            return;
        }

        // Generate secure random token
        var token = GenerateSecureToken();

        // Create password reset token entity
        var resetToken = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = token,
            ExpiresAt = this.dateTime.UtcNow.AddHours(1), // Token expires in 1 hour
            IsUsed = false,
            CreatedAt = this.dateTime.UtcNow,
        };

        this.dbContext.PasswordResetTokens.Add(resetToken);
        await this.dbContext.SaveChangesAsync(cancellationToken);

        // Send email with reset token
        await this.emailSender.SendPasswordResetEmailAsync(command.Email, token, cancellationToken);

        this.logger.LogInformation("Password reset token generated for email: {Email}", maskedEmail);
    }

    private static string GenerateSecureToken()
    {
        // Generate a secure random token (32 bytes = 256 bits)
        var randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        return Convert.ToBase64String(randomBytes);
    }

    private static string MaskEmail(string email)
    {
        var atIndex = email.IndexOf('@');
        if (atIndex <= 2)
        {
            return "***" + email.Substring(atIndex);
        }

        return email.Substring(0, 2) + "***" + email.Substring(atIndex);
    }
}
