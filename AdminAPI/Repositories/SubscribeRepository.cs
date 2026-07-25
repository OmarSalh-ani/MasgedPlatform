using AdminAPI.Data;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class SubscribeRepository(AdminDbContext db) : ISubscribeRepository
{
    public Task<bool> MobileExistsAsync(string mobile, CancellationToken cancellationToken = default) =>
        db.AnnouncementContacts.AnyAsync(x => x.Mobile == mobile, cancellationToken);

    public async Task<AnnouncementContact> AddContactAsync(
        AnnouncementContact contact,
        CancellationToken cancellationToken = default)
    {
        await db.AnnouncementContacts.AddAsync(contact, cancellationToken);
        return contact;
    }

    public Task AddMessageAsync(AnnouncementMessage message, CancellationToken cancellationToken = default) =>
        db.AnnouncementMessages.AddAsync(message, cancellationToken).AsTask();

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
