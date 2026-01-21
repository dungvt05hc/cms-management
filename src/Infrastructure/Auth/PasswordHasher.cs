// <copyright file="PasswordHasher.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Security.Cryptography;

using Application.Abstractions;

namespace Infrastructure.Auth;

/// <summary>
/// Password hasher implementation using PBKDF2.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100000;

    /// <summary>
    /// Hashes a password using PBKDF2.
    /// </summary>
    /// <param name="password">The plain text password.</param>
    /// <returns>The hashed password.</returns>
    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);

        var hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Verifies a password against a hash.
    /// </summary>
    /// <param name="password">The plain text password.</param>
    /// <param name="hashedPassword">The hashed password.</param>
    /// <returns>True if the password matches; otherwise, false.</returns>
    public bool VerifyPassword(string password, string hashedPassword)
    {
        var hashBytes = Convert.FromBase64String(hashedPassword);

        var salt = new byte[SaltSize];
        Array.Copy(hashBytes, 0, salt, 0, SaltSize);

        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);

        for (int i = 0; i < HashSize; i++)
        {
            if (hashBytes[i + SaltSize] != hash[i])
            {
                return false;
            }
        }

        return true;
    }
}
