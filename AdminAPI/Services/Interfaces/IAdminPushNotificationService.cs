namespace AdminAPI.Services.Interfaces;

using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Home;
using AdminAPI.DTOs.PushNotifications;

public interface IAdminPushNotificationService
{
    Task<List<PushNotificationTeacherOptionDto>> GetTeachersAsync(CancellationToken cancellationToken = default);

    Task<PagedResultDto<HomeStudentListItemDto>> GetStudentsAsync(
        HomeListFiltersDto filters,
        CancellationToken cancellationToken = default);

    Task<HomeFilterOptionsDto> GetFilterOptionsAsync(CancellationToken cancellationToken = default);

    Task<SendAdminPushNotificationResultDto> SendAsync(
        SendAdminPushNotificationRequestDto request,
        CancellationToken cancellationToken = default);

    Task<SendAdminPushNotificationResultDto> SendToParentPhonesAsync(
        IReadOnlyList<string> phones,
        string title,
        string body,
        IReadOnlyDictionary<string, string> data,
        string context,
        CancellationToken cancellationToken = default);
}
