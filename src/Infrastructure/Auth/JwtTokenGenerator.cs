// <copyright file="JwtTokenGenerator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Application.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Auth;

/// <summary>
/// JWT token generator implementation.
/// </summary>
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings jwtSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtTokenGenerator"/> class.
    /// </summary>
    /// <param name="jwtSettings">The JWT settings.</param>
    public JwtTokenGenerator(IOptions<JwtSettings> jwtSettings)
    {
        this.jwtSettings = jwtSettings.Value;
    }

    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="phone">The user's phone number.</param>
    /// <returns>The generated JWT token.</returns>
    public string GenerateToken(Guid userId, string phone)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim("phone", phone),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: this.jwtSettings.Issuer,
            audience: this.jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(this.jwtSettings.ExpirationMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
