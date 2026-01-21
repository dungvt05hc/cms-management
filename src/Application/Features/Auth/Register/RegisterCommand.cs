// <copyright file="RegisterCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Auth.Register;

/// <summary>
/// Command to register a new user.
/// </summary>
/// <param name="Phone">The user's phone number.</param>
/// <param name="Email">The user's email (optional).</param>
/// <param name="FullName">The user's full name.</param>
/// <param name="Password">The user's password.</param>
public record RegisterCommand(
    string Phone,
    string? Email,
    string FullName,
    string Password);
