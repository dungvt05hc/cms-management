// <copyright file="AdminLoginResult.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.AdminAuth.Login;

/// <summary>
/// Result of staff login.
/// </summary>
/// <param name="AccessToken">JWT access token.</param>
/// <param name="ExpiresIn">Expiration time in minutes.</param>
/// <param name="Role">Role of the staff user.</param>
public record AdminLoginResult(string AccessToken, int ExpiresIn, string Role);
