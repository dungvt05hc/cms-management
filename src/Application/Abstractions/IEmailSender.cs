// <copyright file="IEmailSender.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Abstractions;

/// <summary>
/// Interface for sending emails.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Sends a password reset email.
    /// </summary>
    /// <param name="email">The recipient email address.</param>
    /// <param name="token">The reset token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendPasswordResetEmailAsync(string email, string token, CancellationToken cancellationToken = default);
}
