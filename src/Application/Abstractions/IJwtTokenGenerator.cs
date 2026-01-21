// <copyright file="IJwtTokenGenerator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Abstractions;

/// <summary>
/// Interface for JWT token generation.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="phone">The user's phone number.</param>
    /// <returns>The generated JWT token.</returns>
    string GenerateToken(Guid userId, string phone);
}
