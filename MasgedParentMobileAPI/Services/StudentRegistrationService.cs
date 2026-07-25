using MasgedParentMobileAPI.Configuration;
using MasgedParentMobileAPI.DTOs;
using MasgedParentMobileAPI.Models;
using MasgedTeacherMobileAPI.Helpers;
using Microsoft.EntityFrameworkCore;

namespace MasgedParentMobileAPI.Services;

public class StudentRegistrationService(
    NewMasgedTeacherAPIDBContext db,
    JwtTokenService jwtTokenService)
{
    public async Task<StudentRegistrationResponseDto> RegisterAsync(
        StudentRegistrationRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ValidateParentRequest(request);

        var mode = NormalizeMode(request.Mode);
        var fatherPhone = PhoneNormalizer.ToCanonical(request.ParentPhone1);
        var phoneVariants = PhoneNormalizer.GetVariants(request.ParentPhone1).ToList();

        var existingWithPassword = await db.RegisterForms
            .AnyAsync(
                r => !string.IsNullOrWhiteSpace(r.ThePassword) &&
                     (phoneVariants.Contains(r.FatherPhone) ||
                      phoneVariants.Contains(r.FatherPhone2)),
                cancellationToken);

        if (existingWithPassword)
            throw new InvalidOperationException("تم التسجيل مسبقًا، يرجى تسجيل الدخول");

        var fatherPhone2 = string.IsNullOrWhiteSpace(request.ParentPhone2)
            ? null
            : PhoneNormalizer.ToCanonical(request.ParentPhone2);

        var fatherName = request.FatherName?.Trim() ?? string.Empty;
        var createdStudents = new List<RegisterForm>();

        foreach (var entry in request.Students)
        {
            ValidateStudentEntry(mode, entry);

            var activity = await db.WomanActivities
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == entry.WomanActivityTypeId, cancellationToken)
                ?? throw new InvalidOperationException("نوع النشاط غير صالح");

            if (!await GetRegistrationEnabledAsync(activity.ForGirl, cancellationToken))
                throw new InvalidOperationException("التسجيل مغلق حالياً");

            var (age, birthDate) = ResolveAgeAndBirthdate(mode, entry);
            if (age < 5)
                throw new InvalidOperationException("يجب أن لا يقل العمر عن 5 سنوات");

            var isGirl = activity.ForGirl;
            var entity = new RegisterForm
            {
                Age = age,
                CreatedAt = KuwaitTime.Now,
                FatherPhone = fatherPhone,
                FatherPhone2 = fatherPhone2,
                StudentGender = isGirl ? "أنثى" : "ذكر",
                StudentName = entry.FullName.Trim(),
                StudentPhone = string.Empty,
                QuranCircleId = null,
                FatherName = fatherName,
                Birthdate = birthDate,
                FullName = entry.FullName.Trim(),
                WomanActivityType = entry.WomanActivityTypeId,
                LearnCertificate = entry.LearnCertificate,
                IsGirl = isGirl ? 1 : 0,
                ThePassword = request.Password,
            };

            db.RegisterForms.Add(entity);
            createdStudents.Add(entity);
        }

        await db.SaveChangesAsync(cancellationToken);

        foreach (var (entity, entry) in createdStudents.Zip(request.Students))
        {
            db.ParentFollowups.Add(new ParentFollowup
            {
                StudentId = entity.Id,
                Address = entry.Address.Trim(),
                MaritalStatus = entry.MaritalStatus.Trim(),
                HealthCondition = entry.HasHealthCondition ? "نعم" : "لا",
                HealthDetails = entry.HealthDetails?.Trim() ?? string.Empty,
                LearningDifficulties = entry.HasLearningDifficulties ? "نعم" : "لا",
                LearningDifficultiesNotes = entry.LearningDifficultiesDetails?.Trim() ?? string.Empty,
                PhotoPath = string.Empty,
            });
        }

        await db.SaveChangesAsync(cancellationToken);

        var first = createdStudents[0];
        var token = jwtTokenService.GenerateToken(first.Id, fatherPhone, fatherName);

        return new StudentRegistrationResponseDto
        {
            Token = token,
            ParentId = first.Id,
            FatherName = fatherName,
            Phone = fatherPhone,
            StudentIds = createdStudents.Select(s => s.Id).ToList(),
        };
    }

    private static void ValidateParentRequest(StudentRegistrationRequestDto request)
    {
        if (request.Students.Count == 0)
            throw new InvalidOperationException("يرجى إضافة طالب واحد على الأقل");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new InvalidOperationException("يرجى إدخال كلمة المرور");

        if (request.Password.Length < 6)
            throw new InvalidOperationException("كلمة المرور يجب أن تكون 6 أحرف على الأقل");

        if (string.IsNullOrWhiteSpace(request.ParentPhoneCountryIso) ||
            request.ParentPhoneCountryIso.Length < 2)
            throw new InvalidOperationException("يرجى اختيار رمز الدولة");

        if (string.IsNullOrWhiteSpace(request.ParentPhone1))
            throw new InvalidOperationException("يرجى إدخال رقم الجوال");

        if (!IsValidLocalPhone(request.ParentPhoneCountryIso, request.ParentPhone1))
            throw new InvalidOperationException(PhoneErrorMessage(request.ParentPhoneCountryIso));

        if (!string.IsNullOrWhiteSpace(request.ParentPhone2) &&
            !IsValidLocalPhone(
                string.IsNullOrWhiteSpace(request.ParentPhone2CountryIso)
                    ? "KW"
                    : request.ParentPhone2CountryIso,
                request.ParentPhone2))
        {
            throw new InvalidOperationException(
                PhoneErrorMessage(request.ParentPhone2CountryIso ?? "KW"));
        }
    }

    private static void ValidateStudentEntry(string mode, StudentRegistrationEntryDto entry)
    {
        if (string.IsNullOrWhiteSpace(entry.FullName))
            throw new InvalidOperationException("يرجى إدخال اسم الطالب");

        if (entry.WomanActivityTypeId <= 0)
            throw new InvalidOperationException("يرجى اختيار نوع النشاط");

        if (string.IsNullOrWhiteSpace(entry.Address))
            throw new InvalidOperationException("يرجى إدخال العنوان");

        if (string.IsNullOrWhiteSpace(entry.MaritalStatus))
            throw new InvalidOperationException("يرجى اختيار الحالة الاجتماعية");

        if (mode == "wregister")
        {
            if (entry.Age is not >= 5)
                throw new InvalidOperationException("يرجى إدخال العمر");
        }
        else if (!entry.Birthdate.HasValue)
        {
            throw new InvalidOperationException("يرجى إدخال تاريخ الميلاد");
        }
    }

    private async Task<bool> GetRegistrationEnabledAsync(
        bool forGirl,
        CancellationToken cancellationToken)
    {
        try
        {
            var setting = await db.AppSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Key == "RegistrationEnabled" && x.ForGirl == forGirl,
                    cancellationToken);

            if (setting != null && bool.TryParse(setting.Value, out var enabled))
                return enabled;
        }
        catch
        {
            // Fall through to disabled when settings cannot be read.
        }

        return false;
    }

    private static (int Age, DateTime Birthdate) ResolveAgeAndBirthdate(
        string mode,
        StudentRegistrationEntryDto entry)
    {
        if (mode == "wregister" && entry.Age.HasValue)
        {
            var age = entry.Age.Value;
            return (age, KuwaitTime.Now.AddYears(-age).Date);
        }

        if (!entry.Birthdate.HasValue)
            throw new InvalidOperationException("يرجى إدخال تاريخ الميلاد");

        var birthDate = entry.Birthdate.Value.Date;
        var calculatedAge = KuwaitTime.Now.Year - birthDate.Year;
        if (birthDate > KuwaitTime.Now.AddYears(-calculatedAge))
            calculatedAge--;

        return (calculatedAge, birthDate);
    }

    private static string NormalizeMode(string? mode)
    {
        var normalized = mode?.Trim().ToLowerInvariant();
        return normalized switch
        {
            "mregister" => "mregister",
            "wregister" => "wregister",
            _ => "default",
        };
    }

    private static string DigitsOnly(string? value) =>
        value == null ? string.Empty : new string(value.Where(char.IsDigit).ToArray());

    private static bool IsValidLocalPhone(string countryIso, string phone)
    {
        var digits = DigitsOnly(phone);
        if (string.IsNullOrEmpty(digits))
            return false;

        if (countryIso.Equals("KW", StringComparison.OrdinalIgnoreCase))
            return digits.Length == 8;

        return digits.Length is >= 7 and <= 15;
    }

    private static string PhoneErrorMessage(string countryIso) =>
        countryIso.Equals("KW", StringComparison.OrdinalIgnoreCase)
            ? "يجب أن يكون رقم الهاتف 8 أرقام (رقم كويتي صحيح)"
            : "أدخل رقم الجوال بدون رمز الدولة (7–15 رقماً)";
}
