using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Extensions;
using MasgedTeacherMobileAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasgedTeacherMobileAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class QuranController(AppDbContext db) : ControllerBase
{
    private const int DefaultMaxPage = 604;
    private static int? _maxPageCache;
    private static readonly object MaxPageLock = new();

    [HttpGet("page")]
    public async Task<IActionResult> GetPage(
        [FromQuery] int? page,
        [FromQuery] int? surah,
        [FromQuery] int? surahId,
        [FromQuery] int? fromAyah,
        [FromQuery] int? from,
        [FromQuery] int? toAyah,
        [FromQuery] int? to,
        CancellationToken cancellationToken)
    {
        var targetSurahId = surah ?? surahId ?? 0;
        var fromAyahValue = fromAyah ?? from ?? 0;
        var toAyahValue = toAyah ?? to ?? 0;

        int currentPage;
        if (page.HasValue && page.Value > 0)
        {
            currentPage = page.Value;
        }
        else if (targetSurahId > 0)
        {
            currentPage = await GetStartPageAsync(targetSurahId, fromAyahValue, cancellationToken);
        }
        else
        {
            currentPage = 1;
        }

        var maxPage = await GetMaxPageAsync(targetSurahId, fromAyahValue, toAyahValue, cancellationToken);
        if (maxPage <= 0)
            maxPage = DefaultMaxPage;

        currentPage = Math.Max(1, Math.Min(currentPage, maxPage));

        var rows = await db.HolyQurans
            .AsNoTracking()
            .Where(h => h.page == currentPage)
            .OrderBy(h => h.sura_no)
            .ThenBy(h => h.aya_no)
            .ThenBy(h => h.line_start)
            .ToListAsync(cancellationToken);

        var pageJozz = rows.Count > 0 ? rows[0].jozz : 0;
        var displayRows = QuranPageBuilder.ApplySurahFilter(
            rows,
            targetSurahId,
            fromAyahValue,
            toAyahValue,
            out var isFiltered,
            out var surahName);

        var lines = QuranPageBuilder.BuildPageLines(displayRows);
        var highlightAyahs = isFiltered && fromAyahValue > 0
            ? QuranPageBuilder.GetHighlightAyahNumbers(displayRows, targetSurahId, fromAyahValue, toAyahValue)
            : [];

        var response = new QuranPageResponseDto
        {
            CurrentPage = currentPage,
            MaxPage = maxPage,
            Jozz = pageJozz,
            PageMeta = QuranPageBuilder.BuildPageMeta(pageJozz, surahName),
            SurahName = surahName,
            HasPrevious = currentPage > 1,
            HasNext = currentPage < maxPage,
            PreviousPage = currentPage > 1 ? currentPage - 1 : null,
            NextPage = currentPage < maxPage ? currentPage + 1 : null,
            IsFiltered = isFiltered,
            FilterSurahId = targetSurahId > 0 ? targetSurahId : null,
            FilterFromAyah = fromAyahValue > 0 ? fromAyahValue : null,
            FilterToAyah = toAyahValue > 0 ? toAyahValue : null,
            HighlightAyahNumbers = highlightAyahs,
            Lines = lines
        };

        return this.ToActionResult(GlobalResponse.Ok(response));
    }

    private async Task<int> GetMaxPageAsync(
        int targetSurahId,
        int fromAyah,
        int toAyah,
        CancellationToken cancellationToken)
    {
        if (targetSurahId > 0)
            return await GetFilteredMaxPageAsync(targetSurahId, toAyah, cancellationToken);

        if (_maxPageCache.HasValue)
            return _maxPageCache.Value;

        lock (MaxPageLock)
        {
            if (_maxPageCache.HasValue)
                return _maxPageCache.Value;
        }

        var max = await db.HolyQurans
            .AsNoTracking()
            .MaxAsync(h => (int?)h.page, cancellationToken) ?? 0;

        lock (MaxPageLock)
        {
            _maxPageCache = max;
        }

        return max;
    }

    private async Task<int> GetFilteredMaxPageAsync(
        int surahId,
        int toAyah,
        CancellationToken cancellationToken)
    {
        var query = db.HolyQurans.AsNoTracking().Where(h => h.sura_no == surahId);

        if (toAyah > 0)
            query = query.Where(h => h.aya_no <= toAyah);

        var max = await query.MaxAsync(h => (int?)h.page, cancellationToken);
        return max ?? DefaultMaxPage;
    }

    private async Task<int> GetStartPageAsync(
        int surahId,
        int ayahNo,
        CancellationToken cancellationToken)
    {
        var query = db.HolyQurans.AsNoTracking().Where(h => h.sura_no == surahId);

        if (ayahNo > 0)
            query = query.Where(h => h.aya_no >= ayahNo);

        var min = await query.MinAsync(h => (int?)h.page, cancellationToken);
        return min ?? 1;
    }
}
