using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Entities;
using MasgedTeacherMobileAPI.Enums;
using Microsoft.EntityFrameworkCore;

namespace MasgedTeacherMobileAPI.Helpers;

public static class WhatsappMessageHelper
{
    public static string FormatMessage(
        string template,
        int? studentId = null,
        string? studentName = null,
        string? fatherName = null,
        string? circleName = null,
        string? teacherName = null,
        DateTime? date = null,
        DateTime? time = null)
    {
        if (string.IsNullOrEmpty(template))
            return string.Empty;

        var formattedMessage = template;

        if (studentId.HasValue)
            formattedMessage = formattedMessage.Replace("{رقم الطالب}", studentId.Value.ToString());

        if (!string.IsNullOrEmpty(studentName))
            formattedMessage = formattedMessage.Replace("{اسم الطالب}", studentName);

        if (!string.IsNullOrEmpty(fatherName))
            formattedMessage = formattedMessage.Replace("{اسم الأب}", fatherName);

        if (!string.IsNullOrEmpty(circleName))
            formattedMessage = formattedMessage.Replace("{اسم الحلقة}", circleName);

        if (!string.IsNullOrEmpty(teacherName))
            formattedMessage = formattedMessage.Replace("{اسم المعلم}", teacherName);

        var dateTime = date ?? KuwaitTime.Now;
        var timeValue = time ?? KuwaitTime.Now;

        formattedMessage = formattedMessage.Replace("{التاريخ}", dateTime.ToString("dd-MM-yyyy"));
        formattedMessage = formattedMessage.Replace("{الوقت}", timeValue.ToString("hh:mm tt"));

        return formattedMessage;
    }

    public static string FormatMessage(string template, IDictionary<string, string> tokens)
    {
        if (string.IsNullOrEmpty(template))
            return string.Empty;

        var formatted = template;
        foreach (var token in tokens)
        {
            if (string.IsNullOrEmpty(token.Key))
                continue;
            formatted = formatted.Replace("{" + token.Key + "}", token.Value ?? string.Empty);
        }

        return formatted;
    }

    public static async Task<string?> GetFormattedMessageAsync(
        AppDbContext db,
        WhatsappMessageEvent eventType,
        RegisterForm student,
        string? circleName = null,
        string? teacherName = null,
        CancellationToken cancellationToken = default)
    {
        if (student is null)
            return null;

        var config = await db.WhatsappPreConfiguredMessages
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Event == eventType.ToString(), cancellationToken);

        if (config is null || !config.IsEnabled)
            return null;

        return FormatMessage(
            config.WhatsappMessage,
            student.Id,
            student.StudentName,
            student.FatherName,
            circleName,
            teacherName,
            KuwaitTime.Now,
            KuwaitTime.Now);
    }

    public static async Task<string?> GetReviseFormattedMessageAsync(
        AppDbContext db,
        RegisterForm student,
        string? circleName,
        string? teacherName,
        string reviseType,
        string? surahName,
        string testFrom,
        string testTo,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        if (student is null)
            return null;

        var config = await db.WhatsappPreConfiguredMessages
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Event == WhatsappMessageEvent.StudentRevise.ToString(), cancellationToken);

        if (config is null || !config.IsEnabled)
            return null;

        var formatted = FormatMessage(
            config.WhatsappMessage,
            student.Id,
            student.StudentName,
            student.FatherName,
            circleName,
            teacherName,
            KuwaitTime.Now,
            KuwaitTime.Now);

        formatted = formatted.Replace("{نوع المراجعة}", reviseType);
        if (!string.IsNullOrEmpty(surahName))
            formatted = formatted.Replace("{اسم السورة}", surahName);
        formatted = formatted.Replace("{من}", testFrom);
        formatted = formatted.Replace("{إلى}", testTo);
        if (!string.IsNullOrEmpty(notes))
            formatted = formatted.Replace("{ملاحظات}", notes);

        return formatted;
    }

    public static async Task<string?> GetTestFormattedMessageAsync(
        AppDbContext db,
        RegisterForm student,
        string? circleName,
        string? teacherName,
        string? surahName,
        string testFrom,
        string testTo,
        string? notes,
        DateTime testDate,
        string? hezbNumber,
        decimal? memorizationScore,
        decimal? tajweedScore,
        decimal? performanceScore,
        decimal? totalScore,
        string? grade,
        decimal finalResult,
        CancellationToken cancellationToken = default)
    {
        if (student is null)
            return null;

        var config = await db.WhatsappPreConfiguredMessages
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Event == WhatsappMessageEvent.StudentTest.ToString(), cancellationToken);

        if (config is null || !config.IsEnabled)
            return null;

        var formatted = FormatMessage(
            config.WhatsappMessage,
            student.Id,
            student.StudentName,
            student.FatherName,
            circleName,
            teacherName,
            KuwaitTime.Now,
            KuwaitTime.Now);

        if (!string.IsNullOrEmpty(surahName))
            formatted = formatted.Replace("{اسم السورة}", surahName);
        formatted = formatted.Replace("{من}", testFrom);
        formatted = formatted.Replace("{إلى}", testTo);
        if (!string.IsNullOrEmpty(notes))
            formatted = formatted.Replace("{ملاحظات}", notes);

        formatted = formatted.Replace("{تاريخ الاختبار}", testDate.ToString("dd-MM-yyyy"));

        if (!string.IsNullOrEmpty(hezbNumber))
            formatted = formatted.Replace("{حزب رقم}", hezbNumber);
        if (memorizationScore.HasValue)
            formatted = formatted.Replace("{درجة الحفظ}", memorizationScore.Value.ToString("N0"));
        if (tajweedScore.HasValue)
            formatted = formatted.Replace("{درجة التجويد}", tajweedScore.Value.ToString("N0"));
        if (performanceScore.HasValue)
            formatted = formatted.Replace("{درجة الأداء}", performanceScore.Value.ToString("N0"));
        if (totalScore.HasValue)
            formatted = formatted.Replace("{المجموع}", totalScore.Value.ToString("N0"));
        if (!string.IsNullOrEmpty(grade))
            formatted = formatted.Replace("{التقدير}", grade);

        formatted = formatted.Replace("{النتيجة النهائية}", finalResult.ToString("N0"));

        return formatted;
    }
}
