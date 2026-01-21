// <copyright file="VerifyOtpValidator.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using FluentValidation;

namespace Application.Features.Auth.VerifyOtp;

/// <summary>
/// Validator for VerifyOtpCommand.
/// </summary>
public class VerifyOtpValidator : AbstractValidator<VerifyOtpCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VerifyOtpValidator"/> class.
    /// </summary>
    public VerifyOtpValidator()
    {
        this.RuleFor(x => x.PhoneOrEmail)
            .NotEmpty().WithMessage("Phone or email is required.");

        this.RuleFor(x => x.Otp)
            .NotEmpty().WithMessage("OTP is required.")
            .Length(6).WithMessage("OTP must be 6 characters.");
    }
}
