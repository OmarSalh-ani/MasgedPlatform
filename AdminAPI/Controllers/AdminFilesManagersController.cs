using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.FilesManager;
using AdminAPI.Helpers;
using AdminAPI.Services;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminfilesmanagers")]
public class AdminFilesManagersController(IFilesManagerService filesManagerService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<FilesManagerListItemDto>>> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 0,
        CancellationToken cancellationToken = default)
    {
        var data = await filesManagerService.GetListAsync(pageNumber, pageSize, cancellationToken);
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<FilesManagerDto>>> GetById(
        int id,
        CancellationToken cancellationToken = default)
    {
        var data = await filesManagerService.GetByIdAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<FilesManagerDto>
        {
            Success = true,
            Message = "OK",
            Data = data
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<FilesManagerDto>>> Create(
        [FromForm] SaveFilesManagerRequestDto request,
        CancellationToken cancellationToken = default)
    {
        EnsureCanModify();
        var data = await filesManagerService.CreateAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<FilesManagerDto>
        {
            Success = true,
            Message = "تم رفع الملف بنجاح",
            Data = data
        });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<FilesManagerDto>>> Update(
        int id,
        [FromForm] SaveFilesManagerRequestDto request,
        CancellationToken cancellationToken = default)
    {
        EnsureCanModify();
        var data = await filesManagerService.UpdateAsync(id, request, cancellationToken);
        return Ok(new ApiResponseDto<FilesManagerDto>
        {
            Success = true,
            Message = "تم تحديث الملف بنجاح",
            Data = data
        });
    }

    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportToExcel(CancellationToken cancellationToken = default)
    {
        var bytes = await filesManagerService.ExportToExcelAsync(cancellationToken);
        var fileName = $"Files_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(
        int id,
        CancellationToken cancellationToken = default)
    {
        EnsureCanModify();
        var deleted = await filesManagerService.DeleteAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = deleted,
            Message = deleted ? "تم حذف الملف بنجاح" : "الملف غير موجود",
            Data = deleted
        });
    }

    private void EnsureCanModify()
    {
        if (AdminUserClaims.IsViewOnly(User))
            throw new UnauthorizedAccessException("ليس لديك صلاحية لتعديل أو إضافة ملفات");
    }
}
