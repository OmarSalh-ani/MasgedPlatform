using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.WomansActivities;
using AdminAPI.Services;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminwomansactivities")]
public class AdminWomansActivitiesController(IWomansActivityService womansActivityService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<WomanActivityListItemDto>>> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 0,
        CancellationToken cancellationToken = default)
    {
        var data = await womansActivityService.GetListAsync(pageNumber, pageSize, cancellationToken);
        return Ok(data);
    }

    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportToExcel(CancellationToken cancellationToken = default)
    {
        var bytes = await womansActivityService.ExportToExcelAsync(cancellationToken);
        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Activities.xlsx");
    }
}
