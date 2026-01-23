// <copyright file="RunIssuanceHandler.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Invoices.RunIssuance;

/// <summary>
/// Handler for RunIssuanceCommand.
/// </summary>
public class RunIssuanceHandler
{
    private readonly IAppDbContext dbContext;
    private readonly IInvoiceIssuer invoiceIssuer;
    private readonly ILogger<RunIssuanceHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RunIssuanceHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="invoiceIssuer">The invoice issuer.</param>
    /// <param name="logger">The logger.</param>
    public RunIssuanceHandler(
        IAppDbContext dbContext,
        IInvoiceIssuer invoiceIssuer,
        ILogger<RunIssuanceHandler> logger)
    {
        this.dbContext = dbContext;
        this.invoiceIssuer = invoiceIssuer;
        this.logger = logger;
    }

    /// <summary>
    /// Handles running invoice issuance for eligible orders.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result containing processed count.</returns>
    public async Task<RunIssuanceResult> Handle(
        RunIssuanceCommand command,
        CancellationToken cancellationToken)
    {
        var tenDaysAgo = DateTime.UtcNow.AddDays(-10);

        var eligibleOrders = await this.dbContext.Orders
            .Where(o =>
                o.Status == OrderStatus.Delivered &&
                o.InvoiceRequested &&
                o.InvoiceIssuedAt == null &&
                o.DeliveredAt != null &&
                o.DeliveredAt <= tenDaysAgo)
            .ToListAsync(cancellationToken);

        this.logger.LogInformation(
            "Found {Count} eligible orders for invoice issuance",
            eligibleOrders.Count);

        var processedCount = 0;

        foreach (var order in eligibleOrders)
        {
            try
            {
                await this.invoiceIssuer.IssueInvoiceAsync(
                    order.Id,
                    order.InvoiceTaxCode!,
                    order.InvoiceCompanyName!,
                    order.InvoiceCompanyAddress!,
                    order.InvoiceEmail!,
                    cancellationToken);

                order.InvoiceIssuedAt = DateTime.UtcNow;
                order.UpdatedAt = DateTime.UtcNow;
                processedCount++;

                this.logger.LogInformation(
                    "Issued invoice for order {OrderId}",
                    order.Id);
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    ex,
                    "Failed to issue invoice for order {OrderId}",
                    order.Id);
            }
        }

        if (processedCount > 0)
        {
            await this.dbContext.SaveChangesAsync(cancellationToken);
        }

        this.logger.LogInformation(
            "Processed {ProcessedCount} of {TotalCount} eligible orders",
            processedCount,
            eligibleOrders.Count);

        return new RunIssuanceResult(processedCount);
    }
}
