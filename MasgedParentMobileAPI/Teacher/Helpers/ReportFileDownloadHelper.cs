using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace MasgedTeacherMobileAPI.Helpers;

public static class ReportFileDownloadHelper
{
    /// <summary>
    /// ASCII filenames are required for reliable downloads on iOS/mobile clients.
    /// </summary>
    public static FileContentResult Create(byte[] bytes, string contentType, string asciiFileName) =>
        new(bytes, contentType) { FileDownloadName = asciiFileName };

    public static string BuildCircleMemorizationReportFileName(string extension)
    {
        var stamp = KuwaitTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
        return $"circle_memorization_report_{stamp}.{extension.TrimStart('.')}";
    }

    public static string BuildTestCertificateFileName(int testId) =>
        $"test_certificate_{testId}_{KuwaitTime.Now:yyyyMMdd_HHmmss}.pdf";
}
