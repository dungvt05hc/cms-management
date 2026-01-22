// <copyright file="AddressDto.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Addresses;

/// <summary>
/// DTO for address.
/// </summary>
/// <param name="Id">Address ID.</param>
/// <param name="UserId">User ID.</param>
/// <param name="FullName">Recipient full name.</param>
/// <param name="Phone">Recipient phone number.</param>
/// <param name="AddressLine">Address line.</param>
/// <param name="Ward">Ward/commune.</param>
/// <param name="District">District.</param>
/// <param name="City">City/province.</param>
/// <param name="IsDefault">Whether this is the default address.</param>
/// <param name="CreatedAt">Created timestamp.</param>
/// <param name="UpdatedAt">Updated timestamp.</param>
public record AddressDto(
    Guid Id,
    Guid UserId,
    string FullName,
    string Phone,
    string AddressLine,
    string Ward,
    string District,
    string City,
    bool IsDefault,
    DateTime CreatedAt,
    DateTime UpdatedAt);
