using AdminAPI.DTOs.Home;
using AdminAPI.Models;
using AdminAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Services;

public static class HomeStudentQueryBuilder
{
    public static IQueryable<RegisterForm> Build(
        Data.AdminDbContext db,
        ICurrentUserContext currentUser,
        HomeListFiltersDto filters,
        IReadOnlyList<int> teacherCircleIds)
    {
        var gender = currentUser.IsGirlTeacher ? "أنثى" : "ذكر";
        var query = db.RegisterForms.AsNoTracking().Where(x => x.StudentGender == gender);

        if (!currentUser.IsAdmin)
            query = query.Where(x => x.QuranCircleId != null && teacherCircleIds.Contains(x.QuranCircleId.Value));

        var circleId = filters.CircleQuery ?? filters.CircleId;
        if (circleId.HasValue)
            query = query.Where(x => x.QuranCircleId == circleId.Value);

        if (!string.IsNullOrWhiteSpace(filters.StudentName))
        {
            var name = filters.StudentName.Trim();
            query = query.Where(x =>
                (x.FullName != null && x.FullName.StartsWith(name)) ||
                x.StudentName.StartsWith(name));
        }

        if (filters.AgeFrom.HasValue)
            query = query.Where(x => x.Age >= filters.AgeFrom.Value);

        if (filters.AgeTo.HasValue)
            query = query.Where(x => x.Age <= filters.AgeTo.Value);

        if (!string.IsNullOrWhiteSpace(filters.FatherMobile))
            query = query.Where(x => x.FatherPhone.Contains(filters.FatherMobile));

        if (filters.SpecialOnly)
            query = query.Where(x => x.IsSpecial);

        if (filters.EliteOnly)
            query = query.Where(x => x.IsElite);

        if (filters.BoysOnly)
            query = query.Where(x => x.StudentGender == "ذكر");

        if (filters.GirlsOnly)
            query = query.Where(x => x.StudentGender == "أنثى");

        if (!string.IsNullOrWhiteSpace(filters.FormStatus))
        {
            var complete = filters.FormStatus == "نعم";
            query = complete
                ? query.Where(x => x.ParentFollowup != null)
                : query.Where(x => x.ParentFollowup == null);
        }

        if (currentUser.IsGirlTeacher && filters.WomanActivityTypeId.HasValue)
            query = query.Where(x => x.WomanActivityType == filters.WomanActivityTypeId.Value);

        return query.OrderByDescending(x => x.Id);
    }
}
