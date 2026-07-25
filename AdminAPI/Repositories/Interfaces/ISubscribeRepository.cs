using AdminAPI.Models;

namespace AdminAPI.Repositories.Interfaces;

public interface ISubscribeRepository
{
    Task<bool> MobileExistsAsync(string mobile, CancellationToken cancellationToken = default);

    Task<AnnouncementContact> AddContactAsync(
        AnnouncementContact contact,
        CancellationToken cancellationToken = default);

    Task AddMessageAsync(AnnouncementMessage message, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
