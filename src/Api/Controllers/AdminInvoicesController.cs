// <copyright file="AdminInvoicesController.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Features.Invoices.RunIssuance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Admin invoices controller.
/// </summary>
[ApiController]
[Route("admin/invoices")]
[Authorize(Policy = "AdminOnly")]
public class AdminInvoicesController : ControllerBase
{
    private readonly RunIssuanceHandler runIssuanceHandler;
    private readonly ILogger<AdminInvoicesController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminInvoicesController"/> class.
    /// </summary>
    /// <param name="runIssuanceHandler">The run issuance handler.</param>
    /// <param name="logger">The logger.</param>
    public AdminInvoicesController(
        RunIssuanceHandler runIssuanceHandler,
        ILogger<AdminInvoicesController> logger)
    {
        this.runIssuanceHandler = runIssuanceHandler;
        this.logger = logger;
    }

    /// <summary>
    /// Trigger invoice issuance for eligible orders (Delivered + 10 days).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result containing processed count.</returns>
    [HttpPost("run-issuance")]
    [ProducesResponseType(typeof(RunIssuanceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RunIssuance(CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Admin triggered invoice issuance");

        var command = new RunIssuanceCommand();
        var result = await this.runIssuanceHandler.Handle(command, cancellationToken);

        return this.Ok(result);
    }
}
