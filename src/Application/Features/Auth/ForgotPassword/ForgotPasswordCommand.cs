// <copyright file="ForgotPasswordCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Auth.ForgotPassword;

/// <summary>
/// Command to initiate password reset via email.
/// </summary>
/// <param name="Email">The email address of the user requesting password reset.</param>
public record ForgotPasswordCommand(string Email);
