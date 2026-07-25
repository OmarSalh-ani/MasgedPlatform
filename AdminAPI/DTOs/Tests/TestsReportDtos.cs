namespace AdminAPI.DTOs.Tests;

public class TestsReportFilterOptionsDto
{
    public List<TestsReportCircleOptionDto> Circles { get; set; } = [];
}

public class TestsReportCircleOptionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class TestsReportListResponseDto
{
    public List<TestsReportRowDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class TestsReportRowDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string ParentPhone { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string CircleName { get; set; } = string.Empty;
    public string ProgramType { get; set; } = string.Empty;
    public string TestFrom { get; set; } = string.Empty;
    public string TestTo { get; set; } = string.Empty;
    public string TestDate { get; set; } = string.Empty;
    public decimal FinalResults { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string TestType { get; set; } = string.Empty;
}

public class TestsReportSourceRow
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string ParentPhone { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string CircleName { get; set; } = string.Empty;
    public string TestFrom { get; set; } = string.Empty;
    public string TestTo { get; set; } = string.Empty;
    public DateTime TestDate { get; set; }
    public decimal FinalResults { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string TestName { get; set; } = string.Empty;
}
