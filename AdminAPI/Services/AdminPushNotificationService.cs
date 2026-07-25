using AdminAPI.Configuration;
using AdminAPI.Data;
using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Home;
using AdminAPI.DTOs.PushNotifications;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using FluentValidation;
using Masged.WhatsApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AdminAPI.Services;

public sealed partial class AdminPushNotificationService(
    AdminDbContext db,
    IHomeService homeService,
    IHomeRepository homeRepository,
    ICurrentUserContext currentUser,
    IOptions<FirebaseSettings> firebaseSettings,
    ILogger<AdminPushNotificationService> logger,
    IHostEnvironment environment) : IAdminPushNotificationService
{
    private readonly FirebaseSettings _firebaseSettings = firebaseSettings.Value;

    public async Task<List<PushNotificationTeacherOptionDto>> GetTeachersAsync(
        CancellationToken cancellationToken = default)
    {
        return await db.Teachers
            .AsNoTracking()
            .Where(t => t.IsGirlTeacher == currentUser.IsGirlTeacher)
            .OrderBy(t => t.Name)
            .Select(t => new PushNotificationTeacherOptionDto
            {
                Id = t.Id,
                Name = t.Name,
            })
            .ToListAsync(cancellationToken);
    }

    public Task<PagedResultDto<HomeStudentListItemDto>> GetStudentsAsync(
        HomeListFiltersDto filters,
        CancellationToken cancellationToken = default) =>
        homeService.GetListAsync(filters, cancellationToken);

    public Task<HomeFilterOptionsDto> GetFilterOptionsAsync(
        CancellationToken cancellationToken = default) =>
        homeService.GetFilterOptionsAsync(cancellationToken);

    public async Task<SendAdminPushNotificationResultDto> SendAsync(
        SendAdminPushNotificationRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.CanModify)
            throw new UnauthorizedAccessException("ليس لديك صلاحية لإرسال الإشعارات");

        var audience = request.Audience.Trim().ToLowerInvariant();
        var title = request.Title.Trim();
        var body = request.Body.Trim();

        if (audience == "teachers")
            return await SendToTeachersAsync(request, title, body, cancellationToken);

        if (audience == "parents")
            return await SendToParentsAsync(request, title, body, cancellationToken);

        throw new ValidationException("نوع الجمهور غير صالح");
    }

    private async Task<SendAdminPushNotificationResultDto> SendToTeachersAsync(
        SendAdminPushNotificationRequestDto request,
        string title,
        string body,
        CancellationToken cancellationToken)
    {
        var teacherIds = request.TargetAll
            ? await db.Teachers
                .AsNoTracking()
                .Where(t => t.IsGirlTeacher == currentUser.IsGirlTeacher)
                .Select(t => t.Id)
                .ToListAsync(cancellationToken)
            : request.TeacherIds.Distinct().ToList();

        if (teacherIds.Count == 0)
            throw new ValidationException("لا يوجد معلمون مستهدفون");

        var tokens = await db.TeacherDeviceTokens
            .AsNoTracking()
            .Where(t => teacherIds.Contains(t.TeacherId))
            .Select(t => t.FcmToken)
            .Distinct()
            .ToListAsync(cancellationToken);

        var teachersWithTokens = await db.TeacherDeviceTokens
            .AsNoTracking()
            .Where(t => teacherIds.Contains(t.TeacherId))
            .Select(t => t.TeacherId)
            .Distinct()
            .CountAsync(cancellationToken);

        var (successCount, failureCount) = await SendAdminMulticastAsync(
            tokens,
            title,
            body,
            DeviceTokenKind.Teacher,
            cancellationToken);

        return BuildResult(teacherIds.Count, teachersWithTokens, tokens.Count, successCount, failureCount);
    }

    private async Task<SendAdminPushNotificationResultDto> SendToParentsAsync(
        SendAdminPushNotificationRequestDto request,
        string title,
        string body,
        CancellationToken cancellationToken)
    {
        var phones = await ResolveParentPhonesAsync(request, cancellationToken);
        if (phones.Count == 0)
            throw new ValidationException("لا يوجد أولياء أمور مستهدفون");

        var phoneVariants = phones
            .SelectMany(PhoneNormalizer.GetVariants)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        var matchedPhones = await db.ParentDeviceTokens
            .AsNoTracking()
            .Where(t => phoneVariants.Contains(t.ParentPhone))
            .Select(t => t.ParentPhone)
            .Distinct()
            .ToListAsync(cancellationToken);

        var tokens = await db.ParentDeviceTokens
            .AsNoTracking()
            .Where(t => phoneVariants.Contains(t.ParentPhone))
            .Select(t => t.FcmToken)
            .Distinct()
            .ToListAsync(cancellationToken);

        var phonesWithTokens = CountPhonesWithTokens(phones, matchedPhones);

        var (successCount, failureCount) = await SendAdminMulticastAsync(
            tokens,
            title,
            body,
            DeviceTokenKind.Parent,
            cancellationToken);

        return BuildResult(phones.Count, phonesWithTokens, tokens.Count, successCount, failureCount);
    }

    private async Task<List<string>> ResolveParentPhonesAsync(
        SendAdminPushNotificationRequestDto request,
        CancellationToken cancellationToken)
    {
        if (request.TargetAll)
        {
            var teacherCircleIds = await GetTeacherCircleIdsAsync(cancellationToken);
            var filters = new HomeListFiltersDto { PageNumber = 1, PageSize = 1 };
            var query = HomeStudentQueryBuilder.Build(db, currentUser, filters, teacherCircleIds);

            return await query
                .Where(x => x.FatherPhone != null && x.FatherPhone != string.Empty)
                .Select(x => x.FatherPhone)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        return await db.RegisterForms
            .AsNoTracking()
            .Where(x => request.StudentIds.Contains(x.Id))
            .Where(x => x.FatherPhone != null && x.FatherPhone != string.Empty)
            .Select(x => x.FatherPhone)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    private async Task<List<int>> GetTeacherCircleIdsAsync(CancellationToken cancellationToken)
    {
        if (currentUser.IsAdmin)
            return [];

        return await homeRepository.GetTeacherCircleIdsAsync(currentUser.TeacherId, cancellationToken);
    }

    private static int CountPhonesWithTokens(IReadOnlyList<string> phones, IReadOnlyList<string> matchedPhones)
    {
        var matchedVariants = matchedPhones
            .SelectMany(PhoneNormalizer.GetVariants)
            .ToHashSet(StringComparer.Ordinal);

        return phones.Count(phone =>
            PhoneNormalizer.GetVariants(phone).Any(variant => matchedVariants.Contains(variant)));
    }

    private static SendAdminPushNotificationResultDto BuildResult(
        int recipientsResolved,
        int recipientsWithTokens,
        int tokensAttempted,
        int successCount,
        int failureCount) =>
        new()
        {
            RecipientsResolved = recipientsResolved,
            RecipientsWithoutTokens = recipientsResolved - recipientsWithTokens,
            TokensAttempted = tokensAttempted,
            SuccessCount = successCount,
            FailureCount = failureCount,
        };
}
