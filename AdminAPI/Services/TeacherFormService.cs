using AdminAPI.Data;
using AdminAPI.DTOs.Teachers;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public class TeacherFormService(
    AdminDbContext db,
    ITeacherRepository repository,
    ICurrentUserContext currentUser,
    TeacherLocationService locationService,
    IOptions<PublicSiteOptions> publicSiteOptions,
    IOptions<TeacherUploadOptions> uploadOptions) : ITeacherFormService
{
    public async Task<TeacherDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        EnsureCanModify();
        var teacher = await db.Teachers.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("المعلم غير موجود");

        return await MapToDtoAsync(teacher, cancellationToken);
    }

    public async Task<List<TeacherCircleOptionDto>> GetCirclesAsync(
        bool forGirls,
        CancellationToken cancellationToken = default)
    {
        EnsureCanModify();
        return await db.QuranCircles.AsNoTracking()
            .Where(x => x.ForGirls == forGirls)
            .OrderBy(x => x.Name)
            .Select(x => new TeacherCircleOptionDto { Id = x.Id, Name = x.Name })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TeacherMosqueOptionDto>> GetMosquesAsync(
        CancellationToken cancellationToken = default)
    {
        EnsureCanModify();
        return await db.Mosques.AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .Select(x => new TeacherMosqueOptionDto
            {
                Id = x.Id,
                Name = x.Name,
                GoogleMapsUrl = x.GoogleMapsUrl,
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<TeacherDto> CreateAsync(
        SaveTeacherRequestDto request,
        CancellationToken cancellationToken = default)
    {
        EnsureCanModify();
        ValidateSave(request, isEdit: false);

        var teacher = new Teacher
        {
            Name = request.Name.Trim(),
            Mobile = request.Mobile?.Trim(),
            Email = request.Email.Trim(),
            Password = request.Password!.Trim(),
            BaseSalary = request.BaseSalary,
            UsersManage = request.UsersManage,
            IsGirlTeacher = request.IsGirlTeacher,
            IsViewOnly = request.IsViewOnly,
            IsSupervisor = request.IsSupervisor,
            Image = await TeacherImageStorage.SaveAsync(
                request.Image,
                uploadOptions.Value.Directory,
                cancellationToken),
        };

        await repository.AddEntityAsync(teacher, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        await AssignCircleAsync(teacher.Id, request.CircleId, request.IsGirlTeacher, cancellationToken);
        await locationService.SaveLocationsAsync(
            teacher.Id,
            TeacherLocationService.ParseMosqueIds(request.SelectedMosqueIds),
            request.ManualLocationsJson,
            cancellationToken);

        return await MapToDtoAsync(teacher, cancellationToken);
    }

    public async Task<TeacherDto> UpdateAsync(
        int id,
        SaveTeacherRequestDto request,
        CancellationToken cancellationToken = default)
    {
        EnsureCanModify();
        ValidateSave(request, isEdit: true);

        var teacher = await repository.GetEntityByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("المعلم غير موجود");

        teacher.Name = request.Name.Trim();
        teacher.Mobile = request.Mobile?.Trim();
        teacher.Email = request.Email.Trim();
        teacher.BaseSalary = request.BaseSalary;
        teacher.UsersManage = request.UsersManage;
        teacher.IsGirlTeacher = request.IsGirlTeacher;
        teacher.IsViewOnly = request.IsViewOnly;
        teacher.IsSupervisor = request.IsSupervisor;

        if (!string.IsNullOrWhiteSpace(request.Password))
            teacher.Password = request.Password.Trim();

        if (request.RemoveImage)
        {
            TeacherImageStorage.DeleteIfExists(teacher.Image, uploadOptions.Value.Directory);
            teacher.Image = null;
        }
        else
        {
            var newImage = await TeacherImageStorage.SaveAsync(
                request.Image,
                uploadOptions.Value.Directory,
                cancellationToken);
            if (!string.IsNullOrEmpty(newImage))
            {
                TeacherImageStorage.DeleteIfExists(teacher.Image, uploadOptions.Value.Directory);
                teacher.Image = newImage;
            }
        }

        await repository.SaveChangesAsync(cancellationToken);
        await AssignCircleAsync(teacher.Id, request.CircleId, request.IsGirlTeacher, cancellationToken);
        await locationService.SaveLocationsAsync(
            teacher.Id,
            TeacherLocationService.ParseMosqueIds(request.SelectedMosqueIds),
            request.ManualLocationsJson,
            cancellationToken);

        return await MapToDtoAsync(teacher, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        EnsureCanModify();
        var imageFileName = await repository.DeleteWithRelatedAsync(
            id,
            currentUser.IsGirlTeacher,
            restrictCirclesToForGirls: false,
            cancellationToken);

        if (imageFileName is null)
            return false;

        TeacherImageStorage.DeleteIfExists(imageFileName, uploadOptions.Value.Directory);
        return true;
    }

    private async Task<TeacherDto> MapToDtoAsync(Teacher teacher, CancellationToken cancellationToken) =>
        new()
        {
            Id = teacher.Id,
            Name = teacher.Name,
            Mobile = teacher.Mobile,
            Email = teacher.Email,
            BaseSalary = teacher.BaseSalary,
            UsersManage = teacher.UsersManage,
            IsGirlTeacher = teacher.IsGirlTeacher ?? false,
            IsViewOnly = teacher.IsViewOnly,
            IsSupervisor = teacher.IsSupervisor,
            ImageUrl = TeacherImageStorage.BuildPublicImageUrl(
                teacher.Image,
                publicSiteOptions.Value.BaseUrl),
            SelectedMosqueIds = await locationService.GetSelectedMosqueIdsAsync(
                teacher.Id,
                cancellationToken),
            ManualLocations = await locationService.GetManualLocationsAsync(
                teacher.Id,
                cancellationToken),
        };

    private async Task AssignCircleAsync(
        int teacherId,
        int? circleId,
        bool isGirlTeacher,
        CancellationToken cancellationToken)
    {
        if (!circleId.HasValue)
            return;

        var circle = await db.QuranCircles.FirstOrDefaultAsync(
            x => x.Id == circleId.Value && x.ForGirls == isGirlTeacher,
            cancellationToken);
        if (circle is not null)
        {
            circle.TeacherId = teacherId;
            await db.SaveChangesAsync(cancellationToken);
        }
    }

    private static void ValidateSave(SaveTeacherRequestDto request, bool isEdit)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ValidationException("يرجى إدخال أسم المعلم");
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ValidationException("يرجى إدخال البريد الإلكتروني");
        if (!isEdit && string.IsNullOrWhiteSpace(request.Password))
            throw new ValidationException("يرجى إدخال كلمة المرور");
    }

    private void EnsureCanModify()
    {
        if (!currentUser.CanModify)
            throw new UnauthorizedAccessException("ليس لديك صلاحية لتعديل أو إضافة معلمين");
    }
}
