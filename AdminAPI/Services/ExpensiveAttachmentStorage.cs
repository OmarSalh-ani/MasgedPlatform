using System.Text.RegularExpressions;
using AdminAPI.DTOs.Expensives;

namespace AdminAPI.Services;

public static partial class ExpensiveAttachmentStorage
{
    public static string CreateSubfolderPath(string uploadDirectory)
    {
        var timestamp = KuwaitTime.Now.ToString("yyyyMMddHHmmss");
        var path = Path.Combine(uploadDirectory, timestamp);
        Directory.CreateDirectory(path);
        return path;
    }

    public static async Task SaveFilesAsync(
        IEnumerable<IFormFile> files,
        string folderPath,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(folderPath);

        foreach (var file in files)
        {
            if (file.Length <= 0)
                continue;

            var safeFileName = SanitizeFileName(Path.GetFileName(file.FileName));
            var filePath = Path.Combine(folderPath, safeFileName);

            await using var stream = File.Create(filePath);
            await file.CopyToAsync(stream, cancellationToken);
        }
    }

    public static List<ExpensiveAttachmentDto> ListFiles(string? folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
            return [];

        var dirInfo = new DirectoryInfo(folderPath);
        return dirInfo.GetFiles()
            .Select(file => new ExpensiveAttachmentDto
            {
                FileName = file.Name,
                UploadDate = file.CreationTime
            })
            .OrderByDescending(f => f.UploadDate)
            .ToList();
    }

    public static string? ResolveFilePath(string? folderPath, string fileName)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
            return null;

        var safeFileName = SanitizeFileName(fileName);
        var filePath = Path.Combine(folderPath, safeFileName);
        return File.Exists(filePath) ? filePath : null;
    }

    public static void DeleteFile(string? folderPath, string fileName)
    {
        var filePath = ResolveFilePath(folderPath, fileName);
        if (filePath is null)
            return;

        try
        {
            File.Delete(filePath);
            if (!string.IsNullOrWhiteSpace(folderPath)
                && Directory.Exists(folderPath)
                && !Directory.EnumerateFileSystemEntries(folderPath).Any())
            {
                Directory.Delete(folderPath);
            }
        }
        catch
        {
            // matches legacy WebForms behavior
        }
    }

    public static string SanitizeFileName(string fileName) =>
        MyRegex().Replace(fileName, "_");

    [GeneratedRegex(@"[^a-zA-Z0-9.-]")]
    private static partial Regex MyRegex();
}
