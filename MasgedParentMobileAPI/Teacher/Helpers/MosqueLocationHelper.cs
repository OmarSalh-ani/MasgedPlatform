using System.Globalization;
using MasgedTeacherMobileAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace MasgedTeacherMobileAPI.Helpers;

public static class MosqueLocationHelper
{
    public const double MaxDistanceMeters = 200;
    private const double EarthRadiusMeters = 6371000;

    public static async Task<bool> IsWithinMosqueRadiusAsync(
        AppDbContext db,
        int teacherId,
        double latitude,
        double longitude,
        CancellationToken cancellationToken,
        double maxDistanceMeters = MaxDistanceMeters)
    {
        var nearestDistance = await GetNearestMosqueDistanceMetersAsync(
            db,
            teacherId,
            latitude,
            longitude,
            cancellationToken);

        return nearestDistance is { } distance && distance <= maxDistanceMeters;
    }

    public static string OutsideMosqueMessageForStudentAttendance(bool isDeparture = false) =>
        isDeparture
            ? $"يجب أن تكون داخل نطاق المسجد المحدد لك لتسجيل انصراف الطلاب. المسافة المسموحة: {MaxDistanceMeters} متر"
            : $"يجب أن تكون داخل نطاق المسجد المحدد لك لتسجيل حضور الطلاب. المسافة المسموحة: {MaxDistanceMeters} متر";

    public static async Task<double?> GetNearestMosqueDistanceMetersAsync(
        AppDbContext db,
        int teacherId,
        double latitude,
        double longitude,
        CancellationToken cancellationToken)
    {
        var teacherLocations = await db.TeacherMapLocations
            .AsNoTracking()
            .Where(x => x.TeacherId == teacherId && x.Latitude != null && x.Longitude != null)
            .ToListAsync(cancellationToken);

        if (teacherLocations.Count == 0)
            return null;

        double? nearest = null;

        foreach (var loc in teacherLocations)
        {
            if (!double.TryParse(loc.Latitude, NumberStyles.Any, CultureInfo.InvariantCulture, out var mosqueLatitude))
                continue;
            if (!double.TryParse(loc.Longitude, NumberStyles.Any, CultureInfo.InvariantCulture, out var mosqueLongitude))
                continue;

            var distanceMeters = CalculateDistanceMeters(mosqueLatitude, mosqueLongitude, latitude, longitude);
            nearest = nearest is null
                ? distanceMeters
                : Math.Min(nearest.Value, distanceMeters);
        }

        return nearest;
    }

    public static double CalculateDistanceMeters(
        double mosqueLatitude,
        double mosqueLongitude,
        double latitude,
        double longitude)
    {
        var lat1Rad = mosqueLatitude * Math.PI / 180.0;
        var lat2Rad = latitude * Math.PI / 180.0;
        var deltaLatRad = (latitude - mosqueLatitude) * Math.PI / 180.0;
        var deltaLonRad = (longitude - mosqueLongitude) * Math.PI / 180.0;

        var a = Math.Sin(deltaLatRad / 2) * Math.Sin(deltaLatRad / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(deltaLonRad / 2) * Math.Sin(deltaLonRad / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusMeters * c;
    }

    public static string FormatDistanceArabic(double meters)
    {
        if (meters < 1000)
            return $"{Math.Round(meters)} متر";

        var km = meters / 1000;
        return km >= 10
            ? $"{Math.Round(km)} كم"
            : $"{km.ToString("0.#", CultureInfo.InvariantCulture)} كم";
    }
}
