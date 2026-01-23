// <copyright file="GetInvoiceProfilesQuery.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.InvoiceProfiles.GetInvoiceProfiles;

/// <summary>
/// Query to get invoice profiles for a user.
/// </summary>
/// <param name="UserId">The user ID.</param>
public record GetInvoiceProfilesQuery(Guid UserId);
