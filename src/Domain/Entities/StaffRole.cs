// <copyright file="StaffRole.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Domain.Entities;

/// <summary>
/// Enumeration of staff roles in the management system.
/// </summary>
public enum StaffRole
{
    /// <summary>
    /// Super Admin with full system access.
    /// </summary>
    SuperAdmin = 0,

    /// <summary>
    /// Admin with management capabilities.
    /// </summary>
    Admin = 1,

    /// <summary>
    /// Staff with limited operational capabilities.
    /// </summary>
    Staff = 2,
}
