// <copyright file="GetProfileQuery.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Users.GetProfile;

/// <summary>
/// Query to get user profile.
/// </summary>
/// <param name="UserId">The user identifier.</param>
public record GetProfileQuery(Guid UserId);
