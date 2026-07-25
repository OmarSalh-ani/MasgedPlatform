using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Expensives;
using AdminAPI.Services;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminexpensives")]
public class AdminExpensivesController(IExpensiveService expensiveService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<ExpensiveListItemDto>>> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 0,
        CancellationToken cancellationToken = default)
    {
        var data = await expensiveService.GetListAsync(pageNumber, pageSize, cancellationToken);
        return Ok(data);
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ApiResponseDto<ExpensiveSummaryDto>>> GetSummary(
        CancellationToken cancellationToken = default)
    {
        var data = await expensiveService.GetSummaryAsync(cancellationToken);
        return Ok(new ApiResponseDto<ExpensiveSummaryDto>
        {
            Success = true,
            Message = "OK",
            Data = data
        });
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportToExcel(CancellationToken cancellationToken = default)
    {
        var bytes = await expensiveService.ExportToExcelAsync(cancellationToken);
        var fileName = $"Expenses_{KuwaitTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<ExpensiveDto>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var data = await expensiveService.GetByIdAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<ExpensiveDto>
        {
            Success = true,
            Message = "OK",
            Data = data
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<ExpensiveDto>>> Create(
        [FromForm] SaveExpensiveRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await expensiveService.CreateAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<ExpensiveDto>
        {
            Success = true,
            Message = "تم الحفظ",
            Data = data
        });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<ExpensiveDto>>> Update(
        int id,
        [FromForm] SaveExpensiveRequestDto request,
        CancellationToken cancellationToken)
    {
        var data = await expensiveService.UpdateAsync(id, request, cancellationToken);
        return Ok(new ApiResponseDto<ExpensiveDto>
        {
            Success = true,
            Message = "تم الحفظ",
            Data = data
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(
        int id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await expensiveService.DeleteAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = deleted,
            Message = deleted ? "تم الحذف" : "المصروف غير موجود",
            Data = deleted
        });
    }

    [HttpDelete("{id:int}/attachments/{fileName}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> DeleteAttachment(
        int id,
        string fileName,
        CancellationToken cancellationToken)
    {
        await expensiveService.DeleteAttachmentAsync(id, fileName, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = true,
            Message = "تم الحذف",
            Data = true
        });
    }

    [HttpGet("{id:int}/attachments/{fileName}")]
    public async Task<IActionResult> DownloadAttachment(
        int id,
        string fileName,
        CancellationToken cancellationToken)
    {
        var (path, safeName) = await expensiveService.GetAttachmentFileAsync(id, fileName, cancellationToken);
        return PhysicalFile(path, "application/octet-stream", safeName);
    }
}
