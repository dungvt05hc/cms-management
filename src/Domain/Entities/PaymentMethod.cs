// <copyright file="PaymentMethod.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Domain.Entities;

/// <summary>
/// Payment method enumeration.
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// Cash on delivery.
    /// </summary>
    COD = 0,

    /// <summary>
    /// Payoo online payment.
    /// </summary>
    Payoo = 1,
}
