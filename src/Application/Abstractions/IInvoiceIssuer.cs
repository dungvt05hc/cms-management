// <copyright file="IInvoiceIssuer.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Abstractions;

/// <summary>
/// Interface for VAT e-invoice issuer service.
/// </summary>
public interface IInvoiceIssuer
{
    /// <summary>
    /// Issues an invoice for an order.
    /// </summary>
    /// <param name="orderId">The order ID.</param>
    /// <param name="taxCode">The tax code (MST).</param>
    /// <param name="companyName">The company name.</param>
    /// <param name="companyAddress">The company address.</param>
    /// <param name="email">The invoice email.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task IssueInvoiceAsync(
        Guid orderId,
        string taxCode,
        string companyName,
        string companyAddress,
        string email,
        CancellationToken cancellationToken = default);
}
