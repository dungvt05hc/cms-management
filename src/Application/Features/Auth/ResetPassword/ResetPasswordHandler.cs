// <copyright file="ResetPasswordHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.ResetPassword;

/// <summary>
/// Handler for ResetPasswordCommand.
/// </summary>
public class ResetPasswordHandler
{
    private readonly IAppDbContext dbContext;
    private readonly IPasswordHasher passwordHasher;
    private readonly IDateTime dateTime;
    private readonly ILogger<ResetPasswordHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResetPasswordHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="passwordHasher">The password hasher.</param>
    /// <param name="dateTime">The date time service.</param>
    /// <param name="logger">The logger.</param>
    public ResetPasswordHandler(
        IAppDbContext dbContext,
        IPasswordHasher passwordHasher,
        IDateTime dateTime,
        ILogger<ResetPasswordHandler> logger)
    {
        this.dbContext = dbContext;
        this.passwordHasher = passwordHasher;
        this.dateTime = dateTime;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the reset password command.
    /// </summary>
    /// <param name="command">The reset password command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when token is invalid, expired, or already used.</exception>
    public async Task Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        // Find the reset token (do not log token for security)
        var resetToken = await this.dbContext.PasswordResetTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == command.Token, cancellationToken);

        if (resetToken == null)
        {
            this.logger.LogWarning("Invalid password reset token attempted");
            throw new InvalidOperationException("Invalid or expired reset token.");
        }

        // Check if token is already used
        if (resetToken.IsUsed)
        {
            this.logger.LogWarning("Attempt to use already used password reset token for user {UserId}", resetToken.UserId);
            throw new InvalidOperationException("Invalid or expired reset token.");
        }

        // Check if token is expired
        if (resetToken.ExpiresAt < this.dateTime.UtcNow)
        {
            this.logger.LogWarning("Expired password reset token attempted for user {UserId}", resetToken.UserId);
            throw new InvalidOperationException("Invalid or expired reset token.");
        }

        // Update user password
        var user = resetToken.User;
        user.PasswordHash = this.passwordHasher.HashPassword(command.NewPassword);
        user.UpdatedAt = this.dateTime.UtcNow;

        // Mark token as used
        resetToken.IsUsed = true;

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("Password successfully reset for user {UserId}", user.Id);
    }
}
