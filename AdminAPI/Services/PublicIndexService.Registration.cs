using AdminAPI.DTOs.PublicIndex;
using AdminAPI.Enums;
using AdminAPI.Models;
using Masged.WhatsApp;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Services;

public partial class PublicIndexService
{
    private async Task<SubmitPublicRegistrationResponseDto> SaveRegistrationAsync(
        string mode,
        SubmitPublicRegistrationRequestDto request,
        CancellationToken cancellationToken)
    {
        var forGirl = mode == "wregister";
        var registrationEnabled = await registrationSettings.GetRegistrationEnabledAsync(forGirl, cancellationToken);
        if (!registrationEnabled)
            throw new InvalidOperationException("التسجيل مغلق حالياً");

        if (string.IsNullOrWhiteSpace(request.ParentPhoneCountryIso))
            throw new InvalidOperationException("يرجى اختيار رمز الدولة قبل إدخال رقم الجوال.");

        var activity = await db.WomanActivities
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.WomanActivityTypeId, cancellationToken)
            ?? throw new InvalidOperationException("نوع النشاط غير صالح");

        var isGirl = activity.ForGirl;
        var (age, birthDate) = ResolveAgeAndBirthdate(mode, request);

        var fatherPhone = PhoneNormalizer.ToCanonical(request.ParentPhone1);

        var fatherPhone2 = string.IsNullOrWhiteSpace(request.ParentPhone2)
            ? null
            : PhoneNormalizer.ToCanonical(request.ParentPhone2);

        var entity = new RegisterForm
        {
            Age = age,
            CreatedAt = KuwaitTime.Now,
            FatherPhone = fatherPhone,
            FatherPhone2 = fatherPhone2,
            StudentGender = isGirl ? "أنثى" : "ذكر",
            StudentName = request.FullName.Trim(),
            StudentPhone = string.Empty,
            QuranCircleId = null,
            FatherName = string.Empty,
            Birthdate = birthDate,
            FullName = request.FullName.Trim(),
            WomanActivityType = request.WomanActivityTypeId,
            LearnCertificate = request.LearnCertificate,
            IsGirl = isGirl ? 1 : 0,
        };

        db.RegisterForms.Add(entity);

        var adminTokens = new Dictionary<string, string>
        {
            ["اسم الطالب"] = request.FullName.Trim(),
            ["العمر"] = age.ToString(),
            ["جوال ولي الأمر"] = fatherPhone,
            ["جوال ولي الأمر البديل"] = fatherPhone2 ?? string.Empty,
            ["نوع النشاط"] = activity.Name,
            ["التاريخ"] = KuwaitTime.Now.ToString("dd-MM-yyyy"),
            ["الوقت"] = KuwaitTime.Now.ToString("hh:mm tt"),
        };

        var adminMessage = await WhatsappMessageFormatter.GetFormattedMessageAsync(
            db,
            WhatsappMessageEvent.RegistrationNewSubmission,
            adminTokens,
            cancellationToken);

        if (!string.IsNullOrEmpty(adminMessage))
        {
            db.WhatsappTempTables.Add(new WhatsappTempTable
            {
                Image = string.Empty,
                Message = adminMessage,
                Mobile = registrationOptions.Value.AdminNotificationMobile,
                IsGirl = isGirl ? 1 : 0,
            });
        }

        await db.SaveChangesAsync(cancellationToken);

        if (!isGirl)
        {
            var parentTokens = new Dictionary<string, string>
            {
                ["اسم الطالب"] = entity.StudentName,
                ["رقم الطالب"] = entity.Id.ToString(),
                ["رابط المتابعة"] = $"https://mosque-mbark-j.com/parents-followup?id={entity.Id}",
            };

            var parentMessage = await WhatsappMessageFormatter.GetFormattedMessageAsync(
                db,
                WhatsappMessageEvent.RegistrationParentFollowupLink,
                parentTokens,
                cancellationToken);

            if (!string.IsNullOrEmpty(parentMessage))
            {
                db.WhatsappTempTables.Add(new WhatsappTempTable
                {
                    Image = string.Empty,
                    Message = parentMessage,
                    Mobile = entity.FatherPhone,
                    IsGirl = 0,
                });
                await db.SaveChangesAsync(cancellationToken);
            }
        }

        return new SubmitPublicRegistrationResponseDto { Id = entity.Id };
    }

    private static (int Age, DateTime Birthdate) ResolveAgeAndBirthdate(
        string mode,
        SubmitPublicRegistrationRequestDto request)
    {
        if (mode == "wregister" && request.Age.HasValue)
        {
            var age = request.Age.Value;
            return (age, KuwaitTime.Now.AddYears(-age).Date);
        }

        if (!request.Birthdate.HasValue)
            throw new InvalidOperationException("يرجى إدخال تاريخ الميلاد");

        var birthDate = request.Birthdate.Value.Date;
        var calculatedAge = KuwaitTime.Now.Year - birthDate.Year;
        if (birthDate > KuwaitTime.Now.AddYears(-calculatedAge))
            calculatedAge--;

        return (calculatedAge, birthDate);
    }
}
