// <copyright file="ShippingCarrierDto.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Shipping;

/// <summary>
/// DTO for shipping carrier.
/// </summary>
/// <param name="Id">The carrier ID.</param>
/// <param name="Code">The carrier code.</param>
/// <param name="Name">The carrier name.</param>
/// <param name="Description">The description.</param>
/// <param name="SupportsCOD">Whether COD is supported.</param>
public record ShippingCarrierDto(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    bool SupportsCOD);
