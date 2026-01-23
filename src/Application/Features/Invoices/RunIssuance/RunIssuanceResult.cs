// <copyright file="RunIssuanceResult.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Invoices.RunIssuance;

/// <summary>
/// Result of running invoice issuance.
/// </summary>
/// <param name="ProcessedCount">Number of orders processed.</param>
public record RunIssuanceResult(int ProcessedCount);
