using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.QuranCircles;
using AdminAPI.Services;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminqurancircles")]
public class AdminQuranCirclesController(IQuranCircleService quranCircleService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<QuranCircleListItemDto>>> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 0,
        [FromQuery] int? teacher = null,
        CancellationToken cancellationToken = default)
    {
        var data = await quranCircleService.GetListAsync(
            pageNumber,
            pageSize,
            teacher,
            cancellationToken);
        return Ok(data);
    }

    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportToExcel(CancellationToken cancellationToken = default)
    {
        var bytes = await quranCircleService.ExportToExcelAsync(cancellationToken);
        var fileName = $"Circles_{KuwaitTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }
}
