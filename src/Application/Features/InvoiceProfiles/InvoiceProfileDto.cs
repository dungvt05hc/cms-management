// <copyright file="InvoiceProfileDto.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.InvoiceProfiles;

/// <summary>
/// DTO for invoice profile.
/// </summary>
/// <param name="Id">The invoice profile ID.</param>
/// <param name="UserId">The user ID.</param>
/// <param name="TaxCode">The tax code (MST).</param>
/// <param name="CompanyName">The company name.</param>
/// <param name="CompanyAddress">The company address.</param>
/// <param name="Email">The invoice email.</param>
/// <param name="CreatedAt">The creation date.</param>
/// <param name="UpdatedAt">The last update date.</param>
public record InvoiceProfileDto(
    Guid Id,
    Guid UserId,
    string TaxCode,
    string CompanyName,
    string CompanyAddress,
    string Email,
    DateTime CreatedAt,
    DateTime UpdatedAt);
