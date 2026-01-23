// <copyright file="StubInvoiceIssuer.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Stub implementation of IInvoiceIssuer for testing and MVP.
/// </summary>
public class StubInvoiceIssuer : IInvoiceIssuer
{
    private readonly ILogger<StubInvoiceIssuer> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StubInvoiceIssuer"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public StubInvoiceIssuer(ILogger<StubInvoiceIssuer> logger)
    {
        this.logger = logger;
    }

    /// <inheritdoc/>
    public Task IssueInvoiceAsync(
        Guid orderId,
        string taxCode,
        string companyName,
        string companyAddress,
        string email,
        CancellationToken cancellationToken = default)
    {
        this.logger.LogInformation(
            "Stub: Issuing invoice for order {OrderId}",
            orderId);

        return Task.CompletedTask;
    }
}
