// <copyright file="CreateStaffCommand.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Application.Features.Staff.CreateStaff;

/// <summary>
/// Command to create a staff user.
/// </summary>
/// <param name="Email">Email address.</param>
/// <param name="Phone">Phone number (optional).</param>
/// <param name="FullName">Full name.</param>
/// <param name="Password">Password.</param>
/// <param name="Role">Role of the staff user (Admin or Staff).</param>
public record CreateStaffCommand(
    string Email,
    string? Phone,
    string FullName,
    string Password,
    string Role);
