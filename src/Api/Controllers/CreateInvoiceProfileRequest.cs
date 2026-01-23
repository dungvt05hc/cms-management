// <copyright file="CreateInvoiceProfileRequest.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Api.Controllers;

/// <summary>
/// Request to create an invoice profile.
/// </summary>
/// <param name="TaxCode">The tax code (MST).</param>
/// <param name="CompanyName">The company name.</param>
/// <param name="CompanyAddress">The company address.</param>
/// <param name="Email">The invoice email.</param>
public record CreateInvoiceProfileRequest(
    string TaxCode,
    string CompanyName,
    string CompanyAddress,
    string Email);
