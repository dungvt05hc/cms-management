// <copyright file="CartDto.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Cart;

/// <summary>
/// Cart data transfer object.
/// </summary>
/// <param name="Id">Cart ID.</param>
/// <param name="UserId">User ID.</param>
/// <param name="Items">Cart items.</param>
/// <param name="Subtotal">Subtotal for selected items.</param>
public record CartDto(
    Guid Id,
    Guid UserId,
    List<CartItemDto> Items,
    decimal Subtotal);
