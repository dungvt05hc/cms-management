// <copyright file="GetAddressesQuery.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Addresses.GetAddresses;

/// <summary>
/// Query to get addresses for a user.
/// </summary>
/// <param name="UserId">The user ID.</param>
public record GetAddressesQuery(Guid UserId);
