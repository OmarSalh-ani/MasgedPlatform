namespace AdminAPI.DTOs.CircleMemorizationRevisionReport;

public class CircleMemorizationRevisionReportRowDto
{
    public int Sequence { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string DayName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string NewMemorization { get; set; } = string.Empty;
    public string Revision { get; set; } = string.Empty;
}

public class CircleMemorizationRevisionReportMetaDto
{
    public string MosqueName { get; set; } = "مسجد الشيخ مبارك عبد الله المبارك الصباح";
    public string CircleName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public DateTime PrintedAt { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public List<CircleMemorizationRevisionReportRowDto> Rows { get; set; } = [];
}

public class CircleMemorizationTeacherOptionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CirclePlanSegmentDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public DateTime PlanDate { get; set; }

    /// <summary>Day the row was actually assessed; null for rows recorded before completion dates were stamped.</summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>Day the report groups the row under.</summary>
    public DateTime EffectiveDate => (CompletedDate ?? PlanDate).Date;

    public int SurahId { get; set; }
    public string SurahName { get; set; } = string.Empty;
    public int FromAyah { get; set; }
    public int ToAyah { get; set; }
}
