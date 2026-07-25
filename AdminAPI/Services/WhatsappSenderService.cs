using AdminAPI.Data;
using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Home;
using AdminAPI.DTOs.WhatsappSender;
using AdminAPI.Models;
using AdminAPI.Services.Interfaces;
using FluentValidation;
using Masged.WhatsApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public class WhatsappSenderService(
    IHomeService homeService,
    AdminDbContext db,
    ICurrentUserContext currentUser,
    IOptions<PublicSiteOptions> publicSiteOptions) : IWhatsappSenderService
{
    public Task<PagedResultDto<HomeStudentListItemDto>> GetListAsync(
        HomeListFiltersDto filters,
        CancellationToken cancellationToken = default) =>
        homeService.GetListAsync(filters, cancellationToken);

    public Task<HomeFilterOptionsDto> GetFilterOptionsAsync(CancellationToken cancellationToken = default) =>
        homeService.GetFilterOptionsAsync(cancellationToken);

    public Task<List<WhatsappSenderFormOptionDto>> GetFormOptionsAsync(
        CancellationToken cancellationToken = default) =>
        db.DynamicForms
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Title)
            .Select(x => new WhatsappSenderFormOptionDto { Id = x.Id, Title = x.Title })
            .ToListAsync(cancellationToken);

    public async Task<string> SendWhatsappAsync(
        SendWhatsappSenderRequestDto request,
        string? base64Image,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.CanModify)
            throw new UnauthorizedAccessException("ليس لديك صلاحية لإرسال رسائل الواتساب");

        if (request.StudentIds.Count == 0)
            throw new ValidationException("يرجى تحديد الطلاب المراد إرسال الرسائل لهم");

        if (string.IsNullOrWhiteSpace(request.Message))
            throw new ValidationException("يرجى كتابة الرسالة أولاً");

        var students = await db.RegisterForms
            .AsNoTracking()
            .Where(x => request.StudentIds.Contains(x.Id))
            .Select(x => new
            {
                x.Id,
                StudentName = x.FullName != null && x.FullName != string.Empty ? x.FullName : x.StudentName,
                x.FatherName,
                x.FatherPhone,
                CircleName = x.QuranCircle != null ? x.QuranCircle.Name : string.Empty,
            })
            .Where(x => !PhoneNormalizer.ContainsArabicDigits(x.FatherPhone))
            .ToListAsync(cancellationToken);

        var baseUrl = publicSiteOptions.Value.BaseUrl.TrimEnd('/');
        var formTitle = string.Empty;
        string? formLinkBaseUrl = null;

        if (request.FormId.HasValue)
        {
            var form = await db.DynamicForms
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.FormId.Value && x.IsActive, cancellationToken);

            if (form != null)
            {
                formTitle = form.Title;
                formLinkBaseUrl = $"{baseUrl}/forms/{form.Id}";
            }
        }

        var isGirl = currentUser.IsGirlTeacher ? 1 : 0;
        foreach (var student in students)
        {
            var message = request.Message
                .Replace("{أسم الطالب}", student.StudentName ?? string.Empty)
                .Replace("{أسم الأب}", student.FatherName ?? string.Empty)
                .Replace("{أسم الحلقة}", student.CircleName ?? string.Empty)
                .Replace("{الرابط}", $"{baseUrl}/parents-followup?id={student.Id}")
                .Replace("{اسم النموذج}", formTitle);

            var formLinkWithStudent = string.IsNullOrEmpty(formLinkBaseUrl)
                ? string.Empty
                : $"{formLinkBaseUrl}?studentId={student.Id}";

            message = message
                .Replace("{رابط النموذج}", formLinkWithStudent)
                .Replace("{}", formLinkWithStudent);

            db.WhatsappTempTables.Add(new WhatsappTempTable
            {
                Image = base64Image,
                Message = message,
                Mobile = PhoneNormalizer.ToWhatsappE164(student.FatherPhone),
                IsGirl = isGirl,
            });
        }

        await db.SaveChangesAsync(cancellationToken);
        return "تم إرسال الرسائل بنجاح";
    }
}
