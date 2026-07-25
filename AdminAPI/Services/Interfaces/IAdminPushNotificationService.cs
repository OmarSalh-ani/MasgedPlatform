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
}
