// <copyright file="ShippingMethodDto.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Shipping;

/// <summary>
/// DTO for shipping method.
/// </summary>
/// <param name="Id">The method ID.</param>
/// <param name="Code">The method code.</param>
/// <param name="Name">The method name.</param>
/// <param name="Description">The description.</param>
/// <param name="Carriers">List of carriers.</param>
public record ShippingMethodDto(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    List<ShippingCarrierDto> Carriers);
