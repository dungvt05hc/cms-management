// <copyright file="CreateStaffResult.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Staff.CreateStaff;

/// <summary>
/// Result of creating a staff user.
/// </summary>
/// <param name="StaffId">The created staff user ID.</param>
/// <param name="Email">Email address.</param>
/// <param name="FullName">Full name.</param>
/// <param name="Role">Role of the staff user.</param>
public record CreateStaffResult(Guid StaffId, string Email, string FullName, string Role);
