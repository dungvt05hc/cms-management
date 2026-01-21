// <copyright file="VerifyOtpHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.VerifyOtp;

/// <summary>
/// Handler for the VerifyOtpCommand (stub implementation for dev/test).
/// </summary>
public class VerifyOtpHandler
{
    private readonly IAppDbContext dbContext;
    private readonly IDateTime dateTime;
    private readonly ILogger<VerifyOtpHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="VerifyOtpHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="dateTime">The date time service.</param>
    /// <param name="logger">The logger.</param>
    public VerifyOtpHandler(
        IAppDbContext dbContext,
        IDateTime dateTime,
        ILogger<VerifyOtpHandler> logger)
    {
        this.dbContext = dbContext;
        this.dateTime = dateTime;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the OTP verification command (stub).
    /// Accepts "123456" as valid OTP for dev/test purposes.
    /// </summary>
    /// <param name="command">The OTP verification command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when OTP is invalid or user not found.</exception>
    public async Task Handle(VerifyOtpCommand command, CancellationToken cancellationToken)
    {
        // Stub: Accept "123456" as valid OTP for dev/test
        if (command.Otp != "123456")
        {
            this.logger.LogWarning("OTP verification failed: Invalid OTP");
            throw new InvalidOperationException("Invalid OTP.");
        }

        // Find user by phone or email
        var user = await this.dbContext.Users
            .FirstOrDefaultAsync(
                u => u.Phone == command.PhoneOrEmail || u.Email == command.PhoneOrEmail,
                cancellationToken);

        if (user == null)
        {
            this.logger.LogWarning("OTP verification failed: User not found for {PhoneOrEmail}", command.PhoneOrEmail);
            throw new InvalidOperationException("User not found.");
        }

        // Mark user as verified
        user.IsVerified = true;
        user.UpdatedAt = this.dateTime.UtcNow;

        await this.dbContext.SaveChangesAsync(cancellationToken);

        this.logger.LogInformation("OTP verified successfully for user {UserId}", user.Id);
    }
}
