using AdminAPI.DTOs.Common;
using AdminAPI.DTOs.Teachers;
using AdminAPI.Helpers;
using AdminAPI.Services;
using AdminAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/adminteacher")]
public class AdminTeacherController(
    ITeacherService teacherService,
    ITeacherFormService teacherFormService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<TeacherListItemDto>>> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 0,
        CancellationToken cancellationToken = default)
    {
        var data = await teacherService.GetListAsync(pageNumber, pageSize, cancellationToken);
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<TeacherDto>>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        EnsureCanModify();
        var data = await teacherFormService.GetByIdAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<TeacherDto> { Success = true, Message = "OK", Data = data });
    }

    [HttpGet("circles")]
    public async Task<ActionResult<ApiResponseDto<List<TeacherCircleOptionDto>>>> GetCircles(
        [FromQuery] bool forGirls,
        CancellationToken cancellationToken)
    {
        EnsureCanModify();
        var data = await teacherFormService.GetCirclesAsync(forGirls, cancellationToken);
        return Ok(new ApiResponseDto<List<TeacherCircleOptionDto>>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpGet("mosques")]
    public async Task<ActionResult<ApiResponseDto<List<TeacherMosqueOptionDto>>>> GetMosques(
        CancellationToken cancellationToken)
    {
        EnsureCanModify();
        var data = await teacherFormService.GetMosquesAsync(cancellationToken);
        return Ok(new ApiResponseDto<List<TeacherMosqueOptionDto>>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpGet("{id:int}/card-print")]
    public async Task<ActionResult<ApiResponseDto<TeacherCardPrintDto>>> GetCardPrint(
        int id,
        CancellationToken cancellationToken)
    {
        var data = await teacherService.GetCardPrintAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<TeacherCardPrintDto>
        {
            Success = true,
            Message = "OK",
            Data = data,
        });
    }

    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportToExcel(CancellationToken cancellationToken = default)
    {
        var bytes = await teacherService.ExportToExcelAsync(cancellationToken);
        var fileName = $"Teachers_{KuwaitTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<TeacherDto>>> Create(
        [FromForm] SaveTeacherRequestDto request,
        CancellationToken cancellationToken)
    {
        EnsureCanModify();
        var data = await teacherFormService.CreateAsync(request, cancellationToken);
        return Ok(new ApiResponseDto<TeacherDto>
        {
            Success = true,
            Message = "تم إضافة المعلم بنجاح",
            Data = data,
        });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<TeacherDto>>> Update(
        int id,
        [FromForm] SaveTeacherRequestDto request,
        CancellationToken cancellationToken)
    {
        EnsureCanModify();
        var data = await teacherFormService.UpdateAsync(id, request, cancellationToken);
        return Ok(new ApiResponseDto<TeacherDto>
        {
            Success = true,
            Message = "تم تحديث بيانات المعلم بنجاح",
            Data = data,
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(
        int id,
        [FromQuery] bool fromForm = false,
        CancellationToken cancellationToken = default)
    {
        EnsureCanModify();
        var deleted = fromForm
            ? await teacherFormService.DeleteAsync(id, cancellationToken)
            : await teacherService.DeleteAsync(id, cancellationToken);
        return Ok(new ApiResponseDto<bool>
        {
            Success = deleted,
            Message = deleted ? "تم حذف المعلم بنجاح" : "المعلم غير موجود",
            Data = deleted,
        });
    }

    private void EnsureCanModify()
    {
        if (AdminUserClaims.IsViewOnly(User))
            throw new UnauthorizedAccessException("ليس لديك صلاحية لتعديل أو إضافة معلمين");
    }
}
