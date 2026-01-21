// <copyright file="LoginResult.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Auth.Login;

/// <summary>
/// Result of a login operation.
/// </summary>
/// <param name="AccessToken">The JWT access token.</param>
/// <param name="ExpiresIn">The token expiration time in seconds.</param>
public record LoginResult(string AccessToken, int ExpiresIn);
