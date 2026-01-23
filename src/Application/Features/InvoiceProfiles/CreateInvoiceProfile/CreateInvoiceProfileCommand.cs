// <copyright file="CreateInvoiceProfileCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.InvoiceProfiles.CreateInvoiceProfile;

/// <summary>
/// Command to create a new invoice profile.
/// </summary>
/// <param name="UserId">The user ID.</param>
/// <param name="TaxCode">The tax code (MST).</param>
/// <param name="CompanyName">The company name.</param>
/// <param name="CompanyAddress">The company address.</param>
/// <param name="Email">The invoice email.</param>
public record CreateInvoiceProfileCommand(
    Guid UserId,
    string TaxCode,
    string CompanyName,
    string CompanyAddress,
    string Email);
