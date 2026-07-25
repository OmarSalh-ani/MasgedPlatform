using AdminAPI.Data;
using AdminAPI.DTOs.ParentsFollowup;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public class ParentsFollowupService(
    AdminDbContext db,
    IParentsFollowupRepository repository,
    IOptions<ParentsFollowupUploadOptions> uploadOptions,
    IOptions<PublicSiteOptions> publicSiteOptions,
    IHttpContextAccessor httpContextAccessor) : IParentsFollowupService
{
    public async Task<ParentsFollowupDto?> GetByStudentIdAsync(
        int studentId,
        CancellationToken cancellationToken = default)
    {
        var student = await repository.GetStudentWithFollowupAsync(studentId, cancellationToken);
        if (student is null)
            return null;

        var followup = student.ParentFollowup;
        var requestBase = GetRequestBaseUrl();

        return new ParentsFollowupDto
        {
            StudentId = student.Id,
            StudentName = student.StudentName,
            Birthdate = student.Birthdate,
            StudentGender = student.StudentGender,
            FatherName = student.FatherName,
            FatherPhone = student.FatherPhone,
            Address = followup?.Address,
            MaritalStatus = followup?.MaritalStatus,
            HealthCondition = followup?.HealthCondition,
            HealthDetails = followup?.HealthDetails,
            LearningDifficulties = followup?.LearningDifficulties,
            LearningDifficultiesNotes = followup?.LearningDifficultiesNotes,
            PhotoUrl = ParentsFollowupPhotoStorage.NormalizePhotoUrl(followup?.PhotoPath, requestBase),
        };
    }

    public async Task SubmitAsync(
        int studentId,
        SaveParentsFollowupRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var student = await db.RegisterForms
            .Include(x => x.ParentFollowup)
            .FirstOrDefaultAsync(x => x.Id == studentId, cancellationToken);

        if (student is null)
            throw new ValidationException("الطالب غير موجود");

        var existingPhotoPath = student.ParentFollowup?.PhotoPath;
        string? photoPath = existingPhotoPath;

        if (request.Photo is { Length: > 0 })
        {
            photoPath = await ParentsFollowupPhotoStorage.SaveAsync(
                request.Photo,
                uploadOptions.Value.Directory,
                cancellationToken);

            if (string.IsNullOrEmpty(photoPath))
                throw new ValidationException("تعذر حفظ الصورة");
        }
        else if (string.IsNullOrWhiteSpace(existingPhotoPath))
        {
            throw new ValidationException("الرجاء رفع صورة شخصية للطالب");
        }

        var followup = student.ParentFollowup;
        if (followup is null)
        {
            followup = new ParentFollowup { StudentId = studentId };
            db.ParentFollowups.Add(followup);
        }

        followup.Address = request.Address;
        followup.MaritalStatus = request.MaritalStatus;
        followup.HealthCondition = request.HealthCondition;
        followup.HealthDetails = request.HealthDetails;
        followup.LearningDifficulties = request.LearningDifficulties;
        followup.LearningDifficultiesNotes = request.LearningDifficultiesNotes;
        followup.PhotoPath = photoPath;

        student.StudentName = request.StudentName.Trim();
        student.FullName = request.StudentName.Trim();
        student.Birthdate = request.Birthdate;
        student.StudentGender = request.StudentGender;
        student.FatherName = request.FatherName.Trim();
        student.FatherPhone = request.FatherPhone.Trim();

        await QueueWelcomeWhatsappAsync(student, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task QueueWelcomeWhatsappAsync(RegisterForm student, CancellationToken cancellationToken)
    {
        var tokens = new Dictionary<string, string>
        {
            ["اسم الطالب"] = student.StudentName ?? string.Empty,
            ["رقم جوال ولي الأمر"] = student.FatherPhone ?? string.Empty,
        };

        var message = await WhatsappPreconfiguredMessageFormatter.GetFormattedMessageAsync(
            db,
            WhatsappPreconfiguredMessageFormatter.ParentPortalWelcomeEvent,
            tokens,
            cancellationToken);

        if (string.IsNullOrEmpty(message))
            return;

        db.WhatsappTempTables.Add(new WhatsappTempTable
        {
            Message = message,
            Mobile = student.FatherPhone,
            IsGirl = student.IsGirl ?? 0,
        });
    }

    private string? GetRequestBaseUrl()
    {
        var request = httpContextAccessor.HttpContext?.Request;
        if (request is null)
            return publicSiteOptions.Value.BaseUrl.TrimEnd('/');

        return $"{request.Scheme}://{request.Host.Value}";
    }
}
