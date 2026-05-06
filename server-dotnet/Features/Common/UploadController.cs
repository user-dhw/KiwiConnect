using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfoExchange.Api.Features.Common;

[ApiController]
public sealed class UploadController : ControllerBase
{
  private const long MaxFileSize = 5 * 1024 * 1024;
  private static readonly Dictionary<string, string[]> AllowedFileTypes = new(StringComparer.OrdinalIgnoreCase)
  {
    [".jpg"] = ["image/jpeg"],
    [".jpeg"] = ["image/jpeg"],
    [".png"] = ["image/png"],
    [".webp"] = ["image/webp"]
  };

  private readonly IWebHostEnvironment _environment;
  private readonly IConfiguration _configuration;

  public UploadController(IWebHostEnvironment environment, IConfiguration configuration)
  {
    _environment = environment;
    _configuration = configuration;
  }

  [HttpPost("/uplod")]
  [AllowAnonymous]
  [RequestSizeLimit(MaxFileSize)]
  public async Task<IActionResult> Upload([FromForm] IFormFile file)
  {
    if (file is null || file.Length == 0)
    {
      return BadRequest("No file uploaded");
    }

    if (file.Length > MaxFileSize)
    {
      return BadRequest("File size cannot exceed 5 MB");
    }

    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
    if (string.IsNullOrWhiteSpace(extension) || !AllowedFileTypes.ContainsKey(extension))
    {
      return BadRequest("Only JPG, JPEG, PNG, and WEBP files are allowed");
    }

    if (!AllowedFileTypes[extension].Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
    {
      return BadRequest("Invalid file content type");
    }

    await using var fileStream = file.OpenReadStream();
    if (!await IsSupportedImageAsync(fileStream, extension))
    {
      return BadRequest("File content does not match the allowed image format");
    }

    var uploadRoot = _configuration["FileStorage:UploadRoot"] ?? "uplodes";
    var absoluteUploadRoot = Path.Combine(_environment.ContentRootPath, uploadRoot);
    Directory.CreateDirectory(absoluteUploadRoot);

    var fileName = $"{Guid.NewGuid():N}{extension}";
    var fullPath = Path.Combine(absoluteUploadRoot, fileName);

    fileStream.Position = 0;
    await using (var stream = System.IO.File.Create(fullPath))
    {
      await fileStream.CopyToAsync(stream);
    }

    var url = $"{Request.Scheme}://{Request.Host}/uplodes/{fileName}";
    return Ok(new
    {
      StoredFileName = fileName,
      OriginalFileName = file.FileName,
      Length = file.Length,
      ContentType = file.ContentType,
      Url = url
    });
  }

  private static async Task<bool> IsSupportedImageAsync(Stream stream, string extension)
  {
    var header = new byte[12];
    var bytesRead = await stream.ReadAsync(header, 0, header.Length);

    return extension switch
    {
      ".jpg" or ".jpeg" => bytesRead >= 3
        && header[0] == 0xFF
        && header[1] == 0xD8
        && header[2] == 0xFF,
      ".png" => bytesRead >= 8
        && header[0] == 0x89
        && header[1] == 0x50
        && header[2] == 0x4E
        && header[3] == 0x47
        && header[4] == 0x0D
        && header[5] == 0x0A
        && header[6] == 0x1A
        && header[7] == 0x0A,
      ".webp" => bytesRead >= 12
        && header[0] == 0x52
        && header[1] == 0x49
        && header[2] == 0x46
        && header[3] == 0x46
        && header[8] == 0x57
        && header[9] == 0x45
        && header[10] == 0x42
        && header[11] == 0x50,
      _ => false
    };
  }
}
