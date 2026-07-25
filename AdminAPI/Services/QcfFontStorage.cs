namespace AdminAPI.Services;

/// <summary>
/// Read-only static hosting for QCF Quran page fonts (mobile app on-demand download).
/// Files: static/qcf-fonts/p1.woff … p604.woff — synced by publish-all.ps1 from ParentApp.
/// </summary>
public static class QcfFontStorage
{
    public const string DirectoryName = "static/qcf-fonts";
    public const string RequestPath = "/static/qcf-fonts";
}
