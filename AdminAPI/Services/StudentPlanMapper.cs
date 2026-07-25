using AdminAPI.DTOs.StudentPlan;
using AdminAPI.Models;

namespace AdminAPI.Services;

public static class StudentPlanMapper
{
    public static List<StudentPlanListOptionDto> MapPlanOptions(IEnumerable<StudentPlan> plans) =>
        plans.Select(p => new StudentPlanListOptionDto
        {
            Id = p.Id,
            Display = p.Name + " (" + p.PlanFromDate.ToString("yyyy-MM-dd") + " - " + p.PlanToDate.ToString("yyyy-MM-dd") + ")",
        }).ToList();

    public static List<StudentPlanItemDto> MapItems(
        IEnumerable<StudentPlanMemorizing> mem,
        IEnumerable<StudentPlanRevise> rev)
    {
        var memItems = mem.Select(x => new StudentPlanItemDto
        {
            Key = "memorizing_" + x.Id,
            PlanType = StudentPlanConstants.TypeMemorizing,
            MemorizationLevel = x.MemorizationLevel,
            SurahId = x.SurahId,
            SurahName = x.QuranSurah?.NameAr ?? "—",
            FromAyahNumber = x.FromAyahNumber,
            ToAyahNumber = x.ToAyahNumber,
            PlanDateFormatted = x.PlanDate.ToString("yyyy-MM-dd"),
        });

        var revItems = rev.Select(x => new StudentPlanItemDto
        {
            Key = "revise_" + x.Id,
            PlanType = StudentPlanConstants.TypeRevise,
            MemorizationLevel = x.MemorizationLevel,
            SurahId = x.SurahId,
            SurahName = x.QuranSurah?.NameAr ?? "—",
            FromAyahNumber = x.FromAyahNumber,
            ToAyahNumber = x.ToAyahNumber,
            PlanDateFormatted = x.PlanDate.ToString("yyyy-MM-dd"),
        });

        return memItems.Concat(revItems).OrderByDescending(x => x.PlanDateFormatted).ToList();
    }

    public static StudentPlanHeaderDto BuildHeader(
        IEnumerable<StudentPlanMemorizing> mem,
        IEnumerable<StudentPlanRevise> rev,
        DateTime defaultDate)
    {
        var rows = mem.Select(x => new { x.PlanDate, PlanEndDate = x.PlanEndDate ?? x.PlanDate, x.MemorizationLevel })
            .Concat(rev.Select(x => new { x.PlanDate, PlanEndDate = x.PlanEndDate ?? x.PlanDate, x.MemorizationLevel }))
            .ToList();

        if (rows.Count == 0)
        {
            var today = defaultDate.ToString("yyyy-MM-dd");
            return new StudentPlanHeaderDto
            {
                MemorizationLevel = StudentPlanConstants.DefaultLevel,
                PlanStartDate = today,
                PlanEndDate = today,
            };
        }

        var minDate = rows.Min(x => x.PlanDate);
        var maxDate = rows.Max(x => x.PlanEndDate);
        var latest = rows.OrderByDescending(x => x.PlanDate).First();

        return new StudentPlanHeaderDto
        {
            MemorizationLevel = latest.MemorizationLevel,
            PlanStartDate = minDate.ToString("yyyy-MM-dd"),
            PlanEndDate = maxDate.ToString("yyyy-MM-dd"),
        };
    }

    public static string GetStudentDisplayName(RegisterForm student) =>
        student.FullName ?? student.StudentName ?? "—";
}
