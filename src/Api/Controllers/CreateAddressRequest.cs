// <copyright file="CreateAddressRequest.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Api.Controllers;

/// <summary>
/// Request to create an address.
/// </summary>
/// <param name="FullName">Recipient full name.</param>
/// <param name="Phone">Recipient phone number.</param>
/// <param name="AddressLine">Address line.</param>
/// <param name="Ward">Ward/commune.</param>
/// <param name="District">District.</param>
/// <param name="City">City/province.</param>
/// <param name="IsDefault">Whether this is the default address.</param>
public record CreateAddressRequest(
    string FullName,
    string Phone,
    string AddressLine,
    string Ward,
    string District,
    string City,
    bool IsDefault);
