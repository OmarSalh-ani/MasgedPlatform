using AdminAPI.Data;
using AdminAPI.DTOs.StudentPlan;
using AdminAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Services;

public static class StudentPlanSurahExpander
{
    public static List<StudentPlanSurahOptionDto> GetExpandedSurahsList(IReadOnlyList<QuranSurah> surahs)
    {
        var items = surahs
            .OrderBy(x => x.SortOrder ?? x.Id)
            .Select(x => new StudentPlanSurahOptionDto { Id = x.Id, NameAr = "سورة " + x.NameAr })
            .ToList();

        for (var i = 1; i <= 60; i++)
            items.Add(new StudentPlanSurahOptionDto { Id = 1000 + i, NameAr = "حزب " + i });

        for (var i = 1; i <= 240; i++)
        {
            var hezb = ((i - 1) / 4) + 1;
            var quarter = ((i - 1) % 4) + 1;
            var qName = quarter switch
            {
                1 => "الأول",
                2 => "الثاني",
                3 => "الثالث",
                _ => "الرابع",
            };
            items.Add(new StudentPlanSurahOptionDto
            {
                Id = 2000 + i,
                NameAr = $"الربع {qName} من الحزب {hezb}",
            });
        }

        return items;
    }

    public static async Task<List<ExpandedPlanRow>> ExpandSurahIdAsync(
        AdminDbContext db,
        int inputId,
        int fromAyah,
        int toAyah,
        string planType,
        CancellationToken cancellationToken)
    {
        var list = new List<ExpandedPlanRow>();
        if (inputId <= 114)
        {
            list.Add(new ExpandedPlanRow
            {
                SurahId = inputId,
                FromAyah = fromAyah,
                ToAyah = toAyah,
                PlanType = planType,
            });
            return list;
        }

        IQueryable<HolyQuran> q = db.HolyQurans.AsNoTracking();
        if (inputId > 1000 && inputId <= 1100)
        {
            var hezb = inputId - 1000;
            q = q.Where(x => x.HezbNo == hezb);
        }
        else if (inputId > 2000 && inputId <= 2300)
        {
            var hezb = ((inputId - 2001) / 4) + 1;
            var quarter = ((inputId - 2001) % 4) + 1;
            q = q.Where(x => x.HezbNo == hezb && x.HezbQuarter == quarter);
        }
        else
        {
            return list;
        }

        var items = await q
            .GroupBy(x => x.SuraNo)
            .Select(g => new { sura_no = g.Key, min = g.Min(a => a.AyaNo), max = g.Max(a => a.AyaNo) })
            .ToListAsync(cancellationToken);

        foreach (var item in items)
        {
            list.Add(new ExpandedPlanRow
            {
                SurahId = item.sura_no,
                FromAyah = item.min,
                ToAyah = item.max,
                PlanType = planType,
            });
        }

        return list;
    }
}
