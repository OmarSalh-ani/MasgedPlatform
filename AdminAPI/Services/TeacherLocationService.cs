using System.Text.Json;
using AdminAPI.Data;
using AdminAPI.DTOs.Teachers;
using AdminAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Services;

public class TeacherLocationService(AdminDbContext db)
{
    public async Task<List<int>> GetSelectedMosqueIdsAsync(
        int teacherId,
        CancellationToken cancellationToken)
    {
        var teacherUrls = await db.TeacherMapLocations
            .AsNoTracking()
            .Where(x => x.TeacherId == teacherId)
            .Select(x => x.MapURL)
            .ToListAsync(cancellationToken);

        var mosques = await db.Mosques.AsNoTracking().ToListAsync(cancellationToken);
        return mosques
            .Where(m => !string.IsNullOrEmpty(m.GoogleMapsUrl)
                && teacherUrls.Contains(m.GoogleMapsUrl))
            .Select(m => m.Id)
            .ToList();
    }

    public async Task<List<TeacherMapLocationDto>> GetManualLocationsAsync(
        int teacherId,
        CancellationToken cancellationToken)
    {
        var locations = await db.TeacherMapLocations
            .AsNoTracking()
            .Where(x => x.TeacherId == teacherId)
            .ToListAsync(cancellationToken);

        var mosqueUrls = await db.Mosques
            .AsNoTracking()
            .Where(m => m.GoogleMapsUrl != null)
            .Select(m => m.GoogleMapsUrl!)
            .ToListAsync(cancellationToken);

        return locations
            .Where(l => !mosqueUrls.Contains(l.MapURL))
            .Select(l => new TeacherMapLocationDto
            {
                Url = l.MapURL,
                Lat = l.Latitude,
                Lng = l.Longitude,
            })
            .ToList();
    }

    public async Task SaveLocationsAsync(
        int teacherId,
        IEnumerable<int> mosqueIds,
        string? manualLocationsJson,
        CancellationToken cancellationToken)
    {
        var oldLocations = await db.TeacherMapLocations
            .Where(x => x.TeacherId == teacherId)
            .ToListAsync(cancellationToken);
        db.TeacherMapLocations.RemoveRange(oldLocations);
        await db.SaveChangesAsync(cancellationToken);

        foreach (var mosqueId in mosqueIds)
        {
            var mosque = await db.Mosques.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == mosqueId, cancellationToken);
            if (mosque is null || string.IsNullOrEmpty(mosque.GoogleMapsUrl))
                continue;

            var coords = GoogleMapsCoordinateExtractor.ExtractCoordinates(mosque.GoogleMapsUrl);
            db.TeacherMapLocations.Add(new TeacherMapLocation
            {
                TeacherId = teacherId,
                MapURL = mosque.GoogleMapsUrl,
                Latitude = coords.Lat,
                Longitude = coords.Lng,
            });
        }

        var manualLocations = ParseManualLocations(manualLocationsJson);
        foreach (var loc in manualLocations)
        {
            if (string.IsNullOrWhiteSpace(loc.Url))
                continue;

            var lat = loc.Lat;
            var lng = loc.Lng;
            if (string.IsNullOrEmpty(lat) || string.IsNullOrEmpty(lng))
            {
                var coords = GoogleMapsCoordinateExtractor.ExtractCoordinates(loc.Url);
                lat = coords.Lat;
                lng = coords.Lng;
            }

            db.TeacherMapLocations.Add(new TeacherMapLocation
            {
                TeacherId = teacherId,
                MapURL = loc.Url.Trim(),
                Latitude = lat,
                Longitude = lng,
            });
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private static List<TeacherMapLocationDto> ParseManualLocations(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return [];

        try
        {
            return JsonSerializer.Deserialize<List<TeacherMapLocationDto>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public static List<int> ParseMosqueIds(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return [];

        return value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(s => int.TryParse(s, out var id) ? id : 0)
            .Where(id => id > 0)
            .ToList();
    }
}
