using AdminAPI.Data;
using AdminAPI.DTOs.Statistics;
using AdminAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class StatisticsRepository(AdminDbContext db) : IStatisticsRepository
{
    public async Task<StatisticsResponseDto> GetStatisticsAsync(
        bool isGirlTeacher,
        DateTime today,
        bool isWorkDay,
        CancellationToken cancellationToken = default)
    {
        var gender = isGirlTeacher ? "أنثى" : "ذكر";

        var totalStudents = await db.RegisterForms
            .AsNoTracking()
            .CountAsync(
                x => x.StudentGender == gender && x.QuranCircleId != null,
                cancellationToken);

        var presentToday = await db.CircleAttendances
            .AsNoTracking()
            .CountAsync(
                x => x.AttendanceDateTime == today
                     && x.IsHere
                     && x.RegisterForm.StudentGender == gender,
                cancellationToken);

        var departedToday = await db.CircleAttendances
            .AsNoTracking()
            .CountAsync(
                x => x.DepartureDate != null
                     && x.DepartureDate.Value.Date == today
                     && x.RegisterForm.StudentGender == gender,
                cancellationToken);

        var absentToday = 0;
        if (isWorkDay)
        {
            absentToday = totalStudents - presentToday;
        }

        var totalTeachers = await db.Teachers
            .AsNoTracking()
            .CountAsync(x => x.IsGirlTeacher == isGirlTeacher, cancellationToken);

        var totalCircles = await db.QuranCircles
            .AsNoTracking()
            .CountAsync(x => x.ForGirls == isGirlTeacher, cancellationToken);

        var specialStudents = await db.RegisterForms
            .AsNoTracking()
            .CountAsync(
                x => x.IsSpecial && x.StudentGender == gender,
                cancellationToken);

        return new StatisticsResponseDto
        {
            CircleStatistics = new CircleStatisticsDto
            {
                TotalStudents = totalStudents,
                PresentToday = presentToday,
                DepartedToday = departedToday,
                AbsentToday = absentToday,
            },
            AdditionalStatistics = new AdditionalStatisticsDto
            {
                TotalTeachers = totalTeachers,
                TotalCircles = totalCircles,
                SpecialStudents = specialStudents,
            },
        };
    }
}
