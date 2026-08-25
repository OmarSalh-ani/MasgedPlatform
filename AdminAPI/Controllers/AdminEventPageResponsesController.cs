using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.EventPageResponses;
using AdminAPI.Services;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/admineventpageresponses")]
public class AdminEventPageResponsesController(
    IEventPageResponseService responseService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<EventPageResponsesPageDto>> GetList(
        [FromQuery] EventPageResponseFiltersDto filters,
        CancellationToken cancellationToken = default)
    {
        var data = await responseService.GetListAsync(filters, cancellationToken);
        return Ok(data);
    }

    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportExcel(
        [FromQuery] EventPageResponseFiltersDto filters,
        CancellationToken cancellationToken = default)
    {
        var bytes = await responseService.ExportExcelAsync(filters, cancellationToken);
        var fileName = $"EventPageResponses_{KuwaitTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}
