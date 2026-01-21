// <copyright file="AdminLoginCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.AdminAuth.Login;

/// <summary>
/// Command to login a staff user.
/// </summary>
/// <param name="EmailOrPhone">Email or phone of the staff user.</param>
/// <param name="Password">Password.</param>
public record AdminLoginCommand(string EmailOrPhone, string Password);
