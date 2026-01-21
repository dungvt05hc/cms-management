// <copyright file="EmailSender.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Stub implementation of IEmailSender for development/testing.
/// </summary>
public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailSender"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public EmailSender(ILogger<EmailSender> logger)
    {
        this.logger = logger;
    }

    /// <inheritdoc/>
    public Task SendPasswordResetEmailAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        // Mask email for logging (show first 2 chars and domain)
        var maskedEmail = MaskEmail(email);

        // Log that email would be sent (do not log the token itself for security)
        this.logger.LogInformation("Password reset email would be sent to {Email}", maskedEmail);

        // In a real implementation, this would send an actual email
        // For MVP stub, we just log and return completed task
        return Task.CompletedTask;
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
