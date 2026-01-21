// <copyright file="VerifyOtpCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Auth.VerifyOtp;

/// <summary>
/// Command to verify OTP.
/// </summary>
/// <param name="PhoneOrEmail">The phone number or email.</param>
/// <param name="Otp">The OTP code.</param>
public record VerifyOtpCommand(string PhoneOrEmail, string Otp);
