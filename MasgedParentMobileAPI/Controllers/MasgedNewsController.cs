using MasgedParentMobileAPI.Configuration;
using MasgedParentMobileAPI.DTOs;
using MasgedParentMobileAPI.Models;
using MasgedParentMobileAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MasgedParentMobileAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MasgedNewsController : ControllerBase
{
    private readonly NewMasgedTeacherAPIDBContext _db;
    private readonly string _mediaBaseUrl;

    public MasgedNewsController(
        NewMasgedTeacherAPIDBContext db,
        IOptions<ApiSettings> apiSettings)
    {
        _db = db;
        _mediaBaseUrl = apiSettings.Value.MediaBaseUrl;
    }

    [HttpGet]
    public async Task<ActionResult<List<NewsItemDto>>> Get()
    {
        var items = await _db.NewsItems
            .OrderByDescending(n => n.SortOrder)
            .ThenByDescending(n => n.NewsDate)
            .Select(n => new NewsItemDto
            {
                Id = n.Id,
                Title = n.Title,
                Description = n.Description,
                NewsDate = n.NewsDate,
                ImageUrl = n.ImageUrl,
            })
            .ToListAsync();

        foreach (var item in items)
            item.ImageUrl = MediaUrlHelper.Resolve(item.ImageUrl, _mediaBaseUrl) ?? string.Empty;

        return Ok(items);
    }
}
