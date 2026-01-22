// <copyright file="StubPaymentGateway.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Application.Abstractions;

namespace Infrastructure.Services;

/// <summary>
/// Stub implementation of payment gateway for development/testing.
/// </summary>
public class StubPaymentGateway : IPaymentGateway
{
    /// <inheritdoc/>
    public Task<(string PaymentUrl, string PaymentReference)> InitiatePaymentAsync(
        decimal amount,
        string orderReference,
        CancellationToken cancellationToken = default)
    {
        var paymentReference = $"PAYOO_{Guid.NewGuid():N}";
        var paymentUrl = $"https://payment.stub.local/pay?ref={paymentReference}&amount={amount}";

        return Task.FromResult((paymentUrl, paymentReference));
    }

    /// <inheritdoc/>
    public Task<bool> VerifyPaymentAsync(string paymentReference, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(paymentReference.StartsWith("PAYOO_"));
    }
}
