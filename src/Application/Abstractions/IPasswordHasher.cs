// <copyright file="IPasswordHasher.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Abstractions;

/// <summary>
/// Interface for password hashing.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a password.
    /// </summary>
    /// <param name="password">The plain text password.</param>
    /// <returns>The hashed password.</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a password against a hash.
    /// </summary>
    /// <param name="password">The plain text password.</param>
    /// <param name="hashedPassword">The hashed password.</param>
    /// <returns>True if the password matches; otherwise, false.</returns>
    bool VerifyPassword(string password, string hashedPassword);
}
