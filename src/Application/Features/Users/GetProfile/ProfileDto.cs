// <copyright file="ProfileDto.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Users.GetProfile;

/// <summary>
/// User profile data transfer object.
/// </summary>
/// <param name="Id">The user identifier.</param>
/// <param name="Phone">The user's phone number.</param>
/// <param name="Email">The user's email (optional).</param>
/// <param name="FullName">The user's full name.</param>
/// <param name="IsVerified">Indicates whether the user is verified.</param>
public record ProfileDto(
    Guid Id,
    string Phone,
    string? Email,
    string FullName,
    bool IsVerified);
