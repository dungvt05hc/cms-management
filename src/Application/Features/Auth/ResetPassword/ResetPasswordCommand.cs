// <copyright file="ResetPasswordCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Auth.ResetPassword;

/// <summary>
/// Command to reset password using a token.
/// </summary>
/// <param name="Token">The password reset token.</param>
/// <param name="NewPassword">The new password.</param>
public record ResetPasswordCommand(string Token, string NewPassword);
