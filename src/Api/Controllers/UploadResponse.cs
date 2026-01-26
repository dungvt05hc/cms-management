// <copyright file="UploadResponse.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

namespace Api.Controllers;

/// <summary>
/// Response model for file upload.
/// </summary>
public class UploadResponse
{
    /// <summary>
    /// Gets or sets the URL of the uploaded file.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the filename of the uploaded file.
    /// </summary>
    public string FileName { get; set; } = string.Empty;
}
