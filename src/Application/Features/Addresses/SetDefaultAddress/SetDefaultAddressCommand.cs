// <copyright file="SetDefaultAddressCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Addresses.SetDefaultAddress;

/// <summary>
/// Command to set an address as default.
/// </summary>
/// <param name="AddressId">The address ID.</param>
/// <param name="UserId">The user ID (for ownership check).</param>
public record SetDefaultAddressCommand(Guid AddressId, Guid UserId);
