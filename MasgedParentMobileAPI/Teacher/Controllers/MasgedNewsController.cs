using MasgedTeacherMobileAPI.Data;
using MasgedTeacherMobileAPI.Dtos;
using MasgedTeacherMobileAPI.Extensions;
using MasgedTeacherMobileAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasgedTeacherMobileAPI.Controllers;

/// <summary>Shared mosque news (same table as MasgedParentMobileAPI).</summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MasgedNewsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var items = await db.NewsItems
            .AsNoTracking()
            .OrderByDescending(n => n.SortOrder)
            .ThenByDescending(n => n.NewsDate)
            .Select(n => new
            {
                n.Id,
                n.Title,
                n.Description,
                n.NewsDate,
                n.ImageUrl,
                n.LinkUrl,
            })
            .ToListAsync(cancellationToken);

        var dto = items.Select(item => new MasgedNewsListItemDto
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            NewsDate = item.NewsDate,
            ImageUrl = MediaUrlHelper.Resolve(item.ImageUrl),
            LinkUrl = string.IsNullOrWhiteSpace(item.LinkUrl) ? null : MediaUrlHelper.Resolve(item.LinkUrl),
        }).ToList();

        return this.ToActionResult(GlobalResponse.Ok(dto));
    }
}
