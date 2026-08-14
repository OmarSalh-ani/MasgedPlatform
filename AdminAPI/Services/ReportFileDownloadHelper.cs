using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Services;

public static class ReportFileDownloadHelper
{
    public static FileContentResult Create(byte[] bytes, string contentType, string asciiFileName) =>
        new(bytes, contentType) { FileDownloadName = asciiFileName };

    public static string BuildCircleMemorizationReportFileName(string extension)
    {
        var stamp = KuwaitTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
        return $"circle_memorization_report_{stamp}.{extension.TrimStart('.')}";
    }
}
