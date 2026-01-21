// <copyright file="LoginQuery.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Auth.Login;

/// <summary>
/// Query to authenticate a user.
/// </summary>
/// <param name="PhoneOrEmail">The phone number or email.</param>
/// <param name="Password">The password.</param>
public record LoginQuery(string PhoneOrEmail, string Password);
