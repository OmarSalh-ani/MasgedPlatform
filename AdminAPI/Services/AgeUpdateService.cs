using AdminAPI.Data;
using AdminAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Services;

/// <summary>
/// Updates ages for all students based on birthdate (legacy <c>AgeUpdateHelper</c>).
/// Invoked by <see cref="AgeUpdateBackgroundService"/> once per day.
/// </summary>
public class AgeUpdateService(AdminDbContext db, ILogger<AgeUpdateService> logger) : IAgeUpdateService
{
    public async Task UpdateAgesIfNeededAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var students = await db.RegisterForms
                .Where(x => x.Birthdate.HasValue)
                .ToListAsync(cancellationToken);

            var updatedCount = 0;
            foreach (var student in students)
            {
                var calculatedAge = CalculateAge(student.Birthdate!.Value);
                if (student.Age != calculatedAge)
                {
                    student.Age = calculatedAge;
                    updatedCount++;
                }
            }

            if (updatedCount > 0)
                await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Age update completed. Updated {UpdatedCount} students on {Date:yyyy-MM-dd}",
                updatedCount,
                KuwaitTime.Now);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Age update failed");
        }
    }

    /// <summary>
    /// Calculates age in years from birthdate using Kuwait local date.
    /// </summary>
    private static int CalculateAge(DateTime birthdate)
    {
        var today = KuwaitTime.Today;
        var age = today.Year - birthdate.Year;

        if (birthdate.Date > today.AddYears(-age))
            age--;

        return age;
    }
}
