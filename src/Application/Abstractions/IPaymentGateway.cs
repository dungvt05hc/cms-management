// <copyright file="IPaymentGateway.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Abstractions;

/// <summary>
/// Interface for payment gateway operations.
/// </summary>
public interface IPaymentGateway
{
    /// <summary>
    /// Initiates a payment and returns a payment URL.
    /// </summary>
    /// <param name="amount">The amount to pay.</param>
    /// <param name="orderReference">The internal order reference.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple containing payment URL and payment reference.</returns>
    Task<(string PaymentUrl, string PaymentReference)> InitiatePaymentAsync(
        decimal amount,
        string orderReference,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies a payment callback from the gateway.
    /// </summary>
    /// <param name="paymentReference">The payment reference from callback.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if payment is successful and verified, false otherwise.</returns>
    Task<bool> VerifyPaymentAsync(string paymentReference, CancellationToken cancellationToken = default);
}
