// <copyright file="ProductCursor.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using System.Text;
using System.Text.Json;

namespace Application.Features.Products.GetProducts;

/// <summary>
/// Helper for encoding/decoding product list cursors.
/// </summary>
public static class ProductCursor
{
    /// <summary>
    /// Encodes cursor data to a base64 string.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="price">The price (for sort stability).</param>
    /// <param name="createdAt">The created timestamp (for default sort).</param>
    /// <returns>The encoded cursor.</returns>
    public static string Encode(Guid productId, decimal price, DateTime createdAt)
    {
        var cursorData = new CursorData(productId, price, createdAt);
        var json = JsonSerializer.Serialize(cursorData);
        var bytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Decodes a cursor string.
    /// </summary>
    /// <param name="cursor">The cursor string.</param>
    /// <returns>The decoded cursor data, or null if invalid.</returns>
    public static CursorData? Decode(string cursor)
    {
        try
        {
            var bytes = Convert.FromBase64String(cursor);
            var json = Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<CursorData>(json);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Cursor data structure.
    /// </summary>
    /// <param name="ProductId">The product ID.</param>
    /// <param name="Price">The price.</param>
    /// <param name="CreatedAt">The created timestamp.</param>
    public record CursorData(Guid ProductId, decimal Price, DateTime CreatedAt);
}
