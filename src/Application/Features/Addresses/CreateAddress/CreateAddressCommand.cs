// <copyright file="CreateAddressCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Addresses.CreateAddress;

/// <summary>
/// Command to create a new address.
/// </summary>
/// <param name="UserId">The user ID.</param>
/// <param name="FullName">Recipient full name.</param>
/// <param name="Phone">Recipient phone number.</param>
/// <param name="AddressLine">Address line.</param>
/// <param name="Ward">Ward/commune.</param>
/// <param name="District">District.</param>
/// <param name="City">City/province.</param>
/// <param name="IsDefault">Whether this is the default address.</param>
public record CreateAddressCommand(
    Guid UserId,
    string FullName,
    string Phone,
    string AddressLine,
    string Ward,
    string District,
    string City,
    bool IsDefault);
