// <copyright file="AdminAuthController.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Features.AdminAuth.Login;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Admin authentication controller.
/// </summary>
[ApiController]
[Route("admin/auth")]
public class AdminAuthController : ControllerBase
{
    private readonly AdminLoginHandler loginHandler;
    private readonly IValidator<AdminLoginCommand> loginValidator;
    private readonly ILogger<AdminAuthController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminAuthController"/> class.
    /// </summary>
    /// <param name="loginHandler">The admin login handler.</param>
    /// <param name="loginValidator">The login validator.</param>
    /// <param name="logger">The logger.</param>
    public AdminAuthController(
        AdminLoginHandler loginHandler,
        IValidator<AdminLoginCommand> loginValidator,
        ILogger<AdminAuthController> logger)
    {
        this.loginHandler = loginHandler;
        this.loginValidator = loginValidator;
        this.logger = logger;
    }

    /// <summary>
    /// Login as staff/admin user.
    /// </summary>
    /// <param name="command">The login command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The login result with JWT token.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AdminLoginResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] AdminLoginCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await this.loginValidator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return this.BadRequest(new { Errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        try
        {
            var result = await this.loginHandler.Handle(command, cancellationToken);
            return this.Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return this.Unauthorized(new { Message = ex.Message });
        }
    }
}
