// <copyright file="AuthController.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Features.Auth.Login;
using Application.Features.Auth.Register;
using Application.Features.Auth.VerifyOtp;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Authentication controller.
/// </summary>
[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterHandler registerHandler;
    private readonly VerifyOtpHandler verifyOtpHandler;
    private readonly LoginHandler loginHandler;
    private readonly IValidator<RegisterCommand> registerValidator;
    private readonly IValidator<VerifyOtpCommand> verifyOtpValidator;
    private readonly IValidator<LoginQuery> loginValidator;
    private readonly ILogger<AuthController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="registerHandler">The register handler.</param>
    /// <param name="verifyOtpHandler">The verify OTP handler.</param>
    /// <param name="loginHandler">The login handler.</param>
    /// <param name="registerValidator">The register validator.</param>
    /// <param name="verifyOtpValidator">The verify OTP validator.</param>
    /// <param name="loginValidator">The login validator.</param>
    /// <param name="logger">The logger.</param>
    public AuthController(
        RegisterHandler registerHandler,
        VerifyOtpHandler verifyOtpHandler,
        LoginHandler loginHandler,
        IValidator<RegisterCommand> registerValidator,
        IValidator<VerifyOtpCommand> verifyOtpValidator,
        IValidator<LoginQuery> loginValidator,
        ILogger<AuthController> logger)
    {
        this.registerHandler = registerHandler;
        this.verifyOtpHandler = verifyOtpHandler;
        this.loginHandler = loginHandler;
        this.registerValidator = registerValidator;
        this.verifyOtpValidator = verifyOtpValidator;
        this.loginValidator = loginValidator;
        this.logger = logger;
    }

    /// <summary>
    /// Register a new user.
    /// </summary>
    /// <param name="command">The registration command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The registration result.</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await this.registerValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return this.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        try
        {
            var result = await this.registerHandler.Handle(command, cancellationToken);
            return this.CreatedAtAction(nameof(this.Register), new { id = result.RegistrationId }, result);
        }
        catch (InvalidOperationException ex)
        {
            return this.Conflict(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Verify OTP for user registration.
    /// </summary>
    /// <param name="command">The OTP verification command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>OK if successful.</returns>
    [HttpPost("otp/verify")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyOtp(
        [FromBody] VerifyOtpCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await this.verifyOtpValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return this.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        try
        {
            await this.verifyOtpHandler.Handle(command, cancellationToken);
            return this.Ok(new { Message = "OTP verified successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return this.BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Login with phone/email and password.
    /// </summary>
    /// <param name="query">The login query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The login result with JWT token.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await this.loginValidator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            return this.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        try
        {
            var result = await this.loginHandler.Handle(query, cancellationToken);
            return this.Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return this.Unauthorized(new { Message = ex.Message });
        }
    }
}
