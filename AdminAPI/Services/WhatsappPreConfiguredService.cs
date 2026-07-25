using AdminAPI.Data;
using AdminAPI.DTOs.WhatsappPreConfigured;
using AdminAPI.Models;
using AdminAPI.Services.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Services;

public class WhatsappPreConfiguredService(AdminDbContext db) : IWhatsappPreConfiguredService
{
    public async Task<List<WhatsappPreConfiguredMessageDto>> GetListAsync(
        CancellationToken cancellationToken = default)
    {
        await EnsureDefaultsAsync(cancellationToken);
        var rows = await db.WhatsappPreConfiguredMessages
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return rows.Select(MapDto).ToList();
    }

    public async Task<WhatsappPreConfiguredMessageDto> UpdateAsync(
        int id,
        UpdateWhatsappPreConfiguredRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var entity = await db.WhatsappPreConfiguredMessages.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new ValidationException("الرسالة غير موجودة");

        entity.WhatsappMessage = request.WhatsappMessage ?? string.Empty;
        await db.SaveChangesAsync(cancellationToken);
        return MapDto(entity);
    }

    public async Task<WhatsappPreConfiguredMessageDto> SetEnabledAsync(
        int id,
        SetWhatsappPreConfiguredEnabledRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var entity = await db.WhatsappPreConfiguredMessages.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new ValidationException("الرسالة غير موجودة");

        entity.IsEnabled = request.IsEnabled;
        await db.SaveChangesAsync(cancellationToken);
        return MapDto(entity);
    }

    public async Task<string> GetTestPreviewAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await db.WhatsappPreConfiguredMessages
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new ValidationException("الرسالة غير موجودة");

        return WhatsappPreConfiguredPreview.ApplySampleData(entity.WhatsappMessage);
    }

    private async Task EnsureDefaultsAsync(CancellationToken cancellationToken)
    {
        var masgedName = await MasgedBrandingHelper.GetMasgedNameAsync(db, cancellationToken);
        var changed = false;
        foreach (var eventKey in WhatsappPreConfiguredCatalog.EventKeys)
        {
            var existing = await db.WhatsappPreConfiguredMessages
                .FirstOrDefaultAsync(x => x.Event == eventKey, cancellationToken);

            if (existing != null)
                continue;

            db.WhatsappPreConfiguredMessages.Add(new WhatsappPreConfiguredMessage
            {
                Event = eventKey,
                WhatsappMessage = WhatsappPreConfiguredCatalog.GetDefaultMessage(eventKey, masgedName),
                IsEnabled = true,
            });
            changed = true;
        }

        if (changed)
            await db.SaveChangesAsync(cancellationToken);
    }

    private static WhatsappPreConfiguredMessageDto MapDto(WhatsappPreConfiguredMessage entity) =>
        new()
        {
            Id = entity.Id,
            Event = entity.Event,
            EventDisplayName = WhatsappPreConfiguredCatalog.GetDisplayName(entity.Event),
            EventDescription = WhatsappPreConfiguredCatalog.GetDescription(entity.Event),
            WhatsappMessage = entity.WhatsappMessage,
            IsEnabled = entity.IsEnabled,
            PreviewMessage = WhatsappPreConfiguredPreview.ApplySampleData(entity.WhatsappMessage),
        };
}
