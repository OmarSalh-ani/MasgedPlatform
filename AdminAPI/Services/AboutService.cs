using AdminAPI.Data;
using AdminAPI.DTOs.About;
using AdminAPI.Models;
using AdminAPI.Repositories.Interfaces;
using AdminAPI.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Services;

public class AboutService(IAboutRepository repository, AdminDbContext db, IMapper mapper) : IAboutService
{
    public async Task<AboutDto?> GetAsync(CancellationToken cancellationToken = default)
    {
        var about = await repository.GetFirstAsync(cancellationToken);
        return about is null ? null : mapper.Map<AboutDto>(about);
    }

    public async Task<AboutDto> SaveAsync(UpdateAboutRequestDto request, CancellationToken cancellationToken = default)
    {
        var about = await db.AboutAssociations.FirstOrDefaultAsync(cancellationToken);

        var address = NormalizeOptional(request.Address);
        var mapsUrl = NormalizeOptional(request.MapsUrl);

        if (about is not null)
        {
            about.Content = request.Content;
            about.Address = address;
            about.MapsUrl = mapsUrl;
        }
        else
        {
            about = new AboutAssociation
            {
                Content = request.Content,
                Address = address,
                MapsUrl = mapsUrl
            };
            await repository.AddAsync(about, cancellationToken);
        }

        await repository.SaveChangesAsync(cancellationToken);
        return mapper.Map<AboutDto>(about);
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
