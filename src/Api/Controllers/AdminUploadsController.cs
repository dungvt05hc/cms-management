// <copyright file="AdminUploadsController.cs" company="CMS Management">
// Copyright (c) CMS Management. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Admin uploads controller for handling file uploads.
/// </summary>
[ApiController]
[Route("admin/uploads")]
[Authorize(Policy = "AdminOnly")]
public class AdminUploadsController : ControllerBase
{
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    private readonly IWebHostEnvironment environment;
    private readonly ILogger<AdminUploadsController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminUploadsController"/> class.
    /// </summary>
    /// <param name="environment">The web host environment.</param>
    /// <param name="logger">The logger.</param>
    public AdminUploadsController(
        IWebHostEnvironment environment,
        ILogger<AdminUploadsController> logger)
    {
        this.environment = environment;
        this.logger = logger;
    }

    /// <summary>
    /// Upload a product image.
    /// </summary>
    /// <param name="file">The image file to upload.</param>
    /// <returns>The URL of the uploaded image.</returns>
    [HttpPost("image")]
    [ProducesResponseType(typeof(UploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [RequestSizeLimit(MaxFileSize)]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return this.BadRequest(new { Message = "No file provided" });
        }

        // Validate file size
        if (file.Length > MaxFileSize)
        {
            return this.BadRequest(new { Message = "File size exceeds 5MB limit" });
        }

        // Validate file extension
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            return this.BadRequest(new { Message = $"Invalid file type. Allowed types: {string.Join(", ", AllowedExtensions)}" });
        }

        // Validate content type
        var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        if (!allowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
        {
            return this.BadRequest(new { Message = "Invalid content type" });
        }

        try
        {
            // Create uploads directory if it doesn't exist
            var uploadsFolder = Path.Combine(this.environment.WebRootPath ?? Path.Combine(this.environment.ContentRootPath, "wwwroot"), "uploads", "products");
            Directory.CreateDirectory(uploadsFolder);

            // Generate unique filename
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Generate URL
            var request = this.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var imageUrl = $"{baseUrl}/uploads/products/{uniqueFileName}";

            this.logger.LogInformation("Image uploaded: {FileName} -> {ImageUrl}", file.FileName, imageUrl);

            return this.Ok(new UploadResponse { Url = imageUrl, FileName = uniqueFileName });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error uploading image: {FileName}", file.FileName);
            return this.StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error uploading file" });
        }
    }

    /// <summary>
    /// Delete an uploaded image.
    /// </summary>
    /// <param name="fileName">The filename to delete.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("image/{fileName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteImage(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return this.BadRequest(new { Message = "Filename is required" });
        }

        // Validate filename to prevent path traversal
        if (fileName.Contains("..") || fileName.Contains('/') || fileName.Contains('\\'))
        {
            return this.BadRequest(new { Message = "Invalid filename" });
        }

        var uploadsFolder = Path.Combine(this.environment.WebRootPath ?? Path.Combine(this.environment.ContentRootPath, "wwwroot"), "uploads", "products");
        var filePath = Path.Combine(uploadsFolder, fileName);

        if (!System.IO.File.Exists(filePath))
        {
            return this.NotFound(new { Message = "File not found" });
        }

        try
        {
            System.IO.File.Delete(filePath);
            this.logger.LogInformation("Image deleted: {FileName}", fileName);
            return this.NoContent();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error deleting image: {FileName}", fileName);
            return this.StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error deleting file" });
        }
    }
}
