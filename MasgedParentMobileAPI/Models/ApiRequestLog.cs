using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasgedParentMobileAPI.Models;

/// <summary>
/// Persists unified API request/response audit rows (middleware).
/// Table must exist (see Scripts/CreateApiRequestLogs.sql).
/// </summary>
[Table("ApiRequestLogs")]
public class ApiRequestLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public DateTime RequestedAt { get; set; }

    [MaxLength(10)]
    public string Method { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Path { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? QueryString { get; set; }

    public string? RequestHeaders { get; set; }

    public string? RequestBody { get; set; }

    public int ResponseStatusCode { get; set; }

    public string? ResponseBody { get; set; }

    public int DurationMs { get; set; }

    [MaxLength(64)]
    public string? ClientIp { get; set; }

    [MaxLength(100)]
    public string? UserId { get; set; }

    [MaxLength(200)]
    public string? UserName { get; set; }
}
