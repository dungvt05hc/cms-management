// <copyright file="GetProfileHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.GetProfile;

/// <summary>
/// Handler for the GetProfileQuery.
/// </summary>
public class GetProfileHandler
{
    private readonly IAppDbContext dbContext;
    private readonly ILogger<GetProfileHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetProfileHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="logger">The logger.</param>
    public GetProfileHandler(
        IAppDbContext dbContext,
        ILogger<GetProfileHandler> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    /// <summary>
    /// Handles the get profile query.
    /// </summary>
    /// <param name="query">The get profile query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user profile.</returns>
    /// <exception cref="InvalidOperationException">Thrown when user not found.</exception>
    public async Task<ProfileDto> Handle(GetProfileQuery query, CancellationToken cancellationToken)
    {
        var user = await this.dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == query.UserId, cancellationToken);

        if (user == null)
        {
            this.logger.LogWarning("User not found: {UserId}", query.UserId);
            throw new InvalidOperationException("User not found.");
        }

        return new ProfileDto(
            user.Id,
            user.Phone,
            user.Email,
            user.FullName,
            user.IsVerified);
    }
}
