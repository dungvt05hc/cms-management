// <copyright file="UpdateAddressCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Addresses.UpdateAddress;

/// <summary>
/// Command to update an address.
/// </summary>
/// <param name="AddressId">The address ID.</param>
/// <param name="UserId">The user ID (for ownership check).</param>
/// <param name="FullName">Recipient full name.</param>
/// <param name="Phone">Recipient phone number.</param>
/// <param name="AddressLine">Address line.</param>
/// <param name="Ward">Ward/commune.</param>
/// <param name="District">District.</param>
/// <param name="City">City/province.</param>
public record UpdateAddressCommand(
    Guid AddressId,
    Guid UserId,
    string FullName,
    string Phone,
    string AddressLine,
    string Ward,
    string District,
    string City);
