// <copyright file="JwtSettings.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Infrastructure.Auth;

/// <summary>
/// JWT configuration settings.
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Gets or sets the JWT secret key.
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the JWT issuer.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the JWT audience.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the JWT expiration time in minutes.
    /// </summary>
    public int ExpirationMinutes { get; set; } = 60;
}
